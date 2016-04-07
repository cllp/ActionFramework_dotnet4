using ActionFramework.Server.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ActionFramework.Server.Api
{
    [RoutePrefix("account")]
    public class AccountController : BaseController
    {
        [HttpGet]
        [Route("profile")]
        [ApiAuthorize]
        public HttpResponseMessage Profile()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<object>(new
                {
                    //IsAuthenticated = IdentityContext.Current.IsAuthenticated,
                    //User = IdentityContext.Current.JsonUser,
                }, Configuration.Formatters.JsonFormatter)
            };
        }

        [HttpGet]
        [Route("signedout")]
        public HttpResponseMessage SignedOut()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<object>(new
                {
                    //IsAuthenticated = IdentityContext.Current.IsAuthenticated,
                    //Message = "You are signed out"
                }, Configuration.Formatters.JsonFormatter)
            };
        }
    }
}