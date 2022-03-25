namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Tests.FeedProjector
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class When_starting_the_projector_with_a_projection_that_throws_an_exception
    {
        private readonly FeedProjector<FeedProjectorTestContext> _sut;
        private readonly LoggerFactoryMock _loggerFactoryMock;
        private readonly FeedProjectionException _expectedException;

        public When_starting_the_projector_with_a_projection_that_throws_an_exception()
        {
            _loggerFactoryMock = new LoggerFactoryMock();
            _expectedException = new FeedProjectionException("error1");

            _sut = new FeedProjector<FeedProjectorTestContext>(
                () => new Mock<Owned<FeedProjectorTestContext>>().Object, 
                _loggerFactoryMock,
                new []{ new FailingRunner(_expectedException) });
        }

        private Func<Task> StartProjections =>
            () => _sut.Start(CancellationToken.None);

        [Fact]
        public void Then_the_projector_does_not_throw_an_exception()
        {
           StartProjections.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Then_the_projector_logs_the_thrown_exception()
        {
           await StartProjections();

           _loggerFactoryMock
               .ResolveLogger<FeedProjector<FeedProjectorTestContext>>()
               .Verify(
                   logger => logger.Log(
                       LogLevel.Error,
                       It.IsAny<EventId>(),
                       It.Is<It.IsAnyType>((v, t) => v.ToString() == "FeedProjectionRunner failed"),
                       _expectedException,
                       (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                   Times.Once);
        }
    }

    public class When_starting_the_projector_with_a_multiple_failing_projections_and_an_infinte_projection
    {
        private readonly FeedProjector<FeedProjectorTestContext> _sut;
        private readonly LoggerFactoryMock _loggerFactoryMock;
        private readonly FeedProjectionException _firstExpectedException, _secondExpectedException;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public When_starting_the_projector_with_a_multiple_failing_projections_and_an_infinte_projection()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _loggerFactoryMock = new LoggerFactoryMock();
            _firstExpectedException = new FeedProjectionException("error1");
            _secondExpectedException = new FeedProjectionException("error2");

            _sut = new FeedProjector<FeedProjectorTestContext>(
                () => new Mock<Owned<FeedProjectorTestContext>>().Object,
                _loggerFactoryMock,
                new IFeedProjectionRunner<FeedProjectorTestContext>[]
                {
                    new FailingRunner(_secondExpectedException, TimeSpan.FromMilliseconds(300)),
                    new InfiniteRunner(),
                    new FailingRunner(_firstExpectedException, TimeSpan.FromMilliseconds(50)),
                });
        }

        private Func<Task> StartProjections =>
            () => _sut.Start(_cancellationTokenSource.Token);

        private void StopProjections()
            => _cancellationTokenSource.Cancel();

        [Fact]
        public async Task Then_the_projector_logs_the_first_thrown_exception_is_logged_while_the_projector_is_still_running()
        {
            //don't await starting the projections
            StartProjections();

            // wait for exception to be thrown
            await Task.Delay(55);

            _loggerFactoryMock
                .ResolveLogger<FeedProjector<FeedProjectorTestContext>>()
                .Verify(
                    logger => logger.Log(
                        LogLevel.Error,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString() == "FeedProjectionRunner failed"),
                        _firstExpectedException,
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Once);

            StopProjections();
        }

        [Fact]
        public async Task Then_the_projector_logs_the_second_thrown_exception_is_logged_while_the_projector_is_still_running()
        {
            //don't await starting the projections
            StartProjections();

            // wait for exception to be thrown
            await Task.Delay(305);

            _loggerFactoryMock
                .ResolveLogger<FeedProjector<FeedProjectorTestContext>>()
                .Verify(
                    logger => logger.Log(
                        LogLevel.Error,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString() == "FeedProjectionRunner failed"),
                        _secondExpectedException,
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Once);

            StopProjections();
        }
    }
}
