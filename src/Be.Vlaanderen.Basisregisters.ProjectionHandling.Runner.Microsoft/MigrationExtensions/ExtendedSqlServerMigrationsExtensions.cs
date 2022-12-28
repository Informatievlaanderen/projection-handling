namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Microsoft.MigrationExtensions
{
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.EntityFrameworkCore.Metadata;
    using global::Microsoft.EntityFrameworkCore.Migrations;

    public static class ExtendedSqlServerMigrationsExtensions
    {
        public static DbContextOptionsBuilder UseExtendedSqlServerMigrations(this DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .ReplaceService<IMigrationsSqlGenerator, ExtendedSqlServerMigrationsSqlGenerator>()
                .ReplaceService<IRelationalAnnotationProvider, ExtendedSqlServerMigrationsAnnotationProvider>();

        public static DbContextOptionsBuilder<T> UseExtendedSqlServerMigrations<T>(this DbContextOptionsBuilder<T> optionsBuilder) where T : DbContext =>
            optionsBuilder
                .ReplaceService<IMigrationsSqlGenerator, ExtendedSqlServerMigrationsSqlGenerator>()
                .ReplaceService<IRelationalAnnotationProvider, ExtendedSqlServerMigrationsAnnotationProvider>();
    }
}
