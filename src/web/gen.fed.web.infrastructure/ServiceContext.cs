using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using gen.fed.web.domain.Abstract;
using gen.fed.web.domain.Entities;
using gen.fed.web.infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace gen.fed.web.infrastructure;

public class ServiceContext : DbContext, IUnitOfWork<NpgsqlConnection>
{
    private IDbContextTransaction? _currentTransaction;

    public ServiceContext(DbContextOptions<ServiceContext> options) : base(options) { }

    public DbTransaction? Transaction => _currentTransaction?.GetDbTransaction();

    public NpgsqlConnection Connection => (NpgsqlConnection)Database.GetDbConnection();

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

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