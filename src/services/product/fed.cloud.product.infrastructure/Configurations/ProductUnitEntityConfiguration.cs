using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.product.infrastructure.Configurations;

public class ProductUnitEntityConfiguration : IEntityTypeConfiguration<ProductUnit>
{
    private readonly IServiceConfiguration _config;

    public ProductUnitEntityConfiguration(IServiceConfiguration config)
    {
        _config = config;
    }

    public void Configure(EntityTypeBuilder<ProductUnit> builder)
    {
        builder.ToTable("productunits", _config.GetSchema());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(x => x.Rate)
            .HasColumnName("Rate")
            .IsRequired();
    }
}