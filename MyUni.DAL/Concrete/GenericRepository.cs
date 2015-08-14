using System;
using System.Data.Entity;
using System.Linq;
using MyUni.Business;
using MyUni.DAL.Abstract;

namespace MyUni.DAL.Concrete
{
    public class GenericRepository<T> : IRepository<T> where T:class, IModel
    {
        public readonly DbContext Context;

        private readonly DbSet<T> dbSet = null; 

        public GenericRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("context cannot be null");
            }

            this.Context = context;

            this.dbSet = this.Context.Set<T>();

            if (this.dbSet == null)
            {
                throw new NullReferenceException("There is no entity defined in this DbContext");
            }
        }


        public T GetById(int id)
        {
            return this.dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return this.dbSet;
        }

        public T Add(T entity)
        {
            if (entity == null)
            {
                return null;
            }

            var addedEntity = this.dbSet.Add(entity);
            return addedEntity;
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                return;
            }

            var dbEntity = this.Context.Entry(entity);
            if (dbEntity == null)
            {
                return;
            }

            dbEntity.State = EntityState.Deleted;
        }

        public void Delete(int id)
        {
            var entity = this.GetById(id);
            this.Delete(entity);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                return;
            }

            var dbEntity =  this.Context.Entry(entity);
            if (dbEntity == null)
            {
                return;
            }

            dbEntity.State = EntityState.Modified;
        }
    }
}