using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;
using TuanZi.EventBuses;

namespace TuanZi.Web.Managers
{
    
    public abstract class Manager<TEntity, TKey>  where TEntity: IEntity<TKey>
    {
        protected IRepository<TEntity, TKey> Repository { get; set; }
        protected IUnitOfWorkManager UnitOfWorkManager { get; set; }
        protected IEventBus EventBus { get; set; }
        protected IServiceProvider ServiceProvider;

        public Manager(IRepository<TEntity, TKey> repository, IUnitOfWorkManager unitOfWorkManager, IEventBus eventBus, IServiceProvider serviceProvider)
        {
            Repository = repository;
            UnitOfWorkManager = unitOfWorkManager;
            EventBus = eventBus;
            ServiceProvider = serviceProvider;
        }


    }
}
