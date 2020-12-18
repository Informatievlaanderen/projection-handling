namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.MigrationExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;

    /// <summary>
    /// commit which broke the implemention in net 5.0: https://github.com/dotnet/efcore/commit/3eec55ecf91b0eb3898e8670f297869f92030c55?branch=3eec55ecf91b0eb3898e8670f297869f92030c55&diff=split
    /// </summary>
    public class ExtendedSqlServerMigrationsAnnotationProvider : SqlServerAnnotationProvider
    {
        public ExtendedSqlServerMigrationsAnnotationProvider(RelationalAnnotationProviderDependencies dependencies) : base(dependencies)
        { }

        public override IEnumerable<IAnnotation> For(ITableIndex index)
        {
            var baseAnnotations = base.For(index);
            var customAnnotations = index.GetAnnotations().Where(a => a.Name == IndexExtensions.ColumnStoreIndexAnnotationName);

            return baseAnnotations.Concat(customAnnotations);
        }
    }
}
