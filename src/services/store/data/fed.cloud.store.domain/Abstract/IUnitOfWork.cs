namespace fed.cloud.store.domain.Abstract
{
    public interface IUnitOfWork
    {
        DbTransaction Transaction { get; }

        Task ApplyChanges();
    }

    public interface IUnitOfWork<TDbConnection> : IUnitOfWork where TDbConnection : DbConnection
    {
        TDbConnection Connection { get; }
    }
}