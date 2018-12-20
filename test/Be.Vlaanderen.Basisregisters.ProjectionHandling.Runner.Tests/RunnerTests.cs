namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Connector;
    using EventHandling;
    using global::SqlStreamStore;
    using global::SqlStreamStore.Streams;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Newtonsoft.Json;
    using Polly;
    using ProjectionStates;
    using SqlStreamStore;
    using Xunit;
    using Xunit.Sdk;

    [Collection("RunnerTests")]//prevents the tests from running in parallel on the same InMemory DbContext
    public class RunnerTests : IDisposable
    {
        public void Dispose()
        {
        }

        [Fact]
        public async Task Testing()
        {
            var projection = new PublicServiceProjections();
            var store = CreateMessageStorerAndStartHandling(projection);

            await store(new TestMessage());

            Policy
                .Handle<AssertActualExpectedException>()
                .WaitAndRetry(5, i => TimeSpan.FromSeconds(2))
                .Execute(() =>
                {
                    Assert.True(projection.HasFailed);
                    Assert.True(projection.Handled);
                });
        }

        [Fact]
        public void StartsAfterCurrentPostionAfterDroppedSubscription()
        {
            var projection = new FailingNonIdempotentProjection(5);
            var store = CreateMessageStorerAndStartHandling(projection);

            var expectedCount = 10;
            Task.WaitAll(Enumerable.Range(0, expectedCount).Select(i => store(new TestMessage { Id = i })).Cast<Task>().ToArray());

            Task.Run(() =>
            {
                while (projection.HandledMessages.Count != 10)
                {
                }
            }).Wait(TimeSpan.FromSeconds(10));

            Assert.Equal(expectedCount, projection.HandledMessages.Count);
            Assert.Equal(expectedCount, projection.HandledMessages.Distinct().Count());
        }

        

        private Func<TestMessage, Task<AppendResult>> CreateMessageStorerAndStartHandling(ConnectedProjection<TestDbContext> projection)
        {
            var eventMapping =
                new EventMapping(EventMapping.DiscoverEventNamesInAssembly(typeof(RunnerTests).Assembly));
            var testRunner =
                new TestRunner(
                    new EnvelopeFactory(
                        eventMapping,
                        new EventDeserializer(JsonConvert.DeserializeObject)),
                    new Logger<TestRunner>(new LoggerFactory()),
                    projection);

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase("testing")
                .Options;
            using (var testDbContext = new TestDbContext(options))
            {
                testDbContext.Database.EnsureDeleted();
            }

            var inMemoryStreamStore = new InMemoryStreamStore(() => DateTime.UtcNow);

            testRunner.Handle(
                inMemoryStreamStore,
                () => new Owned<TestDbContext>(new TestDbContext(options), this));

            return async message => await inMemoryStreamStore.AppendToStream(new StreamId("Bla"), ExpectedVersion.Any,
                new NewStreamMessage(Guid.NewGuid(), eventMapping.GetEventName(typeof(TestMessage)), JsonConvert.SerializeObject(message)));
        }
    }

    [EventName("TestMessage")]
    [EventDescription("TestMessage description")]
    public class TestMessage
    {
        public int Id { get; set; }
    }

    public class TestRunner : Runner<TestDbContext>
    {
        public const string Name = "TestRunner";

        public TestRunner(
            EnvelopeFactory envelopeFactory,
            ILogger<TestRunner> logger,
            ConnectedProjection<TestDbContext> projection) :
            base(
                Name,
                envelopeFactory,
                logger,
                projection)
        {
        }
    }

    public class PublicServiceProjections : ConnectedProjection<TestDbContext>
    {
        public bool HasFailed { get; set; }
        public bool Handled { get; set; }

        public PublicServiceProjections()
        {
            // This handler fails only on the first time it receives a message
            When<Envelope<TestMessage>>((context, message) =>
            {
                if (!HasFailed)
                {
                    HasFailed = true;
                    throw new Exception("Something happened, AAAH!");
                }

                Handled = true;
            });
        }
    }

    /// <summary>
    /// A projection that fails after "failAfter" attempts, but then continues.
    /// It also fails if it sees the same message twice (hence not idempotent)
    /// </summary>
    public class FailingNonIdempotentProjection : ConnectedProjection<TestDbContext>
    {
        public HashSet<int> HandledMessages { get; }

        public FailingNonIdempotentProjection(int failAfter)
        {
            var failedAlready = false;
            HandledMessages = new HashSet<int>();

            When<Envelope<TestMessage>>((context, message) =>
            {
                if (HandledMessages.Count == failAfter && !failedAlready)
                {
                    failedAlready = true;
                    throw new Exception($"I was asked to fail after {failAfter} handled messages");
                }

                if (!HandledMessages.Contains(message.Message.Id))
                {
                    HandledMessages.Add(message.Message.Id);
                }
                else
                {
                    throw new Exception($"I already handled message {message.Message.Id}");
                }
            });
        }
    }

    public class TestDbContext : RunnerDbContext<TestDbContext>
    {
        public override string ProjectionStateSchema => "Test";

        public TestDbContext()
        {
        }

        // This needs to be DbContextOptions<T> for Autofac!
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguringOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory.TestDb.TestDbContext;Trusted_Connection=True;");
        }
    }
}
