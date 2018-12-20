namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Testing
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public abstract class SyndicationProjectionTest<TContext, TMessage, TContent, TModule>
        where TModule : AtomEntryProjectionHandlerModule<TMessage, TContent, TContext>, new()
        where TContext : DbContext
        where TMessage : struct
    {

        public AtomEntryProjectionHandlerModuleScenario<TMessage, TContent, TContext> When
        {
            get
            {
                var module = new TModule();
                var resolver = Resolve.WhenEqualToEvent(module.ProjectionHandlers);
                return new AtomEntryProjectionHandlerModuleScenario<TMessage, TContent, TContext>(resolver, CreateContext);
            }
        }

        public AtomEntryProjectionHandlerModuleScenario<TMessage, TContent, TContext> Given(params AtomEntry[] entries) => When.Given(entries);

        public TContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return CreateContext(options);
        }

        protected abstract TContext CreateContext(DbContextOptions<TContext> options);
    }
}
