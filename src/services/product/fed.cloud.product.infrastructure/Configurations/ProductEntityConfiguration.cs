using System;
using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.product.infrastructure.Configurations;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    private readonly IServiceConfiguration _config;

    public ProductEntityConfiguration(IServiceConfiguration configuration)
    {
        _config = configuration;
    }

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", _config.GetSchema());
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(x => x.Brand)
            .HasColumnName("Brand")
            .IsRequired();

        builder.Property(x => x.GlobalNumber)
            .HasColumnName("GlobalNumber")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.QuantityRate)
            .HasColumnName("QuantityRate")
            .IsRequired();

        builder.Property(x => x.Manufacturer)
            .HasColumnName("Manufacturer")
            .IsRequired(false);

        builder.Property(x => x.CategoryId)
            .HasColumnName("CategoryId")
            .IsRequired();

        builder.Property(x => x.UnitId)
            .HasColumnName("UnitId")
            .IsRequired();

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey("CategoryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey("UnitId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasGeneratedTsVectorColumn(p => p.SearchVector,
                _config.Database.VectorConfig,
                p => new {p.Brand, p.Name, p.GlobalNumber})
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");

        builder.HasIndex(x => x.GlobalNumber).IsUnique();
    }
}