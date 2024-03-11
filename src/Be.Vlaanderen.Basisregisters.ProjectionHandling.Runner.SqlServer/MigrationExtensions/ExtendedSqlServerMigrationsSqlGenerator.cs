namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.SqlServer.MigrationExtensions
{
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Microsoft.EntityFrameworkCore.Migrations.Operations;
    using Microsoft.EntityFrameworkCore.Update;

    public class ExtendedSqlServerMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public ExtendedSqlServerMigrationsSqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            ICommandBatchPreparer commandBatchPreparer) : base(dependencies, commandBatchPreparer)
        { }

        protected override void IndexTraits(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
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
