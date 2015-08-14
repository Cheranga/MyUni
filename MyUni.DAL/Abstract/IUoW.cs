using System;
using MyUni.Business;
using MyUni.DAL.Abstract;

namespace StageDocs.DAL.Abstract
{
    public interface IUoW
    {
        IRepository<T> GetRepository<T>() where T : class, IModel;
        IDataResult Commit(Action action = null);
    }

    public interface IDataResult
    {
        bool Status { get; set; }
        Exception Exception { get; set; }
    }
}