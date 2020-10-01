namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Tests.FeedProjector
{
    using Runner;

    public class FeedProjectorTestContext : RunnerDbContext<FeedProjectorTestContext>
    {
        public override string ProjectionStateSchema => "FakeSchema";
    }
}
