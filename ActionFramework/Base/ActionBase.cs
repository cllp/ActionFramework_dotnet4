using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using System.Reflection;
using ActionFramework.Classes;
using ActionFramework.Events;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ActionFramework.Context;
using ActionFramework.Enum;
using ActionFramework.Logging;
using ActionFramework.Model;

namespace ActionFramework.Base
{
    [System.ComponentModel.DefaultEvent("ExecuteEvent")]
    public abstract class ActionBase : IAction
    {
        private string id;
        private IActionList actionList;
        //private IActionDataSource dataSource;
        private bool enabled;
        private bool clientExecute = true;
        private string description;
        private Type type;
        private Assembly assembly;
        private string status;
        private bool breakOnError = false;
        private List<ActionProperty> dynamicProperties;
        private List<ResourceParameter> resources = new List<ResourceParameter>();
        public delegate void InitEventHandler(object sender, InitEventArgs e);
        public event InitEventHandler Initialize;
        private ICommon common;
        //public delegate void StartEventHandler(object sender, StartEventArgs e);
        //public event StartEventHandler BeforeStart;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public IActionList ActionList
        {
            get { return actionList; }
            set { actionList = value; }
        }

        //public IActionDataSource DataSource
        //{
        //    get { return dataSource; }
        //    set { dataSource = value; }
        //}

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public bool ClientExecute
        {
            get { return clientExecute; }
            set { clientExecute = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        public Assembly Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public bool BreakOnError
        {
            get { return breakOnError; }
            set { breakOnError = value; }
        }

        public List<ActionProperty> DynamicProperties
        {
            get { return dynamicProperties; }
            set { dynamicProperties = value; }
        }

        public List<ResourceParameter> Resources
        {
            get 
            {
                resources = resources.Concat(this.actionList.Resources.ToList()).ToList();
                return resources;
            }
            set { resources = value; }
        }

        public ILogger Log
        {
            get
            {
                return LogContext.Current(this);
            }
        }

        public ISystemLogger EventLogger
        {
            get
            {
                return ActionFactory.EventLogger();
            }
        }

        public ICommon Common
        {
            get { return common; }
        }

        public ActionBase()
        {
            dynamicProperties = new List<ActionProperty>();
            common = new Common();
            //event
            OnInitialize(new InitEventArgs(this));
            //BeforeStart(this, new StartEventArgs());
        }

        void OnInitialize(InitEventArgs e)
        {
            if (Initialize != null) Initialize(this, e);
        }

        public void AddDynamicProperty(ActionProperty property)
        {
            dynamicProperties.Add(property);
        }

        public void AddDynamicProperties(List<ActionProperty> properties)
        {
            foreach (ActionProperty property in properties)
            {
                if (dynamicProperties.Find(p => p.Name.Equals(property.Name)) != null)
                {
                    string message = string.Format("Error when adding dynamic properties. The property '{0}' is defined more than once for action '{1}'", property.Name, this.Type);
                    var ex = new Exception(message);
                    Log.Error(ex);
                    throw ex;
                }

                dynamicProperties.Add(property);
            }
        }

        public void AddResource(ResourceParameter resource)
        {
            resources.Add(resource);
        }

        public void AddResources(List<ResourceParameter> resources)
        {
            foreach (ResourceParameter resource in resources)
                resources.Add(resource);
        }

        public string Prop(string name)
        {
            try
            {
                return ReplaceVariableWithPropertyValue(dynamicProperties.Find(o => o.Name == name).Value);
            }
            catch (Exception ex)
            {
                var msg = "The dynamic property '" + name + "' for object type: '" + this.Type.ToString() + "' with id: '" + this.id + "' is not configured. ";
                
                //warn only if this is executed from client. If executed from another action, its ok for the property to be null before execution
                if(this.clientExecute)
                {
                    Log.Warning(msg, new Exception(ex.Message, ex.InnerException));
                }
                else
                {
                    Log.Info(msg);
                }
                return null; //throw ex;
            }
        }

        //public byte[] Resource(string name)
        //{
        //    try
        //    {
        //        //System.Text.Encoding.UTF8.GetString(file)
        //        return ActionFactory.Compression.DecompressFile(resources.Find(o => o.Name == name).Object);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = "Could not find resource value with name: '" + name + "' for object type: '" + this.Type.ToString() + "' with id: '" + this.id + "'. " + ex.Message;
        //        Log.Error(new Exception(msg, ex.InnerException));
        //        throw ex;
        //    }
        //}

        public ResourceParameter Resource(string name)
        {
            try
            {
                return resources.Find(o => o.FileName.Trim() == name);
                //return System.Text.Encoding.UTF8.GetString(ActionFactory.Compression.DecompressFile(.Object));
            }
            catch (Exception ex)
            {
                var msg = "Could not find resource value with name: '" + name + "' for object type: '" + this.Type.ToString() + "' with id: '" + this.id + "'. " + ex.Message;
                Log.Error(new Exception(msg, ex.InnerException));
                throw ex;
            }
        }

        public IAction FindActionByType(string type)
        {
            var exceptionMessage = "Could not find action value with type: '" + type + "'. Ensure that it is in the configuration";

            try
            {

                var action = this.ActionList.Where(a => a.Type.Name.Equals(type)).FirstOrDefault();

                if (action != null)
                { 
                    return action;
                }
                else
                { 
                    throw new Exception(exceptionMessage);
                }
                //return this.DataSource.ActionList.Where(a => a.Type.Name.Equals(type)).FirstOrDefault();
            }
            catch (Exception ex)
            {
               
                Log.Error(new Exception(ex.Message, ex.InnerException));
                throw ex;
            }
        }

        public IAction FindActionById(string id)
        {
            var exceptionMessage = "Could not find action value with id: '" + id + "'. Ensure that it is in the configuration";

            try
            {

                var action = this.ActionList.Where(a => a.Id.Equals(id)).FirstOrDefault();

                if (action != null)
                {
                    return action;
                }
                else
                {
                    throw new Exception(exceptionMessage);
                }
                //return this.DataSource.ActionList.Where(a => a.Type.Name.Equals(type)).FirstOrDefault();
            }
            catch (Exception ex)
            {

                Log.Error(new Exception(ex.Message, ex.InnerException));
                throw ex;
            }
        }

        public object Invoke(object instance, string method, object[] parameters)
        {
            try
            {
                return this.common.InvokeMethod(instance, method, parameters);
            }
            catch
            {
                Log.Error(new Exception("Could not invoke method: '" + method + "' in instance: '" + instance.GetType().Name));
                return null;
            }
        }

        public object Invoke(object instance, string method)
        {
            try
            {
                return this.common.InvokeMethod(instance, method);
            }
            catch
            {
                Log.Error(new Exception("Could not invoke method: '" + method + "' in instance: '" + instance.GetType().Name));
                return null;
            }
        }

        public virtual object Execute()
        {
            throw new NotImplementedException(string.Format(Constants.ExecuteNotImplementedMessage, "Execute()", this.Type.ToString(), this.Id));
        }

        public virtual object Execute(out object output)
        {
            throw new NotImplementedException(string.Format(Constants.ExecuteNotImplementedMessage, "Execute(out object output)", this.Type.ToString(), this.Id));
        }

        public virtual object Execute(object input)
        {
            throw new NotImplementedException(string.Format(Constants.ExecuteNotImplementedMessage, "Execute(object input)", this.Type.ToString(), this.Id));
        }

        public virtual object Execute(object input, out object output)
        {
            throw new NotImplementedException(string.Format(Constants.ExecuteNotImplementedMessage, "Execute(object input, out object output)", this.Type.ToString(), this.Id));
        }

        public virtual object Execute(params object[] parameters)
        {
            throw new NotImplementedException(string.Format(Constants.ExecuteNotImplementedMessage, "Execute(params object[] parameters)", this.Type.ToString(), this.Id));
        }

        public virtual object Execute(out object output, params object[] parameters)
        {
            throw new NotImplementedException(string.Format(Constants.ExecuteNotImplementedMessage, "out object output, params object[] parameters)", this.Type.ToString(), this.Id));
        }

        public string HandleException(Exception ex)
        {
            this.status = Constants.ExceptionMessage + ". Message: " + ex.Message;
            Log.Error("Status: " + this.status, ex);
            throw ex;
        }

        public string HandleSuccess()
        {
            status = Constants.SuccessMessage;
            Log.Info("Status: " + Constants.SuccessMessage);
            return status;
        }

        public string HandleSuccess(string extraInfo)
        {
            status = Constants.SuccessMessage + extraInfo;
            Log.Info("Status: " + Constants.SuccessMessage + ". Extrainfo: " + extraInfo);
            return status;
        }

        public string ReplaceVariableWithPropertyValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception("Error in function 'ReplaceVariableWithPropertyValue'. The value is null.");

            var switchvalue = string.Empty;

            if (value.Contains("|"))
                switchvalue = value.Split('|')[0];

            switch (switchvalue)
            {
                case "Invoke":
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
                            Log.Error("Internal Invoke cased an exception. Could not Invoke with value: '" + value + "'.", ex);
                            return null;
                        }
                        //break;
                    }
                case "Exec":
                    {
                        string[] execvalue = value.Split('|');
                        var anotheraction = FindActionById(execvalue[1]);
                        var result = anotheraction.Execute().ToString();
                        return result;
                    }
                default:
                    {
                        try
                        {
                            List<string> variables = common.GetVariables(value);
                            if (variables.Count > 0)
                            {
                                string newValue = value;

                                foreach (string var in variables)
                                {
                                    string propertyValue = Prop(var);
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
                        //break;
                    }
            }
        }

        //private List<string> GetVariables(string value)
        //{
        //    List<string> variables = new List<string>();
        //    var pattern = @"\{(.*?)\}";

        //    var matches = Regex.Matches(value, pattern);

        //    foreach (Match m in matches)
        //    {
        //        if (!string.IsNullOrEmpty(m.Groups[1].Value))
        //            variables.Add(m.Groups[1].Value);
        //    }

        //    return variables;
        //}

        public List<string> GetVariables(string value, string pattern)
        {
            List<string> variables = new List<string>();
            //var pattern = @"\{(.*?)\}";

            var matches = Regex.Matches(value, pattern);

            foreach (Match m in matches)
            {
                if (!string.IsNullOrEmpty(m.Groups[1].Value))
                    variables.Add(m.Groups[1].Value);
            }

            return variables;
        }
    }
}
