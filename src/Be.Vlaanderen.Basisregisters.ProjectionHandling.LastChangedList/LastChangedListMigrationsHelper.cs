namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Runner;

    public class LastChangedListMigrationsHelper : RunnerDbContextMigrationHelper<LastChangedListContext>
    {
        public LastChangedListMigrationsHelper(string connectionString, ILoggerFactory loggerFactory)
            : base(
                connectionString,
                new HistoryConfiguration
                {
                    Schema = LastChangedListContext.Schema,
                    Table = LastChangedListContext.MigrationsHistoryTable
                },
                loggerFactory)
        { }

        protected override LastChangedListContext CreateContext(DbContextOptions<LastChangedListContext> migrationContextOptions)
        {
            return new LastChangedListContext(migrationContextOptions);
        }
    }
}
