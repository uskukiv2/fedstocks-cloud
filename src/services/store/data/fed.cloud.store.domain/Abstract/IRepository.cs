namespace fed.cloud.store.domain.Abstract
{
    public interface IRepository<T> where T : IRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}