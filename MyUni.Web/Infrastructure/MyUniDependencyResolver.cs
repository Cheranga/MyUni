using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using MyUni.DAL;
using Ninject;
using Ninject.Syntax;
using Ninject.Web.Common;
using Ninject.Web.WebApi;

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
            this.kernel.Bind<MyUniDbContext>().To<MyUniDbContext>().InRequestScope();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}