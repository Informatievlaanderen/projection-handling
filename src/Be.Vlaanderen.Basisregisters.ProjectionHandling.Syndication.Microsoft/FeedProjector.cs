namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Microsoft
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DependencyInjection.OwnedInstances;
    using global::Microsoft.Extensions.Logging;
    using Runner.Microsoft;

    public class FeedProjector<TContext>
        where TContext: RunnerDbContext<TContext>
    {
        private readonly Func<Owned<TContext>> _contextFactory;
        private readonly List<IFeedProjectionRunner<TContext>> _feedProjectionRunners;
        private readonly ILogger _logger;

        public FeedProjector(
            Func<Owned<TContext>> contextFactory,
            ILoggerFactory loggerFactory,
            IEnumerable<IFeedProjectionRunner<TContext>> configuredFeedProjectionRunners)
        {
            _contextFactory = contextFactory;
            _feedProjectionRunners = configuredFeedProjectionRunners?.ToList() ?? new List<IFeedProjectionRunner<TContext>>();
            _logger = loggerFactory?.CreateLogger<FeedProjector<TContext>>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public FeedProjector<TContext> Register(params IFeedProjectionRunner<TContext>[] feedProjectionRunners)
            => Register((IEnumerable<IFeedProjectionRunner<TContext>>)feedProjectionRunners);

        public FeedProjector<TContext> Register(IEnumerable<IFeedProjectionRunner<TContext>> feedProjectionRunners)
        {
            if (feedProjectionRunners != null)
                _feedProjectionRunners.AddRange(feedProjectionRunners);

            return this;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            var catchupRunners = _feedProjectionRunners.Select(runner => CreateCatchUpTask(runner, cancellationToken));
            await Task.WhenAll(catchupRunners);
        }

        private async Task CreateCatchUpTask(IFeedProjectionRunner<TContext> feed, CancellationToken cancellationToken)
        {
            try
            {
                await feed.CatchUpAsync(_contextFactory, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "FeedProjectionRunner failed");
            }
        }
    }
}
