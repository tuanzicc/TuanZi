using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using TuanZi.AspNetCore.Http;
using TuanZi.Entity;
using TuanZi.EventBuses;
using TuanZi.Exceptions;

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

        protected ClaimsPrincipal CurrentUser
        {
            get
            {
                var context = ServiceProvider.GetService(typeof(HttpContext)) as HttpContext;
                if (!context.User.Identity.IsAuthenticated)
                    throw new SessionExpiredException("Your session has expired.");

                return context.User;
            }
        }

        protected Guid CurrentUserId
        {
            get
            {
                return CurrentUser.GetUserId<Guid>();
            }
        }

        protected Guid CurrentAppId
        {
            get
            {
                return CurrentUser.GetAppId<Guid>();
            }

        }

        protected void LogError(Exception e)
        {
            try
            {
                var logger = ServiceProvider.GetLogger(GetType());
                logger.LogError(e.FormatMessage());
            }
            catch { }
        }


        protected virtual void Commit()
        {
            UnitOfWorkManager.Commit();
        }
    }
}
