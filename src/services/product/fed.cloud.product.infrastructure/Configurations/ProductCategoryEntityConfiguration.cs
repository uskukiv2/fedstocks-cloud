using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.product.infrastructure.Configurations;

public class ProductCategoryEntityConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    private readonly IServiceConfiguration _config;

    public ProductCategoryEntityConfiguration(IServiceConfiguration config)
    {
        _config = config;
    }

    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("productcategories", _config.GetSchema());
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(x => x.ParentId)
            .HasColumnName("ParentId")
            .IsRequired(false);

        builder.HasOne(x => x.ParentCategory)
            .WithMany()
            .HasForeignKey("ParentId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}