using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Repository;
using gen.common.Extensions;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace fed.cloud.menu.infrastructure.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly MenuContext _context;

    public IngredientRepository(MenuContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;
    
    public Task<Ingredient> GetAsync(Guid id)
    {
        return _context.Ingredients.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async void Add(Ingredient entity, CancellationToken token = default)
    {
        await Task.Run(async () =>
        {
            await _context.Ingredients.AddAsync(entity, token);
            _context.Entry(entity).State = EntityState.Added;
        }, token).Forget();
    }

    public async void Update(Ingredient entity, CancellationToken token = default)
    {
        await Task.Run(() =>
        {
            _context.Ingredients.Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }, token).Forget();
    }
}