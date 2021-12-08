using fed.cloud.common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.product.domain.Abstraction
{
    public interface IRepository<T> where T : IEntity
    {
        IUnitOfWork UnitOfWork { get; }
        Task<T> GetAsync(Guid id);
        void Add(T entity, CancellationToken token = default);
        void Update(T entity, CancellationToken token = default);
    }
}
