using fed.cloud.common.Infrastructure;

namespace fed.cloud.menu.data.Abstract
{
    public interface IRepository<T> : IRepository where T : IEntity
    {
        Task<T> GetAsync(Guid id);
        void Add(T entity, CancellationToken token = default);
        void Update(T entity, CancellationToken token = default);
    }
}
