using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Security.AccessControl;
using System.Data.SqlClient;
using ActionFramework;
using ActionFramework.Interfaces;
using ActionFramework.Enum;
using System.Reflection;
using System.Text.RegularExpressions;
using ActionFramework.Context;
using ActionFramework.Logging;
using ActionFramework.Model;

namespace ActionFramework.Classes
{
    public class ActionList : List<IAction>, IActionList
    {
        private List<ActionProperty> globalSettings = new List<ActionProperty>();
        private Resources resources = new Resources();
        private ICommon common = new ActionFramework.Classes.Common();
        private IActionDataSource dataSource;
        private int agentExecute = 0;
        private int internalActionExecute = 0;

        public IActionDataSource DataSource
        {
            get { return dataSource; }
            //set { dataSource = value; }
        }

        public List<ActionProperty> GlobalSettings
        {
            get { return globalSettings; }
        }

        public Resources Resources
        {
            get { return resources; }
            set { resources = value; }
        }

        public ILogger Log
        {
            get 
            { 
                return LogContext.Current();
            }
        }

        public ActionList(IActionDataSource dataSource)
        {
            //this.AddRange(dataSource.GetActions(ActionStatus.Enabled));
            dataSource.FillActions(this, ActionStatus.Enabled);
            this.globalSettings = dataSource.GlobalSettings;
            
            //setting the actionlist on the datasource
            //dataSource.ActionList = this;
            this.dataSource = dataSource;
        }

        public string GlobalSetting(string name)
        {
            try
            {
                return ReplaceVariableWithPropertyValue(globalSettings.Find(o => o.Name == name).Value);
            }
            catch (Exception ex)
            {
                Log.Error(new Exception("Could not find global settingvalue with name: '" + name + "'." + ex.Message, ex.InnerException));
                throw ex;
            }
        }

        public string Prop(string name)
        {
            try
            {
                return ReplaceVariableWithPropertyValue(globalSettings.Find(o => o.Name == name).Value);
            }
            catch (Exception ex)
            {
                Log.Error(new Exception("Could not find global setting value with name: '" + name + "' in the action list. " + ex.Message, ex.InnerException));
                throw ex;
            }
        }

        public ActionResultLog Run(out string runtime)
        {
            DateTime start = DateTime.Now;

            foreach (var a in this)
            {
                try
                {
                    if (a.ClientExecute)
                    {
                        a.Execute();
                        agentExecute++;

                        if (AgentConfigurationContext.Current.Debug)
                        {
                            List<ActionProperty> lst = new List<ActionProperty>();
                            lst.AddRange(a.DynamicProperties);
                            var debug = new LogElements(LogType.Debug.ToString(), DateTime.Now, a);
                            debug.Add(new XmlLog(lst));
                            ActionFactory.CurrentLog().Add(debug);
                            //a.Log.Info(a.Resources);
                        }
                    }
                    else
                    {
                        internalActionExecute++;
                        a.Log.Info("'ClientExecute' property is disabled");     
                    }
                }
                catch (Exception ex)
                {
                    var status = String.Empty;

                    if (string.IsNullOrEmpty(a.Status))
                        status = "Exception. Message: " + ex.Message;
                    else
                        status = a.Status;

                    ActionFactory.EventLogger().Write(System.Diagnostics.EventLogEntryType.Error, "Error while executing action id '" + a.Id + "'" + ". Exception: " + ex.Message, Constants.EventLogId);
                    a.Log.Error("Error while executing action id '" + a.Id + "'");
                    a.Log.Error(ex);
                    if (a.BreakOnError)
                    {
                        a.Log.Info("Execution Aborted on Id: '" + a.Id + "'");
                        break;
                    }
                }
            }

            runtime = GetRunTime(start);

            var actionResult = new ActionResultLog("Action Results");
            actionResult.Total = this.Count;
            actionResult.AgentExecute = agentExecute;
            actionResult.InternalActionExecute = internalActionExecute;
            actionResult.Failed = (this.Count - agentExecute - internalActionExecute);
            actionResult.Runtime = runtime;

            if(AgentConfigurationContext.Current.Debug)
                actionResult.DataSource = this.dataSource;

            return actionResult;
            //return new ActionResultLog(string.Format(Constants.CountMessageText, this.Count, agentExecute.ToString(), internalActionExecute.ToString(), (this.Count - agentExecute - internalActionExecute).ToString()));
        }

        private string ReplaceVariableWithPropertyValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception("Error in function 'ReplaceVariableWithPropertyValue'. The value is null.");

            //TODO: enable invoke from any assembly
            if (value.StartsWith("Invoke"))
            {
                try
                {
                    GlobalActionFunctions gaf = new GlobalActionFunctions();

                    string[] invokes = value.Split('|');

                    if (invokes.Length > 2)
                    {
                        //calling it self to replace any variables
                        object[] par = ReplaceVariableWithPropertyValue(invokes[2]).Split(',');
                        string invFunction = ReplaceVariableWithPropertyValue(invokes[1]);

                        return common.InvokeMethod(gaf, invokes[1], par).ToString();
                    }
                    else
                    {
                        return common.InvokeMethod(gaf, invokes[1]).ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Internal Invoke caused an exception. Could not Invoke with value: '" + value + "'.");
                    Log.Error(ex);
                    return null;
                }
            }
            else
            {
                try
                {
                    List<string> variables = common.GetVariables(value);
                    if (variables.Count > 0)
                    {
                        string newValue = value;

                        foreach (string var in variables)
                        {
                            string propertyValue = GlobalSetting(var);
                            newValue = ActionHelper.RegExReplace(newValue, "{" + var + "}", propertyValue);
                        }

                        return newValue;
                    }
                    else
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Get property value caused an exception. Could not Invoke with value: '" + value + "'.", ex);
                    return null;
                }
            }
        }

        private static string GetRunTime(DateTime start)
        {
            TimeSpan ts = DateTime.Now.Subtract(start);
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

            return elapsedTime;
        }
    }
}
