using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace fed.cloud.product.infrastructure;

public class ProductContext : DbContext, IUnitOfWork<NpgsqlConnection>
{
    private readonly IServiceConfiguration _config;
    private IDbContextTransaction? _currentTransaction;

    public ProductContext(DbContextOptions<ProductContext> options) : base(options) {}

    public ProductContext(DbContextOptions<ProductContext> options, IServiceConfiguration config) : base(options)
    {
        _config = config;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration(_config));
        modelBuilder.ApplyConfiguration(new ProductCategoryEntityConfiguration(_config));
        modelBuilder.ApplyConfiguration(new ProductUnitEntityConfiguration(_config));
        modelBuilder.ApplyConfiguration(new ProductSellerPriceEntityTypeConfiguration(_config));
        modelBuilder.ApplyConfiguration(new SellerEntityTypeConfiguration(_config));
        modelBuilder.ApplyConfiguration(new CountryEntityTypeConfiguration(_config));
        modelBuilder.ApplyConfiguration(new CountyEntityTypeConfiguration(_config));
    }

    public DbTransaction? Transaction => _currentTransaction?.GetDbTransaction();

    public NpgsqlConnection Connection => (NpgsqlConnection) Database.GetDbConnection();

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductUnit> Units { get; set; }

    public DbSet<ProductCategory> Categories { get; set; }

    public DbSet<SellerCompany> Companies { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<County> Counties { get; set; }

    public DbSet<ProductSellerPrice> SellerPrices { get; set; }

    public async Task BeginAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("Transaction operating");
        }

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    public async Task CommitAsync()
    {
        try
        {
            if (_currentTransaction == null)
            {
                return;
            }

            await SaveChangesAsync();
            await _currentTransaction.CommitAsync()!;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            await _currentTransaction?.RollbackAsync()!;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void DropTransaction()
    {
        if (_currentTransaction == null)
        {
            return;
        }

        _currentTransaction.Dispose();
        _currentTransaction = null;
    }
}