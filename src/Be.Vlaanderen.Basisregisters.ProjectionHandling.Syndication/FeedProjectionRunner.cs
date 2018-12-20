namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Autofac.Features.OwnedInstances;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.SyndicationFeed;
    using Runner;

    //todo: move to generic libray
    public class FeedProjectionRunner<TMessage, TContent, TContext>
        where TMessage : struct
        where TContext : RunnerDbContext<TContext>
    {
        private readonly ILogger _logger;
        private readonly IRegistryAtomFeedReader _atomFeedReader;
        private readonly DataContractSerializer _dataContractSerializer;
        private readonly AtomEntryProjectionHandlerResolver<TMessage, TContext> _atomEntryProjectionHandlerResolver;

        // ReSharper disable StaticMemberInGenericType
        public static string RunnerName { get; private set; }
        public static Uri FeedUri { get; private set; }

        public FeedProjectionRunner(
            string runnerName,
            Uri feedUri,
            ILogger logger,
            IRegistryAtomFeedReader atomFeedReader,
            params AtomEntryProjectionHandlerModule<TMessage, TContent, TContext>[] projectionHandlerModules)
        {
            RunnerName = runnerName;
            FeedUri = feedUri;

            _logger = logger;
            _atomFeedReader = atomFeedReader;
            _dataContractSerializer = new DataContractSerializer(typeof(TContent));
            _atomEntryProjectionHandlerResolver = Resolve.WhenEqualToEvent(projectionHandlerModules.SelectMany(t => t.ProjectionHandlers).ToArray());
        }

        public async Task CatchUpAsync(
            Func<Owned<TContext>> contextFactory,
            CancellationToken cancellationToken = default)
        {
            // Discover last projected position
            long? position;
            using (var context = contextFactory().Value)
            {
                var dbPosition = await context
                    .ProjectionStates
                    .SingleOrDefaultAsync(p => p.Name == RunnerName, cancellationToken);

                position = dbPosition != null ? (long?)dbPosition.Position++ : null;
            }

            // Read new events
            var entries = (await _atomFeedReader.ReadEntriesAsync(FeedUri, position)).ToList();

            while (entries.Any())
            {
                if (!long.TryParse(entries.Last().Id, out var lastEntryId))
                    break;

                using (var context = contextFactory().Value)
                {
                    await ProjectAtomEntriesAsync(entries, context, cancellationToken);

                    await context.UpdateProjectionState(RunnerName, lastEntryId, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }

                position = lastEntryId + 1;
                entries = (await _atomFeedReader.ReadEntriesAsync(FeedUri, position)).ToList();
            }
        }

        private async Task ProjectAtomEntriesAsync(
            IEnumerable<IAtomEntry> entries,
            TContext context,
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in entries)
            {
                _logger.LogInformation($"[{DateTime.Now}] [{entry.Id}] [{entry.Title}]");

                try
                {
                    using (var contentXmlReader = XmlReader.Create(new StringReader(entry.Description), new XmlReaderSettings { Async = true }))
                    {
                        var atomEntry = new AtomEntry(entry, _dataContractSerializer.ReadObject(contentXmlReader));

                        await _atomEntryProjectionHandlerResolver(atomEntry)
                            .Handler
                            .Invoke(atomEntry, context, cancellationToken);
                    }
                }
                catch (Exception e) when (e is InvalidOperationException || e is ApplicationException)
                {
                    _logger.LogWarning(e.Message, e);
                }
            }
        }
    }
}
