using fed.cloud.common.Infrastructure;

namespace fed.cloud.menu.data.Abstract;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}