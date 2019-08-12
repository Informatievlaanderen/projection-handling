namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using EntityFrameworkCore.EntityTypeConfiguration;
    using Microsoft.EntityFrameworkCore;
    using ProjectionStates;

    public abstract class RunnerDbContext<TContext> : DbContext where TContext : DbContext
    {
        public abstract string ProjectionStateSchema { get; }

        public DbSet<ProjectionStateItem> ProjectionStates { get; set; }

        protected RunnerDbContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        protected RunnerDbContext(DbContextOptions<TContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
                OnConfiguringOptionsBuilder(optionsBuilder);
        }

        protected virtual void OnConfiguringOptionsBuilder(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProjectionStatesConfiguration(ProjectionStateSchema));
            modelBuilder.AddEntityConfigurationsFromAssembly(typeof(TContext).GetTypeInfo().Assembly);
        }

        public async Task UpdateProjectionState(string projectionName, long position, CancellationToken cancellationToken)
        {
            var projectionStateItem = await ProjectionStates.SingleOrDefaultAsync(item => item.Name == projectionName, cancellationToken);

            if (projectionStateItem == null)
            {
                projectionStateItem = new ProjectionStateItem { Name = projectionName };
                await ProjectionStates.AddAsync(projectionStateItem, cancellationToken);
            }

            projectionStateItem.Position = position;
        }

        public async Task UpdateProjectionDesiredState(string projectionName, string userRequestedState, CancellationToken cancellationToken)
        {
            var projectionStateItem = await ProjectionStates.SingleOrDefaultAsync(item => item.Name == projectionName, cancellationToken);

            projectionStateItem.DesiredState = userRequestedState;
            projectionStateItem.DesiredStateChangedAt = DateTimeOffset.UtcNow;
        }
    }
}
