namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Microsoft
{
    using DependencyInjection;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.Extensions.Logging;

    public class LastChangedListModule : IServiceCollectionModule
    {
        private readonly string _connectionString;
        private readonly string _datadogServiceName;
        private readonly ILoggerFactory _loggerFactory;

        public LastChangedListModule(
            string connectionString,
            string datadogServiceName,
            ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _datadogServiceName = datadogServiceName;
            _loggerFactory = loggerFactory;
        }

        public void Load(IServiceCollection services)
        {
            services.ConfigureLastChangedList(_connectionString, _datadogServiceName, _loggerFactory);
        }
    }
}
