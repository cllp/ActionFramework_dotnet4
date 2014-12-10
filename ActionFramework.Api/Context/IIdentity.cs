using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Api.Context
{
    public interface IIdentity
    {
        bool IsAuthenticated
        {
            get;
        }

        dynamic User
        {
            get;
        }

        string JsonUser
        {
            get;
        }
    }
}
