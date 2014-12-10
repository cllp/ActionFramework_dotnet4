using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RestSharp;
using ActionFramework.Domain.Model;
using ActionFramework.Classes;


namespace ActionFramework.Api.Controllers
{
    [RoutePrefix("app")]
    public class AppController : BaseController
    {

        [Route("getapps/{agentid}")]
        [HttpGet]
        public IEnumerable<App> GetApps(int agentId)
        {
            return appRep.FindAll();//.GetById(agentId);
        }

        [Route("install/{agentid}/{appid}/{uri}")]
        [HttpGet]
        public IRestResponse Install(int agentId, int appId, string uri)
        {
            //1. get the app and assembly from the database
            var app = appRep.GetById(appId);

            //2. install the app to the agent with rest service call
            RestHelper req = new RestHelper(uri.Trim() + "Install", Method.GET);
            req.AddHeader("Accept", "application/xml");
            req.AddParameter("Assembly", app.Assembly);
            var response = req.Execute();
            //var client = new RestClient
            //{
            //    BaseUrl = url
            //};

            //var request = new RestRequest();
            //request.AddHeader("Accept", "application/xml");
            //request.Method = Method.GET;
            //request.AddParameter("Assembly", app.Assembly);

            //var response = req.Execute(request);
            
            //3. if rest service is ok then we update the database
            if(response.StatusCode == HttpStatusCode.OK)
                appRep.Install(agentId, appId, true);

            return response;
        }
    }
}
