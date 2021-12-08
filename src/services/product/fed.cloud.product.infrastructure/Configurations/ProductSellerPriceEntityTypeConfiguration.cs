using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.product.infrastructure.Configurations;

public class ProductSellerPriceEntityTypeConfiguration : IEntityTypeConfiguration<ProductSellerPrice>
{
    private readonly IServiceConfiguration _config;

    public ProductSellerPriceEntityTypeConfiguration(IServiceConfiguration config)
    {
        _config = config;
    }

    public void Configure(EntityTypeBuilder<ProductSellerPrice> builder)
    {
        builder.ToTable("productsellerprices", _config.GetSchema());
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalCurrencyNumber)
            .HasColumnName("OriginalCurrencyNumber")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("Price")
            .IsRequired(false);

        builder.Property(x => x.SellerId)
            .HasColumnName("SellerId")
            .IsRequired();

        builder.Property(x => x.OriginalPrice)
            .HasColumnName("OriginalPrice")
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasColumnName("ProductId")
            .IsRequired();

        builder.HasOne(x => x.Product)
            .WithMany(x => x.SellerPrices)
            .HasForeignKey("ProductId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Seller)
            .WithMany(x => x.SellerPrices)
            .HasForeignKey("SellerId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}