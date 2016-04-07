using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ActionFramework.Domain.EF;
using ActionFramework.Domain.Model;
using Dapper;
using System.Linq.Expressions;

namespace ActionFramework.Server.Data.Interface
{
    //public interface IRepository<T>
    //{
    //    bool Update(T entity);

    //    int Insert(T entity);

    //    bool Delete(T entity);

    //    T GetById(int id);
    //}

    public interface IRepository<T>
    {
        int Insert(T item);
        void Remove(T item);
        void Update(T item);
        T GetById(int id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        IEnumerable<T> FindAll();
    }
}
