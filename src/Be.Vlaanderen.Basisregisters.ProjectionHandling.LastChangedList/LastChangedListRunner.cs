namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using Connector;
    using Microsoft.Extensions.Logging;
    using Runner;
    using SqlStreamStore;

    public class LastChangedListRunner : Runner<LastChangedListContext>
    {
        public LastChangedListRunner(
            string name,
            ConnectedProjection<LastChangedListContext> projections,
            EnvelopeFactory envelopeFactory,
            ILogger logger)
            : base(name,
                envelopeFactory,
                logger,
                projections.Handlers) { }

        public LastChangedListRunner(
            string name,
            EnvelopeFactory envelopeFactory,
            ILogger logger,
            params ConnectedProjection<LastChangedListContext>[] projections)
            : base(name,
                envelopeFactory,
                logger,
                projections)
        { }
    }
}
