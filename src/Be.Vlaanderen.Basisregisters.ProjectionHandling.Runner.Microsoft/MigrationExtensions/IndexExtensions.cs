namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Microsoft.MigrationExtensions
{
    using System.Text;
    using global::Microsoft.EntityFrameworkCore;
    using global::Microsoft.EntityFrameworkCore.Metadata.Builders;

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
