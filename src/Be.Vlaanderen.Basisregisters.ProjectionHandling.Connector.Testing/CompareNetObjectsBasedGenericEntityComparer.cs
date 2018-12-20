namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing
{
    using System.Collections.Generic;
    using System.Linq;
    using KellermanSoftware.CompareNetObjects;

    public class CompareNetObjectsBasedGenericEntityComparer<TEntity> : IEntityComparer<TEntity>
    {
        private readonly CompareLogic _logic;

        public CompareNetObjectsBasedGenericEntityComparer()
            => _logic = new CompareLogic();

        public IEnumerable<EntityComparisonDifference<TEntity>> Compare(TEntity expected, TEntity actual)
            => _logic
                .Compare(expected, actual)
                .Differences
                .Select(diff => new EntityComparisonDifference<TEntity>(expected, actual, diff.ToString()));
    }
}
