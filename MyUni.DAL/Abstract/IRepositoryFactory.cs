using MyUni.Business;
using MyUni.DAL.Abstract;

namespace StageDocs.DAL.Abstract
{
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>() where T : class, IModel;
    }
}