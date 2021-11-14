using System;
using System.Collections.Generic;
using System.Text;

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
