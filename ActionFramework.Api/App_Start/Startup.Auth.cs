using ActionFramework.Api.Context;
using ActionFramework.Api.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace ActionFramework.Api
{
    public partial class Startup
    {
        static Startup()
        {
            OAuthBearerOptions = new OAuthAuthorizationServerOptions
            {
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                Provider = AuthFactory.Provider
            };

            var obo = new OAuthAuthorizationServerOptions();
        }

        public static OAuthAuthorizationServerOptions OAuthBearerOptions { get; private set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            //initialize the identitycontext
            //IdentityContext.Initialize(new FormsIdentity<User>());
            IdentityContext.Initialize(new Identity());

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthBearerOptions);

        }
    }
}