using System;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using gen.fed.web.domain.Entities;
using gen.fed.web.domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace gen.fed.web.infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ServiceContext _context;

    public UserRepository(ServiceContext serviceContext)
    {
        _context = serviceContext;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<User> GetAsync(Guid id)
    {
        return (await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id))!;
    }

    public void Add(User entity, CancellationToken token = default)
    {
        Task.Run(async () =>
        {
            await _context.Users.AddAsync(entity, token);
            _context.Entry(entity).State = EntityState.Added;
        }, token).ConfigureAwait(false);
    }

    public void Update(User entity, CancellationToken token = default)
    {
        Task.Run(() =>
        {
            _context.Users.Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }, token).ConfigureAwait(false);
    }

    public async Task<User> GetUserByInternalId(int internalId)
    {
        return await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.InternalId == internalId);
    }
}