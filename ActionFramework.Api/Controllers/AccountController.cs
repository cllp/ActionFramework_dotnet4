using ActionFramework.Api.Attributes;
using ActionFramework.Api.Context;
using ActionFramework.Api.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Security;

namespace ActionFramework.Api.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : BaseController
    {
        public AccountController()
        {
            // Supress redirection for web services
            HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;
        }
        
        [HttpGet]
        [Route("profile")]
        [ApiAuthorize]
        public HttpResponseMessage Profile()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<object>(new
                {
                    IsAuthenticated = IdentityContext.Current.IsAuthenticated,
                    User = IdentityContext.Current.JsonUser,
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
                    IsAuthenticated = IdentityContext.Current.IsAuthenticated,
                    Message = "You are signed out"
                }, Configuration.Formatters.JsonFormatter)
            };
        }
    }
}