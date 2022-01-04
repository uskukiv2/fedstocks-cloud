using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.product.infrastructure.Configurations;

public class CountryEntityTypeConfiguration : IEntityTypeConfiguration<Country>
{
    private readonly IServiceConfiguration _config;

    public CountryEntityTypeConfiguration(IServiceConfiguration config)
    {
        _config = config;
    }

    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("countries", _config.GetSchema());
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(x => x.GlobalId)
            .HasColumnName("GlobalId")
            .HasColumnType("text")
            .IsRequired();

        builder.HasGeneratedTsVectorColumn(c => c.SearchVector,
                _config.Database.VectorConfig,
                c => new { c.Name, c.GlobalId })
            .HasIndex(c => c.SearchVector)
            .HasMethod("GIN");
    }
}