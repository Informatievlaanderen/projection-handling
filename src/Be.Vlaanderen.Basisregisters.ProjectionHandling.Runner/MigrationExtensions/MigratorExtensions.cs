namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.MigrationExtensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public static class MigratorExtensions
    {
        public static void SetDefaultMigratorCommandTimeout(this DbContext context)
        {
            context.Database.SetCommandTimeout(new TimeSpan(1, 0, 0, 0));
        }

        public static Task MigrateAsync(this DbContext context, CancellationToken cancellationToken)
        {
            context.SetDefaultMigratorCommandTimeout();
            return context.Database.MigrateAsync(cancellationToken);
        }
    }
}
