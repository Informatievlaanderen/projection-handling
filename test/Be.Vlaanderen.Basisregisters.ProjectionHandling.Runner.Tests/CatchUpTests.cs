namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Connector;
    using EventHandling;
    using global::SqlStreamStore;
    using global::SqlStreamStore.Streams;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using SqlStreamStore;
    using Xunit;
    using Xunit.Abstractions;

    [Collection("RunnerTests")]//prevents the tests from running in parallel on the same InMemory DbContext
    public class CatchUpTests : IDisposable
    {
        public CatchUpTests(ITestOutputHelper output) => _logger = new TestLogger<TestRunner>(output);

        public void Dispose()
        {
        }

        private readonly TestLogger<TestRunner> _logger;

        private class MessageCountingProjection : ConnectedProjection<TestDbContext>
        {
            private Func<TestMessage, bool> _runComplete;

            public MessageCountingProjection(Func<TestMessage, bool> runComplete)
            {
                _runComplete = runComplete;
                RunCompleteEvent = new AutoResetEvent(false);
                When<Envelope<TestMessage>>((ctx, m) => Handle(ctx, m));
            }

            public AutoResetEvent RunCompleteEvent { get; private set; }

            public Dictionary<int, int> ReceivedMessagesCounts { get; } = new Dictionary<int, int>();

            private void Handle(TestDbContext ctx, Envelope<TestMessage> message)
            {
                if (ReceivedMessagesCounts.ContainsKey(message.Message.Id))
                    ReceivedMessagesCounts[message.Message.Id]++;
                else
                    ReceivedMessagesCounts[message.Message.Id] = 1;

                if (_runComplete(message.Message))
                {
                    RunCompleteEvent.Set();
                    //RunCompleteEvent.Reset();
                }
            }
        }

        [Fact]
        public void StartsAfterCurrentPositionWhenCatchingUp()
        {
            var eventMapping =
                new EventMapping(EventMapping.DiscoverEventNamesInAssembly(typeof(RunnerTests).Assembly));
            var inMemoryStreamStore = new InMemoryStreamStore(() => DateTime.UtcNow);
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using (var testDbContext = new TestDbContext(options))
            {
                testDbContext.Database.EnsureDeleted();
            }

            async Task<AppendResult> Store(TestMessage message) => await inMemoryStreamStore.AppendToStream(
                new StreamId("Bla"), ExpectedVersion.Any,
                new NewStreamMessage(Guid.NewGuid(), eventMapping.GetEventName(typeof(TestMessage)),
                    JsonConvert.SerializeObject(message)));

            var storedEventsOnFirstRun = 10;
            var storedEventsOnSecondRun = 10;
            var projection = new MessageCountingProjection(m => m.Id == storedEventsOnFirstRun - 1 || m.Id == storedEventsOnFirstRun + storedEventsOnSecondRun -1 );

            //do a first catch-up run
            using (var testRunner =
                new TestRunner(new EnvelopeFactory(eventMapping, new EventDeserializer(JsonConvert.DeserializeObject)),
                    _logger, projection))
            {
                testRunner.CatchupThreshold = storedEventsOnFirstRun - 1;
                testRunner.CatchupPageSize = storedEventsOnFirstRun;

                Task.WaitAll(Enumerable.Range(0, storedEventsOnFirstRun).Select(i => Store(new TestMessage {Id = i}))
                    .Cast<Task>()
                    .ToArray());

                testRunner.StartAsync(inMemoryStreamStore,
                    () => new Owned<TestDbContext>(new TestDbContext(options), this)).Wait();

                projection.RunCompleteEvent.WaitOne();
                _logger.LogInformation("1st run completed");
            }

            //do a second catch-up run and check that the last handled message of the first run is not handled again
            using (var testRunner =
                new TestRunner(new EnvelopeFactory(eventMapping, new EventDeserializer(JsonConvert.DeserializeObject)),
                    _logger, projection))
            {
                testRunner.CatchupThreshold = storedEventsOnSecondRun - 1;
                testRunner.CatchupPageSize = storedEventsOnSecondRun;

                Task.WaitAll(Enumerable.Range(storedEventsOnFirstRun, storedEventsOnSecondRun)
                    .Select(i => Store(new TestMessage {Id = i})).Cast<Task>()
                    .ToArray());

                testRunner.StartAsync(inMemoryStreamStore,
                    () => new Owned<TestDbContext>(new TestDbContext(options), this)).Wait();

                projection.RunCompleteEvent.WaitOne();
                _logger.LogInformation("2nd run completed");
            }

            void MessagecountInspector(int x) => Assert.Equal(1, x);
            Assert.Collection(projection.ReceivedMessagesCounts.Values,
                Enumerable.Range(0, storedEventsOnFirstRun + storedEventsOnSecondRun)
                    .Select(i => (Action<int>) MessagecountInspector)
                    .ToArray());
        }
    }
}
