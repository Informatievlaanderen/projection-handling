namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using Microsoft.EntityFrameworkCore;
    using Runner;

    public class LastChangedListContextMigrationFactory : RunnerDbContextMigrationFactory<LastChangedListContext> {

        public LastChangedListContextMigrationFactory(string connectionStringName)
            : base(connectionStringName, HistoryConfiguration) { }

        private static MigrationHistoryConfiguration HistoryConfiguration =>
            new MigrationHistoryConfiguration
            {
                Schema = LastChangedListContext.Schema,
                Table = LastChangedListContext.MigrationsHistoryTable
            };

        protected override LastChangedListContext CreateContext(DbContextOptions<LastChangedListContext> migrationContextOptions)
            => new LastChangedListContext(migrationContextOptions);
    }
}
