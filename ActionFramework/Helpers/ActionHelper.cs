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
using ActionFramework.Model;

namespace ActionFramework.Classes
{
    public static class ActionHelper
    {
        public static bool loadFromFile = false;

        

        public static string RemoveSpecialCharacters(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            str = rgx.Replace(str, "");

            return str;
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



        //private static XDocument GetXml(string xml)
        //{
        //    if (loadFromFile)
        //        return XDocument.Load(xml);
        //    else
        //        return XDocument.Parse(xml);
        //}

        private static bool ActionTypeFilter(Type m, object filterCriteria)
        {
            return m.ToString().Equals(filterCriteria.ToString());
        }

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
