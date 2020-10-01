namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Tests.FeedProjector
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;

    public class InfiniteRunner : IFeedProjectionRunner<FeedProjectorTestContext>
    {
        public async Task CatchUpAsync(Func<Owned<FeedProjectorTestContext>> context, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            }
        }
    }
}
