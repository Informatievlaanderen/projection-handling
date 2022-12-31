namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Microsoft
{
    using System;
    using System.Linq;

    public static class Resolve
    {
        public static AtomEntryProjectionHandlerResolver<TMessage, TConnection> WhenEqualToEvent<TMessage, TConnection>(
            AtomEntryProjectionHandler<TMessage, TConnection>[] handlers)
            where TMessage : struct
        {
            if (handlers == null)
                throw new ArgumentNullException(nameof(handlers));

            var cache = handlers
                .GroupBy(key => key.Message)
                .ToDictionary(key => key.Key, x => x.ToList());

            return entry =>
            {
                var title = entry.FeedEntry.Title.Split('-')[0];

                return Enum.TryParse<TMessage>(title, out var @event) && cache.TryGetValue(@event, out var resolvedHandlers)
                    ? resolvedHandlers
                    : throw new AtomResolveHandlerException($"Could not resolve a handler for {title}.");
            };
        }
    }
}
