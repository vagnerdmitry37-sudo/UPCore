using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UPCore.Domain.UpEntities;

namespace UPCore.Infrastructure.UpEntities;

internal sealed class UpEntityConfiguration : IEntityTypeConfiguration<UpEntity>
{
    private const int TypeMaxLength = 100;

    public void Configure(EntityTypeBuilder<UpEntity> builder)
    {
        builder.ToTable("UpEntities");

        builder.HasKey(upEntity => upEntity.Id);
        builder.Property(upEntity => upEntity.Id).ValueGeneratedNever();

        builder.Property(upEntity => upEntity.Type)
            .HasMaxLength(TypeMaxLength)
            .IsRequired();

        builder.Property(upEntity => upEntity.Data)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasIndex(upEntity => upEntity.Type);
    }
}
