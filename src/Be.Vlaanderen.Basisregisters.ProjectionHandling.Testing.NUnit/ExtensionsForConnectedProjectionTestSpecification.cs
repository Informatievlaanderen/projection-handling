namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Connector;
    using Connector.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SqlStreamStore;
    using Task = System.Threading.Tasks.Task;

#if NUNIT
    public static class NUnitExtensionsForConnectedProjectionTestSpecification
#elif XUNIT
    public static class XunitExtensionsForConnectedProjectionTestSpecification
#endif
    {
        public static async Task Assert<TContext>(
            this ConnectedProjectionTestSpecification<TContext> specification,
            Func<TContext> contextFactory,
            ILogger logger)
            where TContext : DbContext
        {
            var projector = new ConnectedProjector<TContext>(specification.Resolver);
            using (var ctx = contextFactory())
            {
                ctx.Database.EnsureCreated();

                logger.LogTrace($"Given: {Environment.NewLine}{specification.Messages.ToLogStringLimited(max: int.MaxValue)}");

                var position = 0L;

                foreach (var message in specification.Messages.Select(e => new Envelope(
                    e,
                    new ConcurrentDictionary<string, object>(
                        new List<KeyValuePair<string,object>>
                        {
                            new KeyValuePair<string, object>(Envelope.PositionMetadataKey, position++)
                        })
                    ).ToGenericEnvelope()))
                {
                    await projector.ProjectAsync(ctx, message);

                    await ctx.SaveChangesAsync();
                }

                var result = await specification.Verification(ctx, CancellationToken.None);

                if (result.Failed)
#if NUNIT
                    throw new NUnit.Framework.AssertionException($"The verfication failed because: {result.Message}");
#elif XUNIT
                    throw new Xunit.Sdk.XunitException($"The verfication failed because: {result.Message}");
#endif

                logger.LogTrace(result.Message);
            }
        }
    }
}
