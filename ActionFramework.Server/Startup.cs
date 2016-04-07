using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace ActionFramework.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.Run(async ctx =>
            //{
            //    //Console.WriteLine(ctx.Request.Uri.AbsoluteUri);
            //    //await ctx.TraceOutput(ctx.Request.Uri.AbsoluteUri);//.Response.WriteAsync("Server...");
            //});

            app.Map("/api", apiApp =>
            {
                var config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();
                apiApp.UseWebApi(config);
                apiApp.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            });

            //// Configure Web API for self-host. 
            //HttpConfiguration config = new HttpConfiguration();

            ////use routing from attributes
            //config.MapHttpAttributeRoutes();

            ////enable cors on owin server level
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            //app.UseWebApi(config);
        }
    }
}
