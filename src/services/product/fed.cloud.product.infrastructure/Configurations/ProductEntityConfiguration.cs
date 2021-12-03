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
        builder.OwnsMany<ProductSellerPrice>(x => x.SellerPrices, sp =>
        {
            sp.Property(x => x.ProductId);
            sp.WithOwner();
        });

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(x => x.Brand)
            .HasColumnName("Brand")
            .IsRequired();

        builder.Property(x => x.GlobalNumber)
            .HasColumnName("GlobalNumber")
            .IsRequired();

        builder.Property(x => x.QuantityRate)
            .HasColumnName("QuantityRate")
            .IsRequired();

        builder.Property(x => x.Manufacturer)
            .HasColumnName("Manufacturer")
            .IsRequired(false);

        builder.HasOne(x => x.Category)
            .WithOne()
            .HasForeignKey("CategoryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Unit)
            .WithOne()
            .HasForeignKey("UnitId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasGeneratedTsVectorColumn(p => p.SearchVector,
                _config.Database.VectorConfig,
                p => new {p.Brand, p.Name, p.GlobalNumber})
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");
    }
}