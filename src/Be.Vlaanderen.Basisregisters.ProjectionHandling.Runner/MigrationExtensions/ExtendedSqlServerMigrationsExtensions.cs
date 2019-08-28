namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.MigrationExtensions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Migrations;

    public static class ExtendedSqlServerMigrationsExtensions
    {
        public static DbContextOptionsBuilder UseExtendedSqlServerMigrations(this DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .ReplaceService<IMigrationsSqlGenerator, ExtendedSqlServerMigrationsSqlGenerator>()
                .ReplaceService<IMigrationsAnnotationProvider, ExtendedSqlServerMigrationsAnnotationProvider>();

        public static DbContextOptionsBuilder<T> UseExtendedSqlServerMigrations<T>(this DbContextOptionsBuilder<T> optionsBuilder) where T : DbContext =>
            optionsBuilder
                .ReplaceService<IMigrationsSqlGenerator, ExtendedSqlServerMigrationsSqlGenerator>()
                .ReplaceService<IMigrationsAnnotationProvider, ExtendedSqlServerMigrationsAnnotationProvider>();
    }
}
