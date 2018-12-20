namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class AtomEntryProjectionHandlerModule<TMessage, TContent, TConnection>
        where TMessage : struct
    {
        private readonly List<AtomEntryProjectionHandler<TMessage, TConnection>> _handlers;

        protected AtomEntryProjectionHandlerModule() => _handlers = new List<AtomEntryProjectionHandler<TMessage, TConnection>>();

        public void When(TMessage message, Func<AtomEntry<TContent>, TConnection, CancellationToken, Task> handler)
            => _handlers.Add(
                    new AtomEntryProjectionHandler<TMessage, TConnection>(
                        message,
                        (entry, connection, token) => handler(new AtomEntry<TContent>(entry), connection, token)));

        public AtomEntryProjectionHandler<TMessage, TConnection>[] ProjectionHandlers => _handlers.ToArray();
    }
}
