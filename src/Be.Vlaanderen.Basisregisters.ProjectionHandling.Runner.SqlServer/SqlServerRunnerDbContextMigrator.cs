namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.SqlServer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using Polly;
    using Runner.MigrationExtensions;

    public class SqlServerRunnerDbContextMigrator<TContext> : IRunnerDbContextMigrator
        where TContext : RunnerDbContext<TContext>
    {
        private const int RetryCount = 5;
        private readonly Func<TContext> _createContext;
        private readonly ILogger<SqlServerRunnerDbContextMigrator<TContext>> _logger;

        public SqlServerRunnerDbContextMigrator(Func<TContext> createContext, ILoggerFactory loggerFactory)
        {
            _createContext = createContext;
            _logger = loggerFactory?.CreateLogger<SqlServerRunnerDbContextMigrator<TContext>>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task MigrateAsync(CancellationToken cancellationToken)
        {
            await Policy
                .Handle<SqlException>()
                .WaitAndRetryAsync(
                    RetryCount,
                    retryAttempt =>
                    {
                        var value = Math.Pow(2, retryAttempt) / 4;
                        var randomValue = new Random().Next((int)value * 3, (int)value * 5);
                        _logger.LogInformation("Retrying after {Seconds} seconds...", randomValue);
                        return TimeSpan.FromSeconds(randomValue);
                    }
                )
                .ExecuteAsync(
                    async token =>
                    {
                        _logger.LogInformation("Running EF Migrations for {ContextType}", typeof(TContext).Name);
                        await using var migrationContext = _createContext();
                        await migrationContext.MigrateAsync(cancellationToken);
                    },
                    cancellationToken
                );
        }
    }
}
