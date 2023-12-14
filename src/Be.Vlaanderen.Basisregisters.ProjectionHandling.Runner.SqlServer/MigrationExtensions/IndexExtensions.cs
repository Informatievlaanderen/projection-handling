namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.SqlServer.MigrationExtensions
{
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class IndexExtensions
    {
        public static readonly string ColumnStoreIndexAnnotationName = "SqlServer:ColumnStoreIndex";

        public static IndexBuilder IsColumnStore(this IndexBuilder indexBuilder, string name)
        {
            var includeStatement = new StringBuilder();

            indexBuilder.HasAnnotation(ColumnStoreIndexAnnotationName, includeStatement.ToString());
            indexBuilder.HasName(name);

            return indexBuilder;
        }
    }
}
