using System;
using System.Linq;
using System.Linq.Expressions;
using MyUni.Business;
using MyUni.DAL.Abstract;

namespace StageDocs.DAL.Abstract
{
    public interface IUoW
    {
        IRepository<T> GetRepository<T>() where T : class, IModel;
        IDataResult Commit(Action action = null);

        /// <summary>
        /// Shortcut method made available to the caller to get all records without getting the repository first
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> Get<T>() where T : class, IModel;

        /// <summary>
        /// Shortcut method made available to the caller to get records according to any filter given
        /// </summary>
        /// <typeparam name="T">The required type</typeparam>
        /// <param name="filter">The filter which needs to be applied</param>
        /// <returns>IQueryable of T</returns>
        IQueryable<T> Get<T>(Expression<Func<T, bool>> filter) where T : class, IModel;

    }

    public interface IDataResult
    {
        bool Status { get; set; }
        Exception Exception { get; set; }
    }
}