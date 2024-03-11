namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using ProjectionStates;

    public abstract class RunnerDbContext<TContext> : DbContext
        where TContext : DbContext
    {
        public abstract string ProjectionStateSchema { get; }

        public DbSet<ProjectionStateItem> ProjectionStates => Set<ProjectionStateItem>();

        protected RunnerDbContext()
        { }

        // This needs to be DbContextOptions<T> for Autofac!
        protected RunnerDbContext(DbContextOptions<TContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                OnConfiguringOptionsBuilder(optionsBuilder);
            }
        }

        protected virtual void OnConfiguringOptionsBuilder(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProjectionStatesConfiguration(ProjectionStateSchema));
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).GetTypeInfo().Assembly);
        }

        public virtual async Task UpdateProjectionState(string? projectionName, long position, CancellationToken cancellationToken)
        {
            if (projectionName is null)
            {
                return;
            }

            var projectionStateItem = await ProjectionStates.SingleOrDefaultAsync(item => item.Name == projectionName, cancellationToken).ConfigureAwait(false);

            if (projectionStateItem == null)
            {
                projectionStateItem = new ProjectionStateItem { Name = projectionName };
                await ProjectionStates.AddAsync(projectionStateItem, cancellationToken).ConfigureAwait(false);
            }

            projectionStateItem.Position = position;
        }

        public virtual async Task SetErrorMessage(string projectionName, string? errorMessage, CancellationToken cancellationToken)
        {
            var projectionStateItem = await ProjectionStates.SingleOrDefaultAsync(item => item.Name == projectionName, cancellationToken).ConfigureAwait(false);

            if (projectionStateItem == null)
            {
                projectionStateItem = new ProjectionStateItem
                {
                    Name = projectionName,
                    Position = -1L
                };
                await ProjectionStates.AddAsync(projectionStateItem, cancellationToken).ConfigureAwait(false);
            }

            projectionStateItem.ErrorMessage = errorMessage;
        }

        public virtual async Task UpdateProjectionDesiredState(string projectionName, string desiredState, CancellationToken cancellationToken)
        {
            var projectionStateItem = await ProjectionStates.SingleOrDefaultAsync(item => item.Name == projectionName, cancellationToken).ConfigureAwait(false);

            if (projectionStateItem == null)
            {
                projectionStateItem = new ProjectionStateItem
                {
                    Name = projectionName,
                    Position = -1L
                };
                await ProjectionStates.AddAsync(projectionStateItem, cancellationToken).ConfigureAwait(false);
            }

            projectionStateItem.DesiredState = desiredState;
            projectionStateItem.DesiredStateChangedAt = DateTimeOffset.UtcNow;
        }
    }
}
