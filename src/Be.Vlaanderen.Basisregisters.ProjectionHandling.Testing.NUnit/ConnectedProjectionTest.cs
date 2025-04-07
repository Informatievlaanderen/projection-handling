namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Connector;
    using Connector.Testing;
    using Microsoft.EntityFrameworkCore;
    using SqlStreamStore;

    /// <summary>
    /// Test a ConnectedProjection
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TProjection"></typeparam>
    public class ConnectedProjectionTest<TContext, TProjection>
        where TProjection : ConnectedProjection<TContext> where TContext : DbContext
    {
        private readonly Func<TContext> _createContextFactory;
        private readonly Func<TProjection> _projectionFactory;
        private ConnectedProjectionScenario<TContext>? _context;

        /// <summary>
        /// Setting up the projection scenario with resolver
        /// </summary>
        public ConnectedProjectionScenario<TContext> When
        {
            get
            {
                var projection = _projectionFactory();
                var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);
                return new ConnectedProjectionScenario<TContext>(resolver);
            }
        }

        /// <summary>
        /// ctor of the ConnectedProjectionTest
        /// </summary>
        /// <param name="createContextFactory">factory method on how to create the context</param>
        /// <param name="projectionFactory">factory method on how to create the projection</param>
        public ConnectedProjectionTest(Func<TContext> createContextFactory, Func<TProjection> projectionFactory)
        {
            _createContextFactory = createContextFactory;
            _projectionFactory = projectionFactory;
        }

        /// <summary>
        /// Given a set of events
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public ConnectedProjectionTest<TContext, TProjection> Given(params object[] events)
        {
            _context = When.Given(events.Select(e =>
            {
                if (e is Envelope || (e.GetType().IsGenericType && e.GetType().GetGenericTypeDefinition() == typeof(Envelope<>)))
                {
                    return e;
                }
                return new Envelope(e, new ConcurrentDictionary<string, object>()).ToGenericEnvelope();
            }));

            return this;
        }

        /// <summary>
        /// Then assertions are verified
        /// </summary>
        /// <param name="assertions"></param>
        /// <returns></returns>
        public async Task Then(Func<TContext, Task> assertions)
        {
            var test = CreateTest(assertions);

            using (var dbContext = _createContextFactory())
            {
                dbContext.Database.EnsureCreated();

                foreach (var message in test.Messages)
                {
                    await new ConnectedProjector<TContext>(test.Resolver)
                        .ProjectAsync(dbContext, message);

                    await dbContext.SaveChangesAsync();
                }

                var result = await test.Verification(dbContext, CancellationToken.None);
                if (result.Failed)
#if NUNIT
                    throw new NUnit.Framework.AssertionException($"The verfication failed because: {result.Message}");
#elif XUNIT
                    throw new Xunit.Sdk.XunitException($"The verfication failed because: {result.Message}");
#endif
            }
        }

        private ConnectedProjectionTestSpecification<TContext> CreateTest(Func<TContext, Task> assertions)
        {
            return _context!.Verify(async context =>
            {
                try
                {
                    await assertions(context);
                    return VerificationResult.Pass();
                }
                catch (Exception e)
                {
                    return VerificationResult.Fail(e.Message);
                }
            });
        }
    }
}
