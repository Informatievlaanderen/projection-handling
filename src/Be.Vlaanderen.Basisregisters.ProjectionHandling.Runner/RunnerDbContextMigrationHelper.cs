namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Polly;

    public class MigrationsLogger { }

    public interface IRunnerDbContextMigrationHelper
    {
        Task RunMigrationsAsync(CancellationToken cancellationToken);
    }

    public abstract class RunnerDbContextMigrationHelper<TContext>: IRunnerDbContextMigrationHelper
        where TContext : RunnerDbContext<TContext>
    {
        private const int RetryCount = 5;
        private readonly string _connectionString;
        private readonly HistoryConfiguration _historyConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<MigrationsLogger> _logger;

        protected RunnerDbContextMigrationHelper(
            string connectionString,
            HistoryConfiguration historyConfiguration,
            ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _historyConfiguration = historyConfiguration;
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<MigrationsLogger>() ?? throw new NoNullAllowedException($"Could not create logger for {typeof(MigrationsLogger)}");
        }

        protected abstract TContext CreateContext(DbContextOptions<TContext> migrationContextOptions);

        public async Task RunMigrationsAsync(CancellationToken cancellationToken)
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
                        _logger.LogInformation("Running EF Migrations.");
                        await RunAsync(token);
                    },
                    cancellationToken
                );
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var migrationContextOptions = new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(
                    _connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                        sqlServerOptions
                            .MigrationsHistoryTable(
                                _historyConfiguration.Table,
                                _historyConfiguration.Schema
                            );
                    }
                )
                .UseLoggerFactory(_loggerFactory)
                .Options;

            using (var migrationContext = CreateContext(migrationContextOptions))
                await migrationContext.Database.MigrateAsync(cancellationToken);
        }

        public class HistoryConfiguration
        {
            public string Schema { get; set; }
            public string Table { get; set; }
        }
    }
}
