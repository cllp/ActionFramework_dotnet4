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
    internal sealed class SimpleAuthorizationServerProvider : OAuthBase
    {
        private static string address = ConfigurationManager.AppSettings["ActiveDirectoryAddress"];
        private static string container = ConfigurationManager.AppSettings["ActiveDirectoryContainer"];
        private static string groupname = ConfigurationManager.AppSettings["ActiveDirectoryGroupName"];

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // OAuth2 supports the notion of client authentication
            // this is not used here
            
            
            //this is if we have a client secret etc...
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            bool isMember = false;
            
            using (var adContext = new PrincipalContext(ContextType.Domain, address, container))
            using (var groupPrincipal = GroupPrincipal.FindByIdentity(adContext, groupname))
            using (var userPrincipal = UserPrincipal.FindByIdentity(adContext, context.UserName))
            {
                if (userPrincipal != null)
                {
                    isMember = userPrincipal.IsMemberOf(groupPrincipal);
                    if (isMember)
                    {
                        bool validated = true;
                        //var user = CoreFactory.AppRepository.Validate(
                        //                                            userPrincipal.Name,
                        //                                            userPrincipal.EmailAddress,
                        //                                            userPrincipal.Sid.Value,
                        //                                            groupname,
                        //                                            out validated);

                        dynamic user = new
                        {
                            Id = 1,
                            Name = "Claes-Philip",
                            Email = "Claes-Philip@Staiger.se",
                        };

                        if (validated) //todo addd validation
                        {
                            // create identity
                            var id = new ClaimsIdentity(context.Options.AuthenticationType);
                            id.AddClaim(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(user)));
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
                    else
                    {
                        var ex = new System.Security.Authentication.AuthenticationException(string.Format("User '{0}' is not a member of '{1}'", context.UserName, groupname));
                        //log.Error("", ex);
                        context.Rejected();
                    }
                }
                else
                {
                    var ex = new System.Security.Authentication.AuthenticationException(string.Format("User '{0}' not found", context.UserName));
                    //log.Error("", ex);
                    context.Rejected();
                }
            }


        }
    }
}