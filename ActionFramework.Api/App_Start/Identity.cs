using ActionFramework.Api.Context;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace ActionFramework.Api
{
    public class Identity : IIdentity
    {
        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        private ClaimsIdentity claimsIdentity
        {
            get
            {
                return AuthenticationManager.User.Identities.FirstOrDefault();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                if (claimsIdentity == null)
                    return false;
                else
                    return this.claimsIdentity.IsAuthenticated;
            }
        }

        public IList<Claim> Claims
        {
            get
            {
                return claimsIdentity.Claims as IList<Claim> ?? claimsIdentity.Claims.ToList();
            }
        }

        public Claim GetClaim(string type)
        {
            return Claims.Where(c => c.Type.ToString().ToLower().EndsWith(type.ToLower(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        public string JsonUser
        {
            get
            {
                var claim = GetClaim("userdata");
                if (claim != null)
                {
                    return claim.Value;
                }
                else
                    return string.Empty;
            }
        }

        public dynamic User
        {
            get
            {
                return JsonConvert.DeserializeObject<dynamic>(JsonUser);
            }
        }
    }
}