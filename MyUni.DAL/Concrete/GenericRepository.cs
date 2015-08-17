using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MyUni.Business;
using MyUni.DAL.Abstract;
using Ninject;
using StageDocs.DAL.Abstract;

namespace MyUni.DAL.Concrete
{
    public class GenericRepository<T> : IRepository<T> where T:class, IModel
    {
        public readonly DbContext Context;

        protected readonly DbSet<T> dbSet = null; 

        public IUoW UoW { get; set; }

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


        public virtual T GetById(int id)
        {
            return this.dbSet.Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return this.dbSet;
        }

        public virtual T Add(T entity)
        {
            if (entity == null)
            {
                return null;
            }

            var addedEntity = this.dbSet.Add(entity);
            return addedEntity;
        }

        public virtual void Delete(T entity)
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

        public virtual void Delete(int id)
        {
            var entity = this.GetById(id);
            this.Delete(entity);
        }

        public virtual void Update(T entity)
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

        public virtual IQueryable<T> Get(Func<T, bool> filter)
        {
            if (filter == null)
            {
                return null;
            }

            return this.dbSet.Where(filter).AsQueryable();
        }

        public virtual IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> filter)
        {
            if (filter == null)
            {
                return null;
            }

            return this.dbSet.Include(filter);
        }
    }
}