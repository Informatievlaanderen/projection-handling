namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Model;
    using Runner;
    using Runner.ProjectionStates;

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
            //base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProjectionStatesConfiguration(ProjectionStateSchema));
            modelBuilder.ApplyConfiguration(new LastChangedRecordConfiguration(TableName, Schema));

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
