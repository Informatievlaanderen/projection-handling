namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Microsoft
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using DependencyInjection.OwnedInstances;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.Extensions.Logging;
    using global::Microsoft.SyndicationFeed;
    using Runner.Microsoft;

    public interface IFeedProjectionRunner<TContext>
        where TContext : RunnerDbContext<TContext>
    {
        Task CatchUpAsync(Func<Owned<TContext>> contextFactory, CancellationToken cancellationToken);
    }

    public class FeedProjectionRunner<TMessage, TContent, TContext> : IFeedProjectionRunner<TContext>
        where TMessage : struct
        where TContext : RunnerDbContext<TContext>
    {
        private readonly int _pollingInMilliseconds;
        private readonly ILogger _logger;
        private readonly IRegistryAtomFeedReader _atomFeedReader;
        private readonly DataContractSerializer _dataContractSerializer;
        private readonly AtomEntryProjectionHandlerResolver<TMessage, TContext> _atomEntryProjectionHandlerResolver;
                
        public string RunnerName { get; }
        public Uri FeedUri { get; }
        public string FeedUserName { get; }
        public string FeedPassword { get; }
        public bool EmbedEvent { get; }
        public bool EmbedObject { get; }

        public FeedProjectionRunner(
            string runnerName,
            Uri feedUri,
            string feedUserName,
            string feedPassword,
            int pollingInMilliseconds,
            bool embedEvent,
            bool embedObject,
            ILogger logger,
            IRegistryAtomFeedReader atomFeedReader,
            params AtomEntryProjectionHandlerModule<TMessage, TContent, TContext>[] projectionHandlerModules)
        {
            RunnerName = runnerName;
            FeedUri = feedUri;
            FeedUserName = feedUserName;
            FeedPassword = feedPassword;
            EmbedEvent = embedEvent;
            EmbedObject = embedObject;

            _pollingInMilliseconds = pollingInMilliseconds;
            _logger = logger;
            _atomFeedReader = atomFeedReader;
            _dataContractSerializer = new DataContractSerializer(typeof(TContent));
            _atomEntryProjectionHandlerResolver = Resolve.WhenEqualToEvent(projectionHandlerModules.SelectMany(t => t.ProjectionHandlers).ToArray());
        }

        public async Task CatchUpAsync(
            Func<Owned<TContext>> contextFactory,
            CancellationToken cancellationToken)
        {
            // Discover last projected position
            long? position;
            await using (var context = contextFactory().Value)
            {
                var dbPosition = await context
                    .ProjectionStates
                    .SingleOrDefaultAsync(p => p.Name == RunnerName, cancellationToken);

                position = dbPosition?.Position + 1;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                // Read new events
                var entries = (await _atomFeedReader.ReadEntriesAsync(FeedUri, position, FeedUserName, FeedPassword, EmbedEvent, EmbedObject)).ToList();

                while (entries.Any())
                {
                    if (!long.TryParse(entries.Last().Id, out var lastEntryId))
                    {
                        break;
                    }

                    await using (var context = contextFactory().Value)
                    {
                        await ProjectAtomEntriesAsync(entries, context, cancellationToken);

                        await context.UpdateProjectionState(RunnerName, lastEntryId, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    position = lastEntryId + 1;
                    entries = (await _atomFeedReader.ReadEntriesAsync(FeedUri, position, FeedUserName, FeedPassword, EmbedEvent, EmbedObject)).ToList();
                }

                Thread.Sleep(_pollingInMilliseconds);
            }
        }

        private async Task ProjectAtomEntriesAsync(
            IEnumerable<IAtomEntry> entries,
            TContext context,
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in entries)
            {
                var logMessage = $"[{DateTime.Now}] [{entry.Id}] [{entry.Title}]";
                _logger.LogInformation(logMessage);

                try
                {
                    using var contentXmlReader = XmlReader.Create(new StringReader(entry.Description), new XmlReaderSettings {Async = true});
                    var content = _dataContractSerializer.ReadObject(contentXmlReader);
                    if (content is not null)
                    {
                        var atomEntry = new AtomEntry(entry, content);

                        foreach (var resolvedProjectionHandler in _atomEntryProjectionHandlerResolver(atomEntry))
                        {
                            await resolvedProjectionHandler
                                .Handler
                                .Invoke(atomEntry, context, cancellationToken);
                        }
                    }
                }
                catch (AtomResolveHandlerException e)
                {
                    _logger.LogWarning(e, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    throw;
                }
            }
        }
    }
}
