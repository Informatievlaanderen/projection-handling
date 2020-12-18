namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.ProjectionStates
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProjectionStateItem
    {
        public string Name { get; set; }
        public long Position { get; set; }
        public string? DesiredState { get; set; }
        public DateTimeOffset? DesiredStateChangedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ProjectionStatesConfiguration : IEntityTypeConfiguration<ProjectionStateItem>
    {
        private readonly string _schema;
        private const string TableName = "ProjectionStates";

        public ProjectionStatesConfiguration(string schema)
        {
            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentException("Schema cannot be empty.", nameof(schema));

            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ProjectionStateItem> b)
        {
            b.ToTable(TableName, _schema)
                .HasKey(p => p.Name)
                .IsClustered();

            b.Property(p => p.Name);
            b.Property(p => p.Position);
            b.Property(p => p.DesiredState);
            b.Property(p => p.DesiredStateChangedAt);
            b.Property(p => p.ErrorMessage);
        }
    }
}
