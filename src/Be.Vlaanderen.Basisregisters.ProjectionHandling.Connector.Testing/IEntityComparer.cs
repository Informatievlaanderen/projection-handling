namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing
{
    using System.Collections.Generic;

    public interface IEntityComparer<TEntity>
    {
        IEnumerable<EntityComparisonDifference<TEntity>> Compare(TEntity expected, TEntity actual);
    }
}
