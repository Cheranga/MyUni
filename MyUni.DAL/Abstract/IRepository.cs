using System.Linq;
using MyUni.Business;
using StageDocs.DAL.Abstract;

namespace MyUni.DAL.Abstract
{
    public interface IRepository<T> where T:class, IModel
    {
        T GetById(int id);

        IQueryable<T> GetAll();

        T Add(T entity);

        void Delete(T entity);

        void Delete(int id);

        void Update(T entity);

        IUoW UoW { get; set; }
    }
}