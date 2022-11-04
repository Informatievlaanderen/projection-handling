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

    public interface IFeedProjectionRunner<TContext>
        where TContext : RunnerDbContext<TContext>
    {
        Task CatchUpAsync(Func<Owned<TContext>> context, CancellationToken cancellationToken);
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
            Func<Owned<TContext>> context,
            CancellationToken cancellationToken)
        {
            // Discover last projected position
            long? position;
            await using (var ctx = context().Value)
            {
                var dbPosition = await ctx
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

                    await using (var ctx = context().Value)
                    {
                        await ProjectAtomEntriesAsync(entries, ctx, cancellationToken);

                        await ctx.UpdateProjectionState(RunnerName, lastEntryId, cancellationToken);
                        await ctx.SaveChangesAsync(cancellationToken);
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
                _logger.LogInformation($"[{DateTime.Now}] [{entry.Id}] [{entry.Title}]");

                try
                {
                    using var contentXmlReader = XmlReader.Create(new StringReader(entry.Description), new XmlReaderSettings {Async = true});
                    var content = _dataContractSerializer.ReadObject(contentXmlReader);
                    if (content != null)
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
