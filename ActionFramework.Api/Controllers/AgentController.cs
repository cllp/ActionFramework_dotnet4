using ActionFramework.Domain.Model;
using ActionFramework.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using RestSharp;
using ActionFramework.Classes;

namespace ActionFramework.Api.Controllers
{
    [RoutePrefix("agent")]
    public class AgentController : BaseController
    {
        [Route("getserverinfo/{uri}")]
        [HttpGet]
        [AcceptVerbs("GET")]
        public SystemInformationModel GetServerInfo(string uri)
        {
            RestHelper req = new RestHelper(uri.Trim() + "GetSystemInformation", Method.GET);
            req.AddHeader("Accept", "application/xml");
            var response = req.Execute();

            Dictionary<string, object> returnValue = new Dictionary<string, object>();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {

            }

            return Serializer.Deserialize<SystemInformationModel>(response.Content);
        }

        [Route("getagent/{id}")]
        [HttpGet]
        public AgentModel GetAgent(int agentId)
        {
            var agent = agentRep.GetById(agentId);
            var apps = appRep.GetByAgentId(agentId);

            //Mapper.CreateMap<Agent, AgentModel>();

            //AgentModel model = Mapper.Map<AgentModel>(agent);

            List<SettingModel> settings = new List<SettingModel>();
            List<AppModel> appModels = new List<AppModel>();

            foreach (var setting in agent.Settings)
            {
                var settingModel = new SettingModel();
                settingModel.AgentId = setting.AgentId;

                settingModel.DataType = setting.DataType.Trim();
                settingModel.Id = setting.Id;
                settingModel.Name = setting.Name.Trim();
                settingModel.Value = setting.Value.Trim();
                settings.Add(settingModel);
            }

            foreach (var app in apps)
            {
                AgentAppModel agentAppModel = null;

                if (app.AgentApp != null)
                {
                    agentAppModel = new AgentAppModel();
                    agentAppModel.AgentId = app.AgentApp.AgentId;
                    agentAppModel.AppId = app.AgentApp.AppId.Value;
                    agentAppModel.InstallDate = app.AgentApp.InstallDate.Value;
                    agentAppModel.Installed = app.AgentApp.Installed;
                }

                var appModel = new AppModel();
                appModel.AssemblyInfo = app.AssemblyInfo;
                appModel.Icon = app.Icon;
                appModel.Id = app.Id;
                appModel.Name = app.Name;
                appModel.OrganizationId = app.OrganizationId;
                appModel.Version = app.Version;
                appModel.AgentApp = agentAppModel;
                appModels.Add(appModel);
            }

            var availableApps = appModels.Where(a => a.AgentApp == null || a.AgentApp.Installed.Equals(false));
            var installedApps = appModels.Where(a => a.AgentApp != null && a.AgentApp.Installed.Equals(true));

            OrganizationModel organization = new OrganizationModel();
            organization.Address = agent.Organization.Address.Trim();
            organization.Id = agent.Organization.Id;
            organization.Name = agent.Organization.Name.Trim();
            organization.Phone = agent.Organization.Phone.Trim();

            var model = new AgentModel();
            model.Application = agent.Application.Trim();
            model.CurrentStatus = "Initializing...";
            model.CurrentInterval = string.Empty;
            model.Id = agent.Id;
            model.Notes = agent.Notes.Trim();
            model.ServiceUrl = agent.ServiceUrl.Trim();
            model.Type = agent.Type.Trim();
            model.Version = agent.Version.Trim();
            model.Settings = settings;
            model.AvailableApps = availableApps;
            model.InstalledApps = installedApps;
            model.Organization = organization;
            model.SystemInformation = ActionFramework.Classes.Serializer.Deserialize<SystemInformationModel>(agent.SystemInfo);
            model.Apps = appModels;
            return model;
        }

        [AcceptVerbs("GET", "POST")]
        [Route("getagent/{code}/{secret}")]
        public Agent Get(string code, string secret)
        {
            return actionRep.GetAgentActions(code, secret);
        }

        [Route("getagents")]
        [HttpGet]
        public IEnumerable<AgentModel> GetAgents()
        {
            throw new NotImplementedException();
            //todo: create a new method for agentlist based on current user
            //var agents = actionRep.GetUserData(CurrentUser);

            //List<AgentModel> agentModels = new List<AgentModel>();

            //foreach (var a in agents.Organization.Agents)
            //{
            //    var agent = new AgentModel();
            //    agent.Application = a.Application;
            //    agent.CurrentStatus = "Initializing...";
            //    agent.CurrentInterval = string.Empty;
            //    agent.Id = a.Id;
            //    agent.Notes = a.Notes;
            //    agent.ServiceUrl = a.ServiceUrl;
            //    agent.Type = a.Type;
            //    agent.Version = a.Version;
            //    agentModels.Add(agent);
            //}

            //return agentModels;
        }

        [Route("getapps/{agentid}")]
        [HttpGet]
        public IEnumerable<AppModel> GetApps(int agentId)
        {
            var apps = appRep.GetByAgentId(agentId);

            List<AppModel> appModels = new List<AppModel>();

            foreach (var a in apps)
            {
                var app = new AppModel();
                app.AssemblyInfo = a.AssemblyInfo;
                app.Icon = a.Icon;
                app.Id = a.Id;
                app.Name = a.Name;
                app.OrganizationId = a.OrganizationId;
                app.Version = a.Version;

                AgentAppModel agentAppModel = null;

                if (a.AgentApp != null)
                {
                    agentAppModel = new AgentAppModel();
                    agentAppModel.AgentId = a.AgentApp.AgentId;
                    agentAppModel.AppId = a.AgentApp.AppId.Value;
                    agentAppModel.InstallDate = a.AgentApp.InstallDate.Value;
                    agentAppModel.Installed = a.AgentApp.Installed;
                    app.AgentApp = agentAppModel;
                }

                appModels.Add(app);
            }

            return appModels;
        }

        [Route("getagentschedule/{uri}")]
        [HttpGet]
        public Dictionary<string, object> GetAgentSchedule(string uri)
        {
            RestHelper req = new RestHelper(uri.Trim() + "TimerInfo", Method.GET);
            req.AddHeader("Accept", "application/xml");
            var response = req.Execute();
           
            Dictionary<string, object> returnValue = new Dictionary<string, object>();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                returnValue.Add("Interval", "-1");
                returnValue.Add("Status", "Unavailable");
                return returnValue;
            }

            XDocument xDoc = XDocument.Parse(response.Content);
            var intervalElement = xDoc.Elements().First();

            if (intervalElement.Value.Equals("0"))
            {
                returnValue.Add("IntervalStatus", -1);
                returnValue.Add("Status", "Stopped");
            }
            else
            {
                returnValue.Add("IntervalStatus", 1);
                returnValue.Add("Status", "Runs");
            }

            returnValue.Add("Interval", intervalElement.Value);

            return returnValue;
        }
    }
}