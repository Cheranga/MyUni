﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using MyUni.Business;
using MyUni.DAL;
using MyUni.DAL.Concrete;
using Ninject;
using Ninject.Syntax;
using Ninject.Web.Common;
using Ninject.Web.WebApi;
using StageDocs.DAL.Abstract;

namespace MyUni.Web.Infrastructure
{
    public class MyUniDependencyResolver : NinjectDependencyScope, System.Web.Mvc.IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IKernel kernel;

        public MyUniDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            this.kernel = kernel;
            this.RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            this.kernel.Bind<DbContext>().To<MyUniDbContext>().InRequestScope();

            this.kernel.Bind<IRepositoryFactory>().ToMethod(x =>
            {
                var dbContext = this.kernel.Get<DbContext>();
                var repositoryFactory = new RepositoryFactory(dbContext);

                //
                // Set the specialized repositories
                //
                repositoryFactory.SetCustomRepo(new CourseRepository(dbContext));

                return repositoryFactory;

            }).InRequestScope();

            this.kernel.Bind<IUoW>().To<UoW>().InRequestScope();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}