namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.SqlServer.MigrationExtensions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;

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
