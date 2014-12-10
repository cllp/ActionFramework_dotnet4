using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ActionFramework.Api.Providers
{
    internal sealed class MockAuthorizationServerProvider : OAuthBase
    {
        //public ILog log = CoreFactory.Log;

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // OAuth2 supports the notion of client authentication
            // this is not used here
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            bool validated = true;

            //var user = CoreFactory.AppRepository.Validate(name, email, adkey, "SPM", out validated);

            dynamic user = new
            {
                Id = 1,
                Name = "Claes-Philip",
                Email = "Claes-Philip@Staiger.se",
            };

            if (validated) //todo addd validation
            {
                // create identity
                var serialized = JsonConvert.SerializeObject(user);
                var id = new ClaimsIdentity(context.Options.AuthenticationType);
                id.AddClaim(new Claim(ClaimTypes.UserData, serialized));
                id.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                context.Validated(id);
            }
            else
            {
                var ex = new System.Security.Authentication.AuthenticationException(string.Format("User '{0}' could not be validated", context.UserName));
                //log.Error("", ex);
                context.Rejected();
            }
        }
    }
}