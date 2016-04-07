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

namespace ActionFramework.Server.Data.Repository
{
    internal class UserRepository : Repository<User>, IUserRepository
    {
        public User Authenticate(string email, string password)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {

                var query = @"SELECT u.*, c.* 
                              FROM [User] u
                              INNER JOIN Organization c ON u.OrganizationId = c.Id
                              WHERE u.Email = @Email
                              AND u.Password = @Password";                              
                              
                using (var multi = cn.QueryMultiple(query, new { Email = email, Password = password }))
                {
                    var user = multi.Read<User, Organization, User>((u, c) =>
                    {
                        u.Organization = c;
                        return u;
                    }).FirstOrDefault();

                    return user;
                }
            }
        }

    }
}
