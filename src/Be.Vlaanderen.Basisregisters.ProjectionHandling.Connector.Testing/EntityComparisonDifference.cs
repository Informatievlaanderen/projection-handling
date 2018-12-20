namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing
{
    public class EntityComparisonDifference<TEntity>
    {
        public TEntity Expected { get; }
        public TEntity Actual { get; }
        public string Message { get; }

        public EntityComparisonDifference(
            TEntity expected,
            TEntity actual,
            string message)
        {
            Expected = expected;
            Actual = actual;
            Message = message;
        }
    }
}
