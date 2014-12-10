using ActionFramework.Domain.Model;
using ActionFramework.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using ActionFramework.Domain.Model.Api;

namespace ActionFramework.Api.Controllers
{
    [RoutePrefix("action")]
    public class ActionController : BaseController
    {
        [HttpGet]
        [Route("write/{text}")]
        public HttpResponseMessage Write(string text)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    text,
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }

        // GET api/<controller>
        //public IEnumerable<string> Get(string agentCode, string agentSecret)
        [Route("get/{agentcode}/{agentsecret}")] 
        [AcceptVerbs("GET", "POST")]
        [Route("Get/{code}/{secret}")]
        public Agent Get(string code, string secret)
        {
            var client = actionRep.GetAgentActions(code, secret);
            return client;
        }

        // GET api/<controller>
        //public IEnumerable<string> Get(string agentCode, string agentSecret)
        [Route("getbyagentid/{agentid}")]
        [AcceptVerbs("GET", "POST")]
        public IEnumerable<ActionModel> GetByAgentId(int agentId)
        {
            var viewModels = new List<ActionModel>();
            var dataModels = actionRep.GetActionsByAgentId(agentId);

            foreach (var dataModel in dataModels)
            {
                var m = new ActionModel();
                m.Id = dataModel.Id;
                m.AgentId = dataModel.AgentId;
                m.BreakOnError = dataModel.BreakOnError;
                m.ClientExecute = dataModel.ClientExecute;
                m.Description = dataModel.Description;
                m.Type = dataModel.Type;
                viewModels.Add(m);
            }

            return viewModels;
        }

        [Route("available/{agentId}")]
        [HttpGet]
        public IEnumerable<ActionModel> GetAvailableActions(int agentId)
        {
            var models = new List<ActionModel>();

            foreach (var action in appRep.GetAvailableActionsByAgentId(agentId))
            {
                var appModel = new AppModel();
                appModel.Id = action.App.Id;
                appModel.OrganizationId = action.App.OrganizationId;
                appModel.Name = action.App.Name;
                appModel.Version = action.App.Version;

                var model = new ActionModel();
                model.AgentId = action.AgentId;
                model.AppId = action.AppId.Value;
                model.Type = action.Type;
                model.App = appModel;
                models.Add(model);
            }

            return models;
        }

        //// GET api/<controller>
        ////public IEnumerable<string> Get(string agentCode, string agentSecret)
        //[Route("writelog/{logmodel}")]
        //[AcceptVerbs("GET", "POST", "PUT")]
        //public string WriteLog(LogModel logModel)  //(string agentCode, string agentSecret, string type, string description, string message)
        //{
        //    actionRep.WriteLog(logModel.AgentCode, logModel.AgentSecret, logModel.Type, logModel.Description, logModel.Message);
        //    return "";
        //}

        // GET api/<controller>
        //public IEnumerable<string> Get(string agentCode, string agentSecret)
        [Route("updateinstallapp/{agentcode}/{agentsecret}/{appid}/{installed}")]
        [AcceptVerbs("GET", "POST", "PUT")]
        public string UpdateInstallApp(string agentCode, string agentSecret, int appId, bool installed)
        {
            actionRep.UpdateInstallApp(agentCode, agentSecret, appId, installed);
            return "OK";
        }

        // GET api/<controller>
        //public IEnumerable<string> Get(string agentCode, string agentSecret)
        [Route("addresource/{resourcemodel}")]
        [AcceptVerbs("GET", "POST", "PUT")]
        public string AddResource(ResourceModel resourceModel)
        {
            actionRep.AddResource(resourceModel.AgentCode, resourceModel.AgentSecret, resourceModel.ActionId, resourceModel.Name, resourceModel.Type, resourceModel.Format, resourceModel.Obj);
            return "";
        }

        //[HttpPost]
        //public HttpResponseMessage Create(Domain.Model.Action model)
        //{
        //    int result = actionRep.Insert(model);
        //    Dictionary<string, object> returnValue = new Dictionary<string, object>();

        //    returnValue.Add("Status", "OK");
        //    returnValue.Add("Id", result);

        //    var resp = new HttpResponseMessage()
        //    {
        //        Content = new StringContent("Id: " + result)
        //    };

        //    resp.StatusCode = HttpStatusCode.OK;
        //    return resp;
        //}

        [Route("create/{model}")]
        [HttpPost]
        public ActionModel Create(ActionModel model)
        {
            var action = new Domain.Model.Action();

            action.AgentId = model.AgentId;
            action.AppId = model.AppId;
            action.BreakOnError = true; //model.BreakOnError;
            action.ClientExecute = model.ClientExecute;
            action.Enabled = true; //model.Enabled;
            action.Type = model.Type;

            int result = actionRep.Insert(action);
            model.Id = result;
            return model;
        }
    }
}