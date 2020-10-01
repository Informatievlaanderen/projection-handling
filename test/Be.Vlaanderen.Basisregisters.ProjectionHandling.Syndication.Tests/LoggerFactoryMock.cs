namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Moq;

    public class LoggerFactoryMock : ILoggerFactory
    {
        private readonly IDictionary<string, Mock<ILogger>> _loggerMocks; 

        public LoggerFactoryMock()
        {
            _loggerMocks = new Dictionary<string, Mock<ILogger>>();
        }

        public void Dispose() {}

        public Mock<ILogger> ResolveLogger<T>()
        {
            // code that determines the categoryName from a type is scoped to internal
            // this might break any of that code changes

            var type = typeof(T);
            var fullName = type.FullName ?? type.Name;
            var genericTypeSeparator = fullName.IndexOf('`');

            return ResolveLogger( genericTypeSeparator < 0 ? fullName : fullName.Substring(0, genericTypeSeparator));
        }

        public Mock<ILogger> ResolveLogger(string categoryName)
        {
            if (!_loggerMocks.ContainsKey(categoryName))
            {
                var loggerMock = new Mock<ILogger>();

                // set ILogger.Log by default
                loggerMock.Setup(logger =>
                    logger.Log(
                        It.IsAny<LogLevel>(),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => true),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

                _loggerMocks.Add(categoryName, loggerMock);
            }

            return _loggerMocks[categoryName];
        }

        public ILogger CreateLogger(string categoryName)
            => ResolveLogger(categoryName).Object;


        public void AddProvider(ILoggerProvider provider)
            => throw new NotImplementedException();
    }
}
