using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Api.Providers
{
    internal class OAuthBase : OAuthAuthorizationServerProvider, IOAuthProvider
    {
    }
}