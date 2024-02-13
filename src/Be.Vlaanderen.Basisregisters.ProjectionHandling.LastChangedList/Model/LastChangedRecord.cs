namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Model
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class LastChangedRecord
    {
        public string Id { get; set; }
        public string? CacheKey { get; set; }
        public string? Uri { get; set; }
        public string? AcceptType { get; set; }

        public long Position { get; set; }
        public long LastPopulatedPosition { get; set; }

        public int ErrorCount { get; set; }
        public DateTimeOffset? LastError { get; set; }
        public string? LastErrorMessage { get; set; }

        public bool ToBeIndexed { get; private set; }
    }

    public class LastChangedRecordConfiguration : IEntityTypeConfiguration<LastChangedRecord>
    {
        private readonly string _tableName;
        private readonly string _schema;

        public LastChangedRecordConfiguration(string tableName, string schema)
        {
            _tableName = tableName;
            _schema = schema;
        }

        public LastChangedRecordConfiguration()
        { }

        public void Configure(EntityTypeBuilder<LastChangedRecord> builder)
        {
            builder
                .ToTable(_tableName, _schema)
                .HasKey(x => x.Id)
                .IsClustered();

            builder
                .Property(x => x.ToBeIndexed)
                .ValueGeneratedOnAddOrUpdate()
                .HasComputedColumnSql("CAST(CASE WHEN (([Position] > [LastPopulatedPosition]) AND ([ErrorCount] < 10)) THEN 1 ELSE 0 END AS bit) PERSISTED");

            builder
                .HasIndex(x => x.ToBeIndexed)
                .IncludeProperties(x => x.LastError);
            builder.HasIndex(x => x.Position);
            builder.HasIndex(x => x.CacheKey);
            builder.HasIndex(x => x.ErrorCount);
            builder.HasIndex(x => x.LastError);
            builder.HasIndex(x => new { x.ToBeIndexed, x.LastError });
            builder.HasIndex(x => new
            {
                x.Position,
                x.LastPopulatedPosition,
                x.ErrorCount,
                x.LastError
            });
        }
    }
}
