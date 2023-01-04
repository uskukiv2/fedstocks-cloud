namespace fed.cloud.common.Infrastructure
{
    public interface IConfiguration
    {
        void Configure();
    }

    public interface IRepoConfiguration : IConfiguration
    {
    }
}
