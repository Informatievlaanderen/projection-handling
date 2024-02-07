namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public static class ConnectedProjectionScenarioExtensions
    {
        public static ConnectedProjectionTestSpecification<TContext> Expect<TContext, TEntity>(
            this ConnectedProjectionScenario<TContext> scenario,
            Func<TContext, IQueryable<TEntity>> query,
            params TEntity[] entities) where TEntity : class =>
            scenario.Expect(new CompareNetObjectsBasedGenericEntityComparer<TEntity>(), query, entities);

        public static ConnectedProjectionTestSpecification<TContext> Expect<TContext, TEntity>(
            this ConnectedProjectionScenario<TContext> scenario,
            IEntityComparer<TEntity> comparer,
            Func<TContext, IQueryable<TEntity>> query,
            params TEntity[] expectedEntities) where TEntity : class
        {
            return scenario.Verify(async ctx =>
            {
                var actualEntities = await query(ctx).ToListAsync().ConfigureAwait(false);
                if (actualEntities.Count != expectedEntities.Length)
                    return VerificationResult.Fail(
                        $"  Expected {expectedEntities.Length} entities, but found {actualEntities.Count} entities. {actualEntities.ToLogStringLimited(max: 10)}");

                var differences = expectedEntities
                    .Zip(actualEntities, (expected,
                        actual) => new
                        {
                            expected,
                            actual
                        })
                    .SelectMany(x => comparer.Compare(x.expected, x.actual))
                    .ToList();

                using (var writer = new StringWriter())
                {
                    writer.WriteLine("  Expected: {0} entities ({1}),",
                        expectedEntities.Length,
                        expectedEntities.ToLogStringLimited(max: 10));

                    if (!differences.Any())
                    {
                        writer.WriteLine("  Actual: {0} entities ({1}),",
                            actualEntities.Count,
                            actualEntities.ToLogStringLimited(max: 10));

                        return VerificationResult.Pass(writer.ToString());
                    }

                    writer.WriteLine("  But found the following differences:");
                    foreach (var difference in differences)
                        writer.WriteLine("    {0}", difference.Message);

                    return VerificationResult.Fail(writer.ToString());
                }
            });
        }

        public static ConnectedProjectionTestSpecification<TContext> ExpectNone<TContext, TEntity>(
            this ConnectedProjectionScenario<TContext> scenario,
            Func<TContext, IQueryable<TEntity>> query) where TEntity : class
        {
            return scenario.Verify(async ctx =>
            {
                var actualEntities = await query(ctx).ToListAsync().ConfigureAwait(false);
                if (actualEntities.Count > 0)
                    return VerificationResult.Fail(
                        $"  Expected none, but found {actualEntities.Count} entities. {actualEntities.ToLogStringLimited(max: 10)}");

                return VerificationResult.Pass($"  Expected: none{Environment.NewLine}  Actual: none");
            });
        }
    }
}
