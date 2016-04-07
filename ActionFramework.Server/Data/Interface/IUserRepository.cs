using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ActionFramework.Domain.EF;
using ActionFramework.Domain.Model;

namespace ActionFramework.Server.Data.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        User Authenticate(string email, string password);
    }
}
