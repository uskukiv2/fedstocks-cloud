using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Repository;
using gen.common.Extensions;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace fed.cloud.menu.infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MenuContext _recipeContext;

    public UserRepository(MenuContext recipeContext)
    {
        _recipeContext = recipeContext;
    }

    public IUnitOfWork UnitOfWork => _recipeContext;
    
    public Task<User> GetAsync(Guid id)
    {
        return _recipeContext.Users.SingleOrDefaultAsync(u => u.Id == id);
    }
    
    public Task<User> GetByAuthenticationIdAsync(string authenticationId)
    {
        return _recipeContext.Users.SingleOrDefaultAsync(u => u.AuthenticationId == authenticationId);
    }

    public async void Add(User entity, CancellationToken token = default)
    {
        var task = new Task(async () =>
        {
            await _recipeContext.Users.AddAsync(entity, token);
            _recipeContext.Entry(entity).State = EntityState.Added;
        }, token);

        await task.Forget();
    }

    public async void Update(User entity, CancellationToken token = default)
    {
        var task = new Task(() =>
        {
            _recipeContext.Users.Update(entity);
            _recipeContext.Entry(entity).State = EntityState.Modified;
        }, token);

        await task.Forget();
    }
}