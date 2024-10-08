﻿using System.Threading.Tasks;
using System.Threading;
using System;
using fed.cloud.common.Infrastructure;

namespace gen.fed.web.domain.Abstract
{
    public interface IRepository<T> where T : IEntity
    {
        IUnitOfWork UnitOfWork { get; }
        Task<T> GetAsync(Guid id);
        void Add(T entity, CancellationToken token = default);
        void Update(T entity, CancellationToken token = default);
    }
}