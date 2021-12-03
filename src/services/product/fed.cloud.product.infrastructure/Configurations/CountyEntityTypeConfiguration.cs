using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.product.infrastructure.Configurations;

public class CountyEntityTypeConfiguration : IEntityTypeConfiguration<County>
{
    private readonly IServiceConfiguration _config;

    public CountyEntityTypeConfiguration(IServiceConfiguration config)
    {
        _config = config;
    }

    public void Configure(EntityTypeBuilder<County> builder)
    {
        builder.ToTable("counties", _config.GetSchema());
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(x => x.NumberInCountry)
            .HasColumnName("Number")
            .IsRequired();

        builder.HasOne(x => x.Country)
            .WithOne()
            .HasForeignKey("CountryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}