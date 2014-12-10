using ActionFramework.Api.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ActionFramework.Api
{
    public static class AuthFactory
    {
        public static IOAuthProvider Provider
        {
            get
            {
                if(bool.Parse(ConfigurationManager.AppSettings["AuthenticationMock"]))
                    return new MockAuthorizationServerProvider();
                else
                    return new SimpleAuthorizationServerProvider();
            }
        }
    }
}