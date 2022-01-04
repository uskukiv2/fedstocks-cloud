using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.product.infrastructure.Configurations;

public class SellerEntityTypeConfiguration : IEntityTypeConfiguration<SellerCompany>
{
    private readonly IServiceConfiguration _config;

    public SellerEntityTypeConfiguration(IServiceConfiguration config)
    {
        _config = config;
    }

    public void Configure(EntityTypeBuilder<SellerCompany> builder)
    {
        builder.ToTable("sellercompanies", _config.GetSchema());
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalName)
            .HasColumnName("OriginalName")
            .IsRequired();

        builder.Property(x => x.CountyId)
            .HasColumnName("CountyId")
            .IsRequired();

        builder.Property(x => x.CountryId)
            .HasColumnName("CountryId")
            .IsRequired();

        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey("CountryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(x => x.County)
            .WithMany()
            .HasForeignKey("CountyId")
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasGeneratedTsVectorColumn(p => p.SearchVector,
                _config.Database.VectorConfig,
                p => new { p.OriginalName })
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");
    }
}