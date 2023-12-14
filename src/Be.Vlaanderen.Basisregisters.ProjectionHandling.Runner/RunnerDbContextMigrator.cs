namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRunnerDbContextMigrator
    {
        Task MigrateAsync(CancellationToken cancellationToken);
    }
}
