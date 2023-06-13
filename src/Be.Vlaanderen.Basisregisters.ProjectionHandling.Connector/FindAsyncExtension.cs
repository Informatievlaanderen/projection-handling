namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public static class FindAsyncExtension
    {
        public static ValueTask<TEntity?> FindAsync<TEntity, T>(
            this DbSet<TEntity> dbSet,
            T keyValue,
            CancellationToken cancellationToken)
            where TEntity : class => dbSet.FindAsync(new object[] { keyValue }, cancellationToken);
    }
}
