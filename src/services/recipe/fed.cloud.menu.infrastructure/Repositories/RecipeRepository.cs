using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Repository;
using gen.common.Extensions;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace fed.cloud.menu.infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly MenuContext _context;

    public RecipeRepository(MenuContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;
    
    public Task<Recipe> GetAsync(Guid id)
    {
        return _context.Recipes.SingleOrDefaultAsync(x => x.Id == id);
    }
    
    public IAsyncEnumerable<Recipe> GetAllByUserIdAsync(Guid userId)
    {
        return _context.Recipes.AsNoTracking().Where(x => x.OwnerId == userId)
            .Include(x => x.Ingredients)
            .AsAsyncEnumerable();
    }

    public IQueryable<Recipe> GetByPageAsync(int size, int positions)
    {
        return _context.Recipes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Skip(positions)
            .Take(size)
            .Include(r => r.Ingredients)
            .AsQueryable();
    }

    public async void Add(Recipe entity, CancellationToken token = default)
    {
        await Task.Run(async () =>
        {
            await _context.Recipes.AddAsync(entity, token);
            _context.Entry(entity).State = EntityState.Added;
        }, token).Forget();
    }

    public async void Update(Recipe entity, CancellationToken token = default)
    {
        await Task.Run(() =>
        { 
            _context.Recipes.Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }, token).Forget();
    }

    public Task DeleteRecipe(Guid id, CancellationToken ct)
    {
        return Task.Run(async () =>
        {
            var recipe = await GetAsync(id);
            _context.Recipes.Remove(recipe);
            _context.Entry(recipe).State = EntityState.Deleted;
        }, ct).Forget();
    }
}