using fed.cloud.common.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.store.domain.Abstract
{
    public interface IRepository<T> where T : IRoot
    {
        IUnitOfWork UnitOfWork { get; }
        Task<T> GetAsync(Guid id);
        void Add(T entity, CancellationToken token = default);
        void Update(T entity, CancellationToken token = default);
    }
}