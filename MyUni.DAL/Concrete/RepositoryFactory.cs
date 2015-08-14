using System.Data.Entity;
using System.Diagnostics;
using MyUni.Business;
using MyUni.DAL.Abstract;
using StageDocs.DAL.Abstract;

namespace MyUni.DAL.Concrete
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly DbContext context;

        public RepositoryFactory(DbContext context)
        {
            this.context = context;
            Debug.WriteLine("RepositoryFactory created...");
        }

        public IRepository<T> GetRepository<T>() where T : class, IModel
        {
            var repository = new GenericRepository<T>(this.context);

            return repository;
        }
    }
}