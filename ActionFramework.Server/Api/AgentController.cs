using ActionFramework.Classes;
using ActionFramework.Domain.Model;
using ActionFramework.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace ActionFramework.Server.Api
{
    [RoutePrefix("agent")]
    public class AgentController : BaseController
    {
        [Route("list")]
        [HttpGet]
        public HttpResponseMessage ListAgents()
        {
            List<object> agents = new List<object>();
            foreach(var dir in Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agents")))
            {
                string uri = File.ReadAllText(Path.Combine(dir, "uri.txt"));
                DirectoryInfo di = new DirectoryInfo(dir);
                agents.Add(new {
                    Name = di.Name,
                    Uri = uri
                });
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<object>(new
                {
                    agents
                }, Configuration.Formatters.JsonFormatter)
            };
        }

        [Route("uri/{agentid}")]
        [HttpGet]
        public HttpResponseMessage GetUri(string agentId)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agents", agentId);
            string uri = File.ReadAllText(Path.Combine(dir, "uri.txt"));

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(uri, Encoding.UTF8, "text/html")
            };
        }

        [HttpGet]
        [Route("config/{agent}/{file}")]
        public HttpResponseMessage Config(string agent, string file)
        {
            var path = string.Format("{0}/{1}/{2}/{3}/{4}", AppDomain.CurrentDomain.BaseDirectory, "Agents", agent, "Configuration", file);
            var doc = XDocument.Load(path);
            string compact = ActionFactory.Compression.CompressString(doc.Root.ToString());

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(compact, Encoding.UTF8, "text/html")
            };
        }

        [HttpGet]
        [Route("run/{agent}")]
        public HttpResponseMessage Config(string agent)
        {
            var path = string.Format("{0}/{1}/{2}/", AppDomain.CurrentDomain.BaseDirectory, "Agents", agent);
            string uri = File.ReadAllText(Path.Combine(path, "uri.txt"));

            RestHelper req = new RestHelper(uri.Trim() + "/run", "GET");
            var response = req.Execute();
            
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response.Content, Encoding.UTF8, "text/html")
            };
        }

        [Route("getserverinfo/{uri}")]
        [HttpGet]
        [AcceptVerbs("GET")]
        public SystemInformationModel GetServerInfo(string uri)
        {
            RestHelper req = new RestHelper(uri.Trim() + "GetSystemInformation", "GET");
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

        [AcceptVerbs("GET")]
        [Route("getagent/{id}")]
        public Agent Get(string id)
        {
            return actionRep.GetAgentActions(id);
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
            RestHelper req = new RestHelper(uri.Trim() + "TimerInfo", "GET");
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