namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Connector;
    using global::SqlStreamStore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SqlStreamStore;

    public abstract class Runner<TContext> : IDisposable
        where TContext : RunnerDbContext<TContext>
    {
        private readonly ConcurrentBag<Task> _tasks;
        private readonly ConcurrentBag<IAllStreamSubscription> _subscriptions;
        private readonly EnvelopeFactory _envelopeFactory;
        private readonly ILogger _logger;
        private readonly ConnectedProjector<TContext> _projector;

        // ReSharper disable once StaticMemberInGenericType
        public static string RunnerName { get; private set; }

        public int CatchupThreshold { get; set; } = 1000;
        public int CatchupPageSize { get; set; } = 1000;

        protected Runner(
            string runnerName,
            EnvelopeFactory envelopeFactory,
            ILogger logger,
            IEnumerable<ConnectedProjectionHandler<TContext>> handlers)
        {
            RunnerName = runnerName;
            _envelopeFactory = envelopeFactory;
            _logger = logger;
            _tasks = new ConcurrentBag<Task>();
            _subscriptions = new ConcurrentBag<IAllStreamSubscription>();
            _projector = new ConnectedProjector<TContext>(Resolve.WhenEqualToHandlerMessageType(handlers.ToArray()));
        }

        protected Runner(
            string runnerName,
            EnvelopeFactory envelopeFactory,
            ILogger logger,
            params IEnumerable<ConnectedProjectionHandler<TContext>>[] handlers) :
            this(runnerName, envelopeFactory, logger, handlers.SelectMany(x => x))
        { }

        protected Runner(
            string runnerName,
            EnvelopeFactory envelopeFactory,
            ILogger logger,
            params ConnectedProjection<TContext>[] projections) :
            this(runnerName, envelopeFactory, logger, projections.SelectMany(x => x.Handlers))
        { }

        // sync handling ohne catchup
        [Obsolete("Prefer using StartAsync()")]
        public void Handle(IReadonlyStreamStore streamStore, Func<Owned<TContext>> contextFactory)
        {
            CleanUp();

            long? position;
            using (var context = contextFactory().Value)
            {
                position = context
                    .ProjectionStates
                    .SingleOrDefault(p => p.Name == RunnerName)
                    ?.Position;
            }

            _tasks.Add(
                Task.Factory.StartNew(
                    () => CreateSubscription(streamStore, contextFactory, position),
                    TaskCreationOptions.LongRunning));
        }

        private void CreateSubscription(IReadonlyStreamStore streamStore, Func<Owned<TContext>> contextFactory, long? position)
        {
            _subscriptions.Add(
                streamStore.SubscribeToAll(
                    name: RunnerName,
                    continueAfterPosition: position,
                    streamMessageReceived: async (subscription, message, cancellationToken) =>
                    {
                        _logger.LogTrace(
                            "[{Latency}] [POS {Position}] [{StreamId}] [{Type}]",
                            DateTime.UtcNow - message.CreatedUtc, // This is not very precise since we could have differing clocks, and should be seen as merely informational
                            message.Position,
                            message.StreamId,
                            message.Type);

                        if (_envelopeFactory.TryCreate(message, out var envelope))
                        {
                            using (var context = contextFactory().Value)
                            {
                                await context.UpdateProjectionState(RunnerName, message.Position, cancellationToken);

                                await _projector.ProjectAsync(context, envelope, cancellationToken);

                                await context.SaveChangesAsync(cancellationToken);
                            }
                        }
                        else
                        {
                            _logger.LogDebug(
                                "Skipping message {Type} at position {Position} in stream {Stream}@{Version} because it does not appear in the event mapping",
                                message.Type,
                                message.Position,
                                message.StreamId,
                                message.StreamVersion);
                        }
                    },
                    subscriptionDropped: async (subscription, reason, exception) =>
                    {
                        _logger.LogWarning(exception, "Subscription {SubscriptionName} was dropped. Reason: {Reason}", subscription.Name, reason);

                        await Task.Delay(TimeSpan.FromSeconds(5));

                        // When Handle throws, we will no longer be subscribed (and no subsequent subscribe will be attempted).
                        Handle(streamStore, contextFactory);
                    }));
        }

        // async handling with catchup

        public async Task StartAsync(IReadonlyStreamStore streamStore, Func<Owned<TContext>> contextFactory, CancellationToken cancellationToken = default)
        {
            cancellationToken.Register(CleanUp);

            await StartAsyncInternal(streamStore, contextFactory, cancellationToken);
        }

        private async Task StartAsyncInternal(
            IReadonlyStreamStore streamStore,
            Func<Owned<TContext>> contextFactory,
            CancellationToken cancellationToken)
        {
            CleanUp();

            // discover runner position
            long? position;
            using (var context = contextFactory().Value)
            {
                var dbPosition = await context
                    .ProjectionStates
                    .SingleOrDefaultAsync(p => p.Name == RunnerName, cancellationToken);

                position = dbPosition?.Position;
            }

            // discover head position
            var head = await streamStore.ReadHeadPosition(cancellationToken);

            // determine whether to play catch up or start subscribing
            var shouldCatchUp = position.HasValue
                ? head - position.Value > CatchupThreshold
                : head + 1 > CatchupThreshold; // head+1 because first position is 0

            if (shouldCatchUp)
            {
                _tasks.Add(CatchUpAsync(streamStore, contextFactory, position, cancellationToken));
            }
            else
            {
                _subscriptions.Add(
                    streamStore.SubscribeToAll(
                        name: RunnerName,
                        continueAfterPosition: position,
                        streamMessageReceived: async (subscription, message, ct) =>
                        {
                            _logger.LogTrace(
                                "[{Latency}] [POS {Position}] [{StreamId}] [{Type}]",
                                DateTime.UtcNow - message.CreatedUtc, // This is not very precise since we could have differing clocks, and should be seen as merely informational
                                message.Position,
                                message.StreamId,
                                message.Type);

                            if (_envelopeFactory.TryCreate(message, out var envelope))
                            {
                                using (var context = contextFactory().Value)
                                {
                                    await context.UpdateProjectionState(RunnerName, message.Position, cancellationToken);

                                    await _projector.ProjectAsync(context, envelope, cancellationToken);

                                    await context.SaveChangesAsync(cancellationToken);
                                }
                            }
                            else
                            {
                                _logger.LogDebug(
                                    "Skipping message {Type} at position {Position} in stream {Stream}@{Version} because it does not appear in the event mapping",
                                    message.Type,
                                    message.Position,
                                    message.StreamId,
                                    message.StreamVersion);
                            }
                        },
                        subscriptionDropped: async (subscription, reason, exception) =>
                        {
                            _logger.LogWarning(exception, "Subscription {SubscriptionName} was dropped. Reason: {Reason}", subscription.Name, reason);

                            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                            await StartAsyncInternal(streamStore, contextFactory, cancellationToken);
                        }));
            }
        }

        private async Task CatchUpAsync(IReadonlyStreamStore streamStore, Func<Owned<TContext>> contextFactory, long? position, CancellationToken cancellationToken)
        {
            long? positionOfLastMessageOnPage = null;
            try
            {
                _logger.LogDebug(
                    "CatchupThreshold ({CatchupThreshold}) exceeded, catching up with paging (CatchupPageSize: {CatchupPageSize})",
                    CatchupThreshold,
                    CatchupPageSize);

                var fromPositionInclusive = position ?? global::SqlStreamStore.Streams.Position.Start;

                var page = await streamStore.ReadAllForwards(fromPositionInclusive, CatchupPageSize, true, cancellationToken);

                _logger.LogDebug(
                    "Processing page of {PageSize} starting at POS {FromPosition}",
                    page.Messages.Length,
                    page.FromPosition);

                using (var context = contextFactory().Value)
                {
                    foreach (var message in page.Messages)
                    {
                        if (message.Position == fromPositionInclusive && message.Position != global::SqlStreamStore.Streams.Position.Start)
                            continue; // because fromPosition should be exclusive

                        positionOfLastMessageOnPage = message.Position;
                        _logger.LogTrace(
                            "[{Latency}] [POS {Position}] [{StreamId}] [{Type}]",
                            DateTime.UtcNow - message.CreatedUtc, // This is not very precise since we could have differing clocks, and should be seen as merely informational
                            message.Position,
                            message.StreamId,
                            message.Type);

                        if (_envelopeFactory.TryCreate(message, out var envelope))
                        {
                            await _projector.ProjectAsync(context, envelope, cancellationToken);
                        }
                        else
                        {
                            _logger.LogDebug(
                                "Skipping message {Type} at position {Position} in stream {Stream}@{Version} because it does not appear in the event mapping",
                                message.Type,
                                message.Position,
                                message.StreamId,
                                message.StreamVersion);
                        }
                    }

                    if (positionOfLastMessageOnPage.HasValue)
                        await context.UpdateProjectionState(RunnerName, positionOfLastMessageOnPage.Value, cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);
                }

                while (!page.IsEnd)
                {
                    page = await page.ReadNext(cancellationToken);
                    _logger.LogDebug(
                        "Processing page of {PageSize} starting at POS {FromPosition}",
                        page.Messages.Length,
                        page.FromPosition);

                    using (var context = contextFactory().Value)
                    {
                        positionOfLastMessageOnPage = null;
                        foreach (var message in page.Messages)
                        {
                            positionOfLastMessageOnPage = message.Position;
                            _logger.LogTrace(
                                "[{Latency}] [POS {Position}] [{StreamId}] [{Type}]",
                                DateTime.UtcNow - message.CreatedUtc, // This is not very precise since we could have differing clocks, and should be seen as merely informational
                                message.Position,
                                message.StreamId,
                                message.Type);

                            if (_envelopeFactory.TryCreate(message, out var envelope))
                            {
                                await _projector.ProjectAsync(context, envelope, cancellationToken);
                            }
                            else
                            {
                                _logger.LogDebug(
                                    "Skipping message {Type} at position {Position} in stream {Stream}@{Version} because it does not appear in the event mapping",
                                    message.Type,
                                    message.Position,
                                    message.StreamId,
                                    message.StreamVersion);
                            }
                        }

                        if (positionOfLastMessageOnPage.HasValue)
                            await context.UpdateProjectionState(RunnerName, positionOfLastMessageOnPage.Value, cancellationToken);

                        await context.SaveChangesAsync(cancellationToken);
                    }
                }

                await StartAsyncInternal(streamStore, contextFactory, cancellationToken); // subscribe when done catching up
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "{RunnerName} catching up failed because an exception was thrown when handling the message at {Position}.",
                    RunnerName,
                    positionOfLastMessageOnPage ?? -1L);

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                await StartAsyncInternal(streamStore, contextFactory, cancellationToken);
            }
        }

        public void Dispose() => CleanUp();

        private void CleanUp()
        {
            while (_tasks.TryTake(out var task))
            {
                if (task.IsCanceled || task.IsCompleted || task.IsFaulted)
                    task.Dispose();
            }

            while (_subscriptions.TryTake(out var subscription))
                subscription.Dispose();
        }
    }
}
