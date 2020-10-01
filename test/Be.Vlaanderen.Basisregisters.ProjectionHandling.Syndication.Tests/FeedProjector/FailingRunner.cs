namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Tests.FeedProjector
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;

    public class FailingRunner : IFeedProjectionRunner<FeedProjectorTestContext>
    {
        private readonly Exception _exception;
        private readonly TimeSpan _delay;

        public FailingRunner(Exception exception)
            : this(exception, TimeSpan.Zero)
        { }

        public FailingRunner(Exception exception, TimeSpan delay)
        {
            _exception = exception;
            _delay = delay;
        }

        public async Task CatchUpAsync(Func<Owned<FeedProjectorTestContext>> context, CancellationToken cancellationToken)
        {
            await Task.Delay(_delay, cancellationToken);
            throw _exception;
        }
    }
}
