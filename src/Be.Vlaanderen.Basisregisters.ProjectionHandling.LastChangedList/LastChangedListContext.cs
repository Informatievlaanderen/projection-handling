namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Model;
    using Runner;

    public class LastChangedListContext : RunnerDbContext<LastChangedListContext>
    {
        public const string Schema = "Redis";
        public const string MigrationsHistoryTable = "__EFMigrationsHistoryLastChangedList";
        public const string TableName = "LastChangedList";

        public override string ProjectionStateSchema => Schema;

        public DbSet<LastChangedRecord> LastChangedList { get; set; }

        public LastChangedListContext(DbContextOptions<LastChangedListContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<LastChangedRecord>()
                .Property(x => x.ToBeIndexed)
                .ValueGeneratedOnAddOrUpdate()
                .HasComputedColumnSql("CAST(CASE WHEN (([Position] > [LastPopulatedPosition]) AND ([ErrorCount] < 10)) THEN 1 ELSE 0 END AS bit) PERSISTED");

            modelBuilder
                .Entity<LastChangedRecord>()
                .HasIndex(x => x.ToBeIndexed)
                .IncludeProperties(x => x.LastError);

            modelBuilder
                .Entity<LastChangedRecord>()
                .HasIndex(x => x.Position);

            modelBuilder
                .Entity<LastChangedRecord>()
                .HasIndex(x => x.CacheKey);

            modelBuilder
                .Entity<LastChangedRecord>()
                .HasIndex(x => x.ErrorCount);

            modelBuilder
                .Entity<LastChangedRecord>()
                .HasIndex(x => x.LastError);

            modelBuilder
                .Entity<LastChangedRecord>()
                .HasKey(x => x.Id)
                .IsClustered();

            modelBuilder
                .Entity<LastChangedRecord>()
                .HasIndex(x => new
                {
                    x.Position,
                    x.LastPopulatedPosition,
                    x.ErrorCount,
                    x.LastError
                });

            modelBuilder
                .HasDefaultSchema(Schema);
        }
    }

    public class LastChangedListContextFactory : IDesignTimeDbContextFactory<LastChangedListContext>
    {
        public LastChangedListContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LastChangedListContext>();
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory.LastChangedListContext;Trusted_Connection=True;",
                options =>
                {
                    options.EnableRetryOnFailure();
                    options.MigrationsHistoryTable(LastChangedListContext.MigrationsHistoryTable, LastChangedListContext.Schema);
                });

            return new LastChangedListContext(optionsBuilder.Options);
        }
    }
}
