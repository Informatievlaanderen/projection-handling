namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.MigrationExtensions
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public static class MigratorExtensions
    {
        public static void SetDefaultMigratorCommandTimeout(this DbContext context)
        {
            context.Database.SetCommandTimeout(new TimeSpan(1, 0, 0, 0));
        }
    }
}
