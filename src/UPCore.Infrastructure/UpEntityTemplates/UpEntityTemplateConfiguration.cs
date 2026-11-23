using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Infrastructure.UpEntityTemplates;

internal sealed class UpEntityTemplateConfiguration : IEntityTypeConfiguration<UpEntityTemplate>
{
    private const int TypeMaxLength = 100;

    public void Configure(EntityTypeBuilder<UpEntityTemplate> builder)
    {
        builder.ToTable("UpEntityTemplates");

        builder.HasKey(upEntityTemplate => upEntityTemplate.Id);
        builder.Property(upEntityTemplate => upEntityTemplate.Id).ValueGeneratedNever();

        builder.Property(upEntityTemplate => upEntityTemplate.Type)
            .HasMaxLength(TypeMaxLength)
            .IsRequired();

        builder.HasIndex(upEntityTemplate => upEntityTemplate.Type).IsUnique();

        builder.OwnsMany(upEntityTemplate => upEntityTemplate.Fields, fields =>
        {
            fields.ToJson();
            fields.Property(field => field.Kind).HasConversion<string>();
        });
    }
}
