using System;
using System.Data.Entity;
using System.Diagnostics;
using MyUni.Business;
using MyUni.DAL.Abstract;
using StageDocs.DAL.Abstract;

namespace MyUni.DAL.Concrete
{
    public class UoW : IUoW
    {
        protected readonly DbContext Context;
        private readonly IRepositoryFactory repositoryFactory;

        public UoW(DbContext context, IRepositoryFactory repositoryFactory)
        {
            if (context == null)
            {
                throw new ArgumentException("context cannot be null");
            }

            if (repositoryFactory == null)
            {
                throw new ArgumentException("repositoryFactory cannot be null");
            }

            this.Context = context;
            this.repositoryFactory = repositoryFactory;

            Debug.WriteLine("UoW created...");
        }

        public virtual IRepository<T> GetRepository<T>() where T:class, IModel
        {
            return this.repositoryFactory.GetRepository<T>();
        }

        public virtual IDataResult Commit( Action action = null  )
        {
            using (var transaction = this.Context.Database.BeginTransaction())
            {
                try
                {
                    if (action != null)
                    {
                        action();
                    }

                    this.Context.SaveChanges();
                    transaction.Commit();

                    var dataResult = new DataResult
                    {
                        Status = true
                    };

                    return dataResult;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    var dataResult = new DataResult
                    {
                        Exception = exception
                    };

                    return dataResult;
                }
            }
        }

        //public virtual T Commit<T>(Func<T> action) where T:class 
        //{
        //    T result = null;
        //    using (var transaction = this.Context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            result = action();

        //            this.Context.SaveChanges();
        //            transaction.Commit();
        //        }
        //        catch (Exception exception)
        //        {
        //            transaction.Rollback();

        //            throw;
        //        }
        //    }
        //}

    }

    public class DataResult : IDataResult
    {
        public bool Status { get; set; }

        public Exception Exception { get; set; }
    }
}
