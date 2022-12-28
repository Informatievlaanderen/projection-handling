namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Microsoft.MigrationExtensions
{
    using global::Microsoft.EntityFrameworkCore.Metadata;
    using global::Microsoft.EntityFrameworkCore.Migrations;
    using global::Microsoft.EntityFrameworkCore.Migrations.Operations;

    public class ExtendedSqlServerMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public ExtendedSqlServerMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, IRelationalAnnotationProvider migrationsAnnotations) :
            base(dependencies, migrationsAnnotations)
        { }

        protected override void IndexTraits(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            base.IndexTraits(operation, model, builder);

            var includeIndexAnnotation = operation.FindAnnotation(IndexExtensions.ColumnStoreIndexAnnotationName);
            if (includeIndexAnnotation != null)
            {
                builder.Append("COLUMNSTORE ");
            }
        }
    }
}
