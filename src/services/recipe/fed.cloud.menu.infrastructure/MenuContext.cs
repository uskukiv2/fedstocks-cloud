using System.Data;
using System.Data.Common;
using fed.cloud.menu.infrastructure.Configurations;
using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Models.Read;
using fed.cloud.menu.infrastructure.Factories;
using fed.cloud.menu.infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace fed.cloud.menu.infrastructure;

public class MenuContext : DbContext, IUnitOfWork<NpgsqlConnection>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepositoryFactory _repositoryFactory;
    
    private IDbContextTransaction? _currentTransaction;

    public MenuContext(IServiceProvider serviceProvider, IRepositoryFactory repositoryFactory, DbContextOptions<MenuContext> options) : base(options)
    {
        _serviceProvider = serviceProvider;
        _repositoryFactory = repositoryFactory;
    }
    
    public MenuContext(DbContextOptions<MenuContext> options) : base(options) { }

    public DbTransaction? Transaction => _currentTransaction?.GetDbTransaction();

    public NpgsqlConnection Connection => (NpgsqlConnection)Database.GetDbConnection();
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Recipe> Recipes { get; set; }
    
    public DbSet<Ingredient> Ingredients { get; set; }
    
    public DbSet<Unit> UnitTypes { get; set; }
    
    public DbSet<RecipeModel> RecipeModels { get; set; }
    
    public DbSet<RecipeIngredientModel> RecipeIngredientModels { get; set; }
    
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new IngredientEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeIngredientEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UnitTypeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new RecipeModelEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeIngredientModelEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new AuditLogsEntityTypeConfiguration());
    }

    public async Task BeginAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("Transaction operating");
        }

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    public async Task CommitAsync(Guid callerId)
    {
        await OnBeforeCommitAsync(callerId);
        
        await CommitAsync();
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

    public T GetRepository<T>() where T : IRepository
    {
        return _repositoryFactory.Create<T>();
    }

    private async Task OnBeforeCommitAsync(Guid callerId)
    {
        ChangeTracker.DetectChanges();

        var auditManager = _serviceProvider.GetRequiredService<IAuditManager>();

        await auditManager.WriteAuditAsync(callerId, ChangeTracker.Entries());
    }
}