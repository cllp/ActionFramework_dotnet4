using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Server.Data.Interface;
using ActionFramework.Domain.Model;
using Dapper;
using ActionFramework.Server.Data.Helpers;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq.Expressions;

namespace ActionFramework.Server.Data.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private string _tableName;

        public Repository()
        {
            this._tableName = typeof(T).Name;
        }

        public virtual int Insert(T item)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                int returnid = cn.Insert(item);
                return returnid;
            }
        }

        public virtual void Update(T item)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                cn.Update(item);
            }
        }

        public virtual void Remove(T item)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                cn.Delete(item);
            }
        }

        public virtual T GetById(int id)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                return cn.Query<T>("SELECT * FROM " + _tableName + " WHERE Id = @Id", new { Id = id }).FirstOrDefault();
            }
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> items = null;

            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                QueryResult result = DynamicQuery.GetDynamicQuery(_tableName, predicate);
                items = cn.Query<T>(result.Sql, (object)result.Param);
                return items;
            }
        }

        public virtual IEnumerable<T> FindAll()
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {                
                return cn.Query<T>("SELECT * FROM " + _tableName);
            }
        }
    }
}
