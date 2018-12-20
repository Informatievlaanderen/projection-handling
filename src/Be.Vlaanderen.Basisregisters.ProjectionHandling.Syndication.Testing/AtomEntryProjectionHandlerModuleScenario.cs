namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Testing
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions.Execution;
    using Microsoft.EntityFrameworkCore;

    public class AtomEntryProjectionHandlerModuleScenario<TMessage, TContent, TContext>
        where TMessage : struct
        where TContext : DbContext
    {
        private readonly AtomEntryProjectionHandlerResolver<TMessage, TContext> _resolver;
        private readonly AtomEntry[] _entries;
        private readonly Func<TContext> _contextFactory;

        public AtomEntryProjectionHandlerModuleScenario(
            AtomEntryProjectionHandlerResolver<TMessage, TContext> resolver,
            Func<TContext> contextFactory)
        {
            _resolver = resolver;
            _contextFactory = contextFactory;
            _entries = new AtomEntry[0];
        }

        private AtomEntryProjectionHandlerModuleScenario(
            AtomEntryProjectionHandlerResolver<TMessage, TContext> resolver,
            AtomEntry[] entries,
            Func<TContext> contextFactory)
        {
            _resolver = resolver;
            _entries = entries;
            _contextFactory = contextFactory;
        }

        public AtomEntryProjectionHandlerModuleScenario<TMessage, TContent, TContext> Given(params AtomEntry[] entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            return new AtomEntryProjectionHandlerModuleScenario<TMessage, TContent, TContext>(
                _resolver,
                _entries.Concat(entries).ToArray(),
                _contextFactory);
        }

        public AtomEntryProjectionHandlerModuleScenario<TMessage, TContent, TContext> Project(params AtomEntry[] entries) => Given(entries);

        public async Task Then(Func<TContext, Task> assertions)
        {
            using (var context = _contextFactory())
            {
                await context.Database.EnsureCreatedAsync();

                foreach (var entry in _entries)
                {
                    try
                    {
                        await _resolver(entry).Handler.Invoke(entry, context, CancellationToken.None);
                    }
                    catch (InvalidOperationException)
                    { }
                }

                await context.SaveChangesAsync();

                try
                {
                    await assertions(context);
                }
                catch (Exception e)
                {
                    throw new AssertionFailedException(e.Message);
                }
            }
        }
    }
}
