﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using System.Xml.Linq;
using ActionFramework.Enum;
using ActionFramework.Classes;
//using ActionFramework.Context;
using ActionFramework.Model;
using ActionFramework.Extensions;
using ActionFramework.Reflections;

namespace ActionFramework.Agent.DataSource
{
    public class XmlDataSource : IActionDataSource
    {
        private string xml;

        private List<Model.App> apps = new List<Model.App>();

        public List<Model.App> Apps
        {
            get { return apps; }
            set { apps = value; }
        }
      
        public string Xml
        {
            get { return xml; }
            set { xml = value; }
        }

        public XmlDataSource(string xml)
        {
            this.xml = xml;
        }

        public virtual List<IAction> GetActions(ActionStatus status)
        {
            List<XElement> actionElements = ActionHelper.GetActionElements(status, xml);
            List<XElement> settingElements = ActionHelper.GetSettingElements(xml);
            List<IAction> actionList = new List<IAction>();
            Type[] actionTypes = ReflectionHelper.GetActionTypes(settingElements);

            foreach (XElement e in actionElements)
            {
                try
                {
                    var property = ActionHelper.GetActionProperty(e, "Type").Value;

                    if(property == null)
                        throw new Exception("Action Property '" + e.Name + "' could not be found");

                    Type actionType = ReflectionHelper.GetActionType(actionTypes, property);
                    IAction action = (IAction)System.Activator.CreateInstance(actionType);
                    action.Id = e.Attribute("Id").Value;
                    action.Type = actionType;
                    action.Assembly = actionType.Assembly;
                    action.Description = e.Attribute("Description").Value;
                    action.AddDynamicProperties(ActionHelper.GetActionProperties(e));
                    action.AddDynamicProperties(ActionHelper.GetSettingProperties(settingElements));

                    if (e.Attribute("BreakOnError") != null)
                        action.BreakOnError = bool.Parse(e.Attribute("BreakOnError").Value);

                    if (e.Attribute("ClientExecute") != null)
                        action.ClientExecute = Convert.ToBoolean(e.Attribute("ClientExecute").Value);

                    //action.DataSource = this;
                    actionList.Add(action);
                }
                catch
                {
                    throw new Exception("GetActions caused an exception in XmlDataSource class. Assembly could not be found for type: '" + e.Attribute("Type").Value + "'");
                    //LogContext.Current().Add(LogType.Error, ex);
                    //throw ex;
                }
            }

            //resolve the properties after list is done, due to that properties values might depend on execution of others
            foreach (var a in actionList)
            {
                //a.DataSource.ActionList = actionList;
                a.ResolveStaticProperties();
            }

            return actionList;
        }

        public List<ActionProperty> GlobalSettings
        {
            get {

                List<XElement> settingElements = ActionHelper.GetSettingElements(xml);
                return ActionHelper.GetSettingProperties(settingElements);
            }
        }

        public void FillActions(IActionList actionList, ActionStatus status)
        {
            List<XElement> actionElements = ActionHelper.GetActionElements(status, xml);
            List<XElement> settingElements = ActionHelper.GetSettingElements(xml);
            
            Type[] actionTypes = ReflectionHelper.GetActionTypes(settingElements);

            foreach (XElement e in actionElements)
            {
                Type actionType = ReflectionHelper.GetActionType(actionTypes, ActionHelper.GetActionProperty(e, "Type").Value);

                try
                {
                    
                    IAction action = (IAction)System.Activator.CreateInstance(actionType);
                    action.Id = e.Attribute("Id").Value;
                    action.Type = actionType;
                    action.Assembly = actionType.Assembly;
                    action.Description = e.Attribute("Description").Value;
                    action.AddDynamicProperties(ActionHelper.GetActionProperties(e));
                    action.AddDynamicProperties(ActionHelper.GetSettingProperties(settingElements));

                    if (e.Attribute("BreakOnError") != null)
                        action.BreakOnError = bool.Parse(e.Attribute("BreakOnError").Value);

                    if (e.Attribute("ClientExecute") != null)
                        action.ClientExecute = Convert.ToBoolean(e.Attribute("ClientExecute").Value);

                    //action.DataSource = this;
                    actionList.Add(action);
                }
                catch (Exception ex)
                {
                    //todo throw a more detailed log.
                    var message = string.Format("Failed to create an instance of the IAction Type: '{0}'", actionType.FullName);
                    throw new Exception(message + ". " + ex.Message, ex.InnerException);
                }
            }

            //resolve the properties after list is done, due to that properties values might depend on execution of others
            foreach (var a in actionList)
            {
                a.ActionList = actionList;
                a.ResolveStaticProperties();
            }
        }
    }
}
