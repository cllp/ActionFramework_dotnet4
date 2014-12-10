using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Entities;
using System.Xml.Linq;
using ActionFramework.Enum;
using System.Reflection;
using ActionFramework.Interfaces;
using System.IO;
using System.Text.RegularExpressions;
using ActionFramework.Domain.Model;
using System.Configuration;
using RestSharp;
using ActionFramework.Context;
using ActionFramework.Logging;
using ActionFramework.DataSource;
using ActionFramework.Model;

namespace ActionFramework.Classes
{
    public static class ActionHelper
    {
        public static bool loadFromFile = false;

        public static Type[] GetExecutingAssemblyTypes()
        {

            return Assembly.GetExecutingAssembly().GetTypes();

        }

        public static Type[] GetCallingAssemblyTypes()
        {

            return Assembly.GetCallingAssembly().GetTypes();

        }

        public static Assembly[] GetDomainAssemblies()
        {

            return AppDomain.CurrentDomain.GetAssemblies();

        }

        public static Assembly[] GetCustomAssemblies(string customAssemblyPath)
        {
            List<Assembly> assemblies = new List<Assembly>();
            if (!Directory.Exists(customAssemblyPath))
                throw new Exception("Path does not exists!");

            DirectoryInfo di = new DirectoryInfo(customAssemblyPath);
            FileInfo[] rgFiles = di.GetFiles("*.dll");

            foreach (FileInfo fi in rgFiles)
            {
                Assembly a = Assembly.LoadFrom(fi.FullName);
                assemblies.Add(a);
            }

            return assemblies.ToArray();
        }

        public static string RemoveSpecialCharacters(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            str = rgx.Replace(str, "");

            return str;
        }

        public static Assembly[] GetCustomAssemblies(string customAssemblyPath, List<App> appList)
        {
            List<Assembly> assemblies = new List<Assembly>();
            if (!Directory.Exists(customAssemblyPath))
                throw new Exception("Path does not exists!");

            DirectoryInfo di = new DirectoryInfo(customAssemblyPath);
            FileInfo[] rgFiles = di.GetFiles("*.dll");

            //TODO get only files dll that is included in as installed apps
            //i think we can check by name even if version is included in the physical filename

            foreach (FileInfo fi in rgFiles) //.Where(f => f.Name.IsInList())
            {
                if (appList.Any(x => string.Format(Constants.AssemblyFileName, x.Name.Trim(), x.Version.Trim()).Equals(fi.Name)))
                {
                    Assembly a = Assembly.LoadFrom(fi.FullName);
                    assemblies.Add(a);
                }
            }

            return assemblies.ToArray();
        }

        public static Type GetActionType(Type[] types, string name)
        {
            return (from t in types where t.Name.Equals(name.Trim()) select t).FirstOrDefault();
        }

        public static Type[] GetActionTypes(List<XElement> settingElements)
        {

            List<Type> types = new List<Type>();
            Type actionInterface = typeof(IAction);

            string customAssemblyPath = "";
            ActionProperty customXmlAssemblyPath = (from p in GetSettingProperties(settingElements) where p.Name.Equals(Constants.CustomAssemblyFolderName) select p).FirstOrDefault();

            string customExeAssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (customXmlAssemblyPath == null)
                customAssemblyPath = customExeAssemblyPath;
            else
            {
                if (string.IsNullOrEmpty(customXmlAssemblyPath.Value))
                    customAssemblyPath = customExeAssemblyPath;
                else
                    customAssemblyPath = customXmlAssemblyPath.Value;
            }

            Assembly[] customAssemblies = GetCustomAssemblies(customAssemblyPath);

            foreach (Assembly a in customAssemblies)
            {
                foreach (Type type in a.GetTypes())
                {
                    bool isAssignedFrom = IsActionType(type);
                    bool containsActionBase = type.Name.Contains("ActionBase");
                    bool containsIAction = type.Name.Contains("IAction");

                    if (isAssignedFrom && !containsIAction && !containsActionBase)
                        types.Add(type);
                }
            }

            return types.ToArray();
        }


        private static PropertyInfo[] GetPropertyInfoName(Type obj)
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = obj.GetProperties(BindingFlags.GetProperty);
            // sort properties by name
            Array.Sort(propertyInfos,
                    delegate(PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
                    { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            // write property names
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var info = propertyInfo.Name;
            }

            return propertyInfos;
        }

        /// <summary>
        /// here we can set the values of properties runtime
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(Type actionType, object instance, string propertyName, object value)
        {
            // create instance of class Action
            //object instance = Activator.CreateInstance(actionType); // set when it is already instanced
            // get info about property: public double Number
            PropertyInfo prop = actionType.GetProperty(propertyName);

            // set value of property
            prop.SetValue(instance, value, null);
        }

        public static PropertyInfo[] GetActionProperties(Type actionType)
        {
            return actionType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        }

        public static PropertyInfo[] GetAllActionProperties(Type actionType)
        {
            return actionType.GetProperties();
        }

        public static object InvokeMember(object instance, string method)
        {
            return instance.GetType().InvokeMember(method, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, instance, null);
        }

        public static List<Type> GetAssemblyActions(Assembly assembly)
        {
            List<Type> types = new List<Type>();
            Type actionInterface = typeof(IAction);

            foreach (Type type in assembly.GetTypes())
            {
                var props = GetPropertyInfoName(type);
                //properties
                MethodInfo[] methods = type.GetMethods();//.GetMethod("Method1");
                MemberInfo[] members = type.GetMembers();//.GetMethod("Method1");
                // Obtain a reference to the parameters collection of the MethodInfo instance.

                foreach (var mi in methods)
                {
                    ParameterInfo[] Params = mi.GetParameters();

                    foreach (var info in Params)
                    {
                        var iii = info.Member.Name;
                    }


                }


                bool isAssignedFrom = IsActionType(type);
                bool containsActionBase = type.Name.Contains("ActionBase");
                bool containsIAction = type.Name.Contains("IAction");

                if (isAssignedFrom && !containsIAction && !containsActionBase)
                    types.Add(type);
            }

            return types;
        }

        public static Type[] GetActionTypes(List<ActionProperty> settings)
        {
            List<Type> types = new List<Type>();
            Type actionInterface = typeof(IAction);

            string customAssemblyPath = "";
            ActionProperty customXmlAssemblyPath = (from p in settings where p.Name.Equals(Constants.CustomAssemblyFolderName) select p).FirstOrDefault();

            string customExeAssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (customXmlAssemblyPath == null)
                customAssemblyPath = customExeAssemblyPath;
            else
            {
                if (string.IsNullOrEmpty(customXmlAssemblyPath.Value))
                    customAssemblyPath = customExeAssemblyPath;
                else
                    customAssemblyPath = customXmlAssemblyPath.Value;
            }

            Assembly[] customAssemblies = GetCustomAssemblies(customAssemblyPath);

            foreach (Assembly a in customAssemblies)
            {
                types.AddRange(GetAssemblyActions(a));
            }

            return types.ToArray();
        }

        public static Type[] GetActionTypes(List<ActionProperty> settings, List<App> appList)
        {
            List<Type> types = new List<Type>();
            Type actionInterface = typeof(IAction);

            string customAssemblyPath = "";
            ActionProperty customXmlAssemblyPath = (from p in settings where p.Name.Equals(Constants.CustomAssemblyFolderName) select p).FirstOrDefault();

            string customExeAssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (customXmlAssemblyPath == null)
                customAssemblyPath = customExeAssemblyPath;
            else
            {
                if (string.IsNullOrEmpty(customXmlAssemblyPath.Value))
                    customAssemblyPath = customExeAssemblyPath;
                else
                    customAssemblyPath = customXmlAssemblyPath.Value;
            }

            Assembly[] customAssemblies = GetCustomAssemblies(customAssemblyPath, appList);

            foreach (Assembly a in customAssemblies)
            {
                types.AddRange(GetAssemblyActions(a));
            }

            return types.ToArray();
        }

        private static bool IsActionType(Type type)
        {
            System.Reflection.TypeFilter actionFilter = new System.Reflection.TypeFilter(ActionTypeFilter);
            Type[] interfaces = type.FindInterfaces(actionFilter, typeof(IAction));
            return interfaces.Count() > 0;
        }

        private static bool ActionTypeFilter(Type m, object filterCriteria)
        {
            return m.ToString().Equals(filterCriteria.ToString());
        }

        public static bool Match(string input, string pattern)
        {
            Regex regexMatch = new Regex(@pattern);
            Match m = regexMatch.Match(input);
            return m.Success;
        }

        public static string RegExReplace(string stringToReplace, string patternToReplace, string patternToReplaceWith)
        {
            return Regex.Replace(stringToReplace, EscapeAll(patternToReplace), patternToReplaceWith);
        }

        public static string EscapeAll(string pattern)
        {
            string fm_String = pattern;

            // Escape these characters
            // . $ ^ { [ ( | ) * + ? \

            // Always escape '\' first
            fm_String = fm_String.Replace("\\", "\\\\");
            fm_String = fm_String.Replace(".", "\\.");
            fm_String = fm_String.Replace("$", "\\$");
            fm_String = fm_String.Replace("^", "\\^");
            fm_String = fm_String.Replace("{", "\\{");
            fm_String = fm_String.Replace("[", "\\[");
            fm_String = fm_String.Replace("(", "\\(");
            fm_String = fm_String.Replace("|", "\\|");
            fm_String = fm_String.Replace(")", "\\)");
            fm_String = fm_String.Replace("*", "\\*");
            fm_String = fm_String.Replace("+", "\\+");
            fm_String = fm_String.Replace("?", "\\?");

            return fm_String;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string[] RegExSplit(string input, string pattern, RegexOptions option)
        {
            return System.Text.RegularExpressions.Regex.Split(input, pattern, option);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="filePath"></param>
        ///// <returns></returns>
        //public static string ReadTextFile(string filePath)
        //{
        //    StreamReader fp = new StreamReader(filePath, Encoding.Default);
        //    string filecontent = fp.ReadToEnd();
        //    fp.Close();
        //    return filecontent;
        //}

        public static List<XElement> GetActionElements(ActionStatus status, string xml)
        {
            //XDocument xmlFile = XDocument.Load(xmlPath);
            XDocument xmlFile = XDocument.Parse(xml);//GetXml(xml);
            List<XElement> elements = new List<XElement>();

            switch (status)
            {
                case ActionStatus.Enabled:
                    {
                        elements = (from s in xmlFile.Elements("ActionFramework").Elements("Actions").Elements("Action")
                                    where s.Attribute("Enabled").Value.ToLower().Equals("true")
                                    select s).ToList();
                        break;
                    }
                case ActionStatus.Disabled:
                    {
                        elements = (from s in xmlFile.Elements("ActionFramework").Elements("Actions").Elements("Action")
                                    where s.Attribute("Enabled").Value.ToLower().Equals("false")
                                    select s).ToList();
                        break;
                    }
                default:
                    {
                        elements = (from s in xmlFile.Elements("ActionFramework").Elements("Actions").Elements("Action")
                                    select s).ToList();
                        break;
                    }
            }

            return elements;
        }

        public static List<XElement> GetActionElements(ActionStatus status, string xml, string actionId)
        {
            //XDocument xmlFile = XDocument.Load(xmlPath);
            XDocument xmlFile = XDocument.Parse(xml);//GetXml(xml);
            List<XElement> elements = new List<XElement>();

            switch (status)
            {
                case ActionStatus.Enabled:
                    {
                        elements = (from s in xmlFile.Elements("ActionFramework").Elements("Actions").Elements("Action")
                                    where s.Attribute("Enabled").Value.ToLower().Equals("true")
                                    && s.Attribute("Id").Value.Equals(actionId)
                                    select s).ToList();
                        break;
                    }
                case ActionStatus.Disabled:
                    {
                        elements = (from s in xmlFile.Elements("ActionFramework").Elements("Actions").Elements("Action")
                                    where s.Attribute("Enabled").Value.ToLower().Equals("false")
                                    && s.Attribute("Id").Value.Equals(actionId)
                                    select s).ToList();
                        break;
                    }
                default:
                    {
                        elements = (from s in xmlFile.Elements("ActionFramework").Elements("Actions").Elements("Action")
                                    where s.Attribute("Id").Value.Equals(actionId)
                                    select s).ToList();
                        break;
                    }
            }

            return elements;
        }

        public static List<XElement> GetSettingElements(string xml)
        {
            XDocument xmlFile = XDocument.Parse(xml);//GetXml(xml); //XDocument.Load(xmlPath);
            List<XElement> elements = new List<XElement>();

            elements = (from s in xmlFile.Elements("ActionFramework").Elements("Settings").Elements("Setting")
                        select s).ToList();

            return elements;
        }

        public static ActionProperty GetActionProperty(XElement e, string name)
        {
            return new ActionProperty(name, e.Attribute(name).Value);
        }

        public static List<ActionProperty> GetActionProperties(XElement e)
        {
            List<ActionProperty> propList = new List<ActionProperty>();

            foreach (XElement propElement in e.Elements())
                propList.Add(new ActionProperty(propElement.Attribute("Name").Value, propElement.Attribute("Value").Value));

            return propList;
        }

        public static List<ActionProperty> GetSettingProperties(List<XElement> elements)
        {
            List<ActionProperty> prop = new List<ActionProperty>();

            foreach (XElement e in elements)
                prop.Add(new ActionProperty(e.Attribute("Name").Value, e.Attribute("Value").Value));

            return prop;
        }

        private static List<string> GetCustomAssemblyNames(string customAssemblyPath)
        {
            List<string> assemblies = new List<string>();
            if (!Directory.Exists(customAssemblyPath))
                throw new Exception("Path does not exists!");

            DirectoryInfo di = new DirectoryInfo(customAssemblyPath);
            FileInfo[] rgFiles = di.GetFiles("*.dll");

            foreach (FileInfo fi in rgFiles)
            {
                assemblies.Add(fi.Name);
            }

            return assemblies;
        }

        //private static XDocument GetXml(string xml)
        //{
        //    if (loadFromFile)
        //        return XDocument.Load(xml);
        //    else
        //        return XDocument.Parse(xml);
        //}

        public static bool InstallApp(int id, string name, string assembly)
        {
            GlobalActionFunctions gaf = new GlobalActionFunctions();
            var folder = gaf.GetExecutionRoot();

            LogContext.Current().Info("Installed '" + name.Trim() + "' assembly with id '" + id + "'");
            var file = ActionFactory.Compression.DecompressFile(assembly);
            File.WriteAllBytes(folder + name.Trim() + ".dll", file);

            return true;
        }

        public static string GetDirectoryPath()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            FileInfo fileInfo = new FileInfo(path);
            string dir = fileInfo.DirectoryName;
            return dir;
        }

        //public static IActionDataSource GetConfigurationDataSource(string xml)
        //{
        //    IActionDataSource dataSource;
        //    dataSource = new XmlDataSource(xml);
        //    return dataSource;
        //}

        //public static IActionDataSource GetConfigurationDataSource()
        //{
        //    return GetConfigurationDataSource(Path.Combine(ActionHelper.GetDirectoryPath(), AgentConfigurationContext.Current.ActionFile));
        //}

        //only used when installation is triggered from the configuration, now agent installation is pushed from web to client directly
        //public static string WriteUpdateInstalled(int appId, bool installed)
        //{
        //    var url = AgentConfigurationContext.Current.WebApiUrl + "/action/UpdateInstallApp/";//ConfigurationManager.AppSettings["WebApiUrl"] + "/action/UpdateInstallApp/";
        //    var client = new RestClient
        //    {
        //        BaseUrl = url
        //    };

        //    var request = new RestRequest();
        //    request.AddHeader("Accept", "application/xml");
        //    request.Method = Method.GET;

        //    request.AddParameter("agentCode", AgentConfigurationContext.Current.AgentCode);
        //    request.AddParameter("agentSecret", AgentConfigurationContext.Current.AgentSecret);
        //    request.AddParameter("appId", appId);
        //    request.AddParameter("installed", true);

        //    var response = client.Execute(request);

        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //        throw new Exception("Could not WriteUpdateInstalled through API. Statuscode: '" + response.StatusCode + "'. Message: " + response.Content);

        //    return response.StatusCode.ToString();
        //}
    }
}
