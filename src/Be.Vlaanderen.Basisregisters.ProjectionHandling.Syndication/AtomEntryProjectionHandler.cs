namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public delegate AtomEntryProjectionHandler<TMessage, TConnection> AtomEntryProjectionHandlerResolver<TMessage, TConnection>(AtomEntry entry)
        where TMessage : struct;

    public class AtomEntryProjectionHandler<TMessage, TConnection> where TMessage : struct
    {
        public TMessage Message { get; }
        public Func<AtomEntry, TConnection, CancellationToken, Task> Handler { get; }

        public AtomEntryProjectionHandler(TMessage message, Func<AtomEntry, TConnection, CancellationToken, Task> handler)
        {
            Message = message;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
    }
}
