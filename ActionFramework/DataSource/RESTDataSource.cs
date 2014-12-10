using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using ActionFramework.Entities;
using System.Xml.Linq;
using ActionFramework.Enum;
using ActionFramework.Classes;
using ActionFramework.Domain.Model;
using System.Configuration;
using RestSharp;
using ActionFramework.Extensions;
using ActionFramework.Context;
using ActionFramework.Model;

namespace ActionFramework.DataSource
{
    public class RESTDataSource : IActionDataSource
    {
        private Agent agent;
        private List<ActionProperty> globalSettings = new List<ActionProperty>();
        private List<App> apps = new List<App>();

        public List<ActionProperty> GlobalSettings
        {
            get { 
           
                globalSettings.Clear();

                foreach (var s in agent.Settings)
                {
                    globalSettings.Add(new ActionProperty(s.Name, s.Value));
                }

                return globalSettings;
            }
            set { globalSettings = value; }
        }

        public List<App> Apps
        {
            get { return apps; }
            set { apps = value; }
        }

        public RESTDataSource()
        {
            this.agent = GetAgent();
        }

        public RESTDataSource(string xml)
        {
            this.agent = GetAgent(xml);
        }

        public virtual void FillActions(IActionList actionList, ActionStatus status)
        {
            globalSettings = new List<ActionProperty>();

            //add the apps for installation
            //InstallApps(agent.Apps.ToList());

            foreach (var s in agent.Settings)
            {
                globalSettings.Add(new ActionProperty(s.Name, s.Value));
            }

            //todo: we provide the agent.Apps here //agent.InstalledApps
            
            //Type[] actionTypes = ActionHelper.GetActionTypes(globalSettings);
            Type[] actionTypes = ActionHelper.GetActionTypes(globalSettings, agent.Apps.ToList());

            foreach (var a in agent.Actions)
            {
                try
                {
                    Type actionType = ActionHelper.GetActionType(actionTypes, a.Type.Trim());

                    IAction action = (IAction)Activator.CreateInstance(actionType);

                    //action.ActionListMember = actionListMember;

                    action.Id = a.Id.ToString();
                    action.Type = actionType;
                    action.Assembly = actionType.Assembly;
                    action.BreakOnError = a.BreakOnError;
                    action.Description = a.Description;
                    action.ClientExecute = a.ClientExecute;
                    //action.DataSource = this;
                    List<ActionProperty> properties = new List<ActionProperty>();

                    foreach (var p in a.Properties)
                    {
                        properties.Add(new ActionProperty(p.Name, p.Value));
                    }

                    //TODO, redesign model and entities for Action Framework and Resources
                    foreach (var r in a.Resources)
                    {
                        //TODO: map the resource from db
                        //action.AddResource(r);
                    }

                    action.AddDynamicProperties(properties);
                    action.AddDynamicProperties(globalSettings);
                    actionList.Add(action);
                }
                catch (Exception ex)
                {
                    throw ex; //new Exception("GetActions caused an exception in RESTDataSource class. Assembly could not be found for type: '" + a.Type + "'");
                    //LogContext.Current().Add(LogType.Error, ex);
                    //throw ex;
                }
            }

            //resolve the properties after list is done, due to that properties values might depend on execution of others
            foreach (var a in actionList)
            {
                a.ActionList = actionList;
                a.ResolveStaticProperties();
            }

        }
        
        public virtual List<IAction> GetActions(ActionStatus status)
        {
            List<IAction> actionList = new List<IAction>();
            globalSettings = new List<ActionProperty>();

            //add the apps for installation
            //InstallApps(agent.Apps.ToList());

            foreach (var s in agent.Settings)
            {
                globalSettings.Add(new ActionProperty(s.Name, s.Value));
            }

            Type[] actionTypes = ActionHelper.GetActionTypes(globalSettings);

            foreach (var a in agent.Actions)
            {
                try
                {
                    Type actionType = ActionHelper.GetActionType(actionTypes, a.Type);

                    IAction action = (IAction)Activator.CreateInstance(actionType);
                    
                    //action.ActionListMember = actionListMember;

                    action.Id = a.Id.ToString();
                    action.Type = actionType;
                    action.Assembly = actionType.Assembly;
                    action.BreakOnError = a.BreakOnError;
                    action.Description = a.Description;
                    action.ClientExecute = a.ClientExecute;
                    //action.DataSource = this;
                    List<ActionProperty> properties = new List<ActionProperty>();

                    foreach (var p in a.Properties)
                    {
                        properties.Add(new ActionProperty(p.Name, p.Value));
                    }

                    //foreach (var r in a.Resources)
                    //{
                    //    action.AddResource(r);
                    //}

                    action.AddDynamicProperties(properties);
                    action.AddDynamicProperties(globalSettings);
                    actionList.Add(action);
                }
                catch
                {
                    throw new Exception("GetActions caused an exception in RESTDataSource class. Assembly could not be found for type: '" + a.Type + "'");
                    //LogContext.Current().Add(LogType.Error, ex);
                    //throw ex;
                }
            }

            //resolve the properties after list is done, due to that properties values might depend on execution of others
            foreach(var a in actionList)
            {
                a.ResolveStaticProperties();
            }

            return actionList;
        }

        private Agent GetAgent()
        {
            var url = string.Format("{0}/{1}/{2}/{3}", "agent","getagent", AgentConfigurationContext.Current.AgentCode, AgentConfigurationContext.Current.AgentSecret);
            RestHelper req = new RestHelper(url, Method.GET);
            req.AddHeader("Accept", "application/xml");
            var response = req.Execute();
            return Serializer.Deserialize<ActionFramework.Domain.Model.Agent>(response.Content);
        }

        private Agent GetAgent(string xml)
        {
            if (xml.EndsWith(".xml"))
            {
                //xml is a reference path to xml, load it and parse it
                return Serializer.Deserialize<ActionFramework.Domain.Model.Agent>(XDocument.Load(xml).ToString());
            }
            else
            {
                //xml is an xml string, just parse it
                return Serializer.Deserialize<ActionFramework.Domain.Model.Agent>(xml);
            }
        }
    }
}
