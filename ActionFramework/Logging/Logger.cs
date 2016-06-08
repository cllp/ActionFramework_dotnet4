using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using System.Xml.Linq;
using System.IO;
using RestSharp;
using System.Configuration;
using ActionFramework.Enum;
using ActionFramework.Classes;
using System.Xml;
using System.Reflection;
using ActionFramework.Context;
using Newtonsoft.Json;

namespace ActionFramework.Logging
{
    internal class Logger : ILogger
    {
        private IAction action;
        private List<LogElements> listOfElements = new List<LogElements>();
        private const string LogDescription = "ActionLog";

        public IAction Action
        {
            get { return action; }
            set { action = value; }
        }

        public List<LogElements> ListOfElements
        {
            get { return listOfElements; }
        }

        private XDocument CreateLogFile()
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", "iso-8859-1", "true"));
            XElement header = new XElement("ActionLog");
            header.Add(new XAttribute("Created", DateTime.Now));
            doc.Add(header);
            return doc;
        }

        //private string WriteLog(string log, string description)
        //{
        //    var model = new ActionFramework.Domain.Model.Api.LogModel();
        //    //model.AgentCode = AgentConfigurationContext.Current.AgentCode;
        //    //model.AgentSecret = AgentConfigurationContext.Current.AgentSecret;
        //    model.Description = description;
        //    model.Message = ActionFactory.Compression.CompressString(log);
        //    model.Type = GetLogTypes(log);

        //    RestHelper req = new RestHelper(AgentConfigurationContext.Current.ServerUrl + "/log/write/");
        //    req.AddBody(model);
        //    return req.Execute().StatusCode.ToString();
        //}

        private string GetLogTypes(string log)
        {
            var doc = XDocument.Parse(log);
            var sb = new StringBuilder();

            var logElement = doc.Element("ActionLog").Element("Log");

            foreach (var s in LogType.GetValues(typeof(LogType)))
            {
                if (logElement.Element(s.ToString()) != null)
                    sb.Append(s.ToString() + ",");
            }

            var logtypes = sb.ToString();

            if (string.IsNullOrEmpty(logtypes))
                return string.Empty;

            return logtypes.Remove(logtypes.Length - 1, 1);
        }

        private LogElements FindCurrentActionElements(string type, out bool found)
        {
            found = false;
            LogElements elements = ListOfElements.Find(e => e.Action.Id.Equals(this.Action.Id));

            if (elements == null)
            {
                //setting default Action to elements if the action is not null
                if (this.action != null)
                    elements = new LogElements("Action", DateTime.Now, this.action);
                else
                    elements = new LogElements(type, DateTime.Now, this.action);
            }
            else
                found = true;

            return elements;
        }

        public void Info(string s)
        {
            bool found = false;
            var elements = FindCurrentActionElements(LogType.Information.ToString(), out found);
            elements.Add(new InformationLog(s));

            if (!found)
                ListOfElements.Add(elements);
        }

        public void Warning(string s)
        {
            bool found = false;
            var elements = FindCurrentActionElements(LogType.Warning.ToString(), out found);
            elements.Add(new WarningLog(s));

            if (!found)
                ListOfElements.Add(elements);
        }

        public void Warning(string s, Exception ex)
        {
            bool found = false;
            var elements = FindCurrentActionElements(LogType.Warning.ToString(), out found);
            elements.Add(new WarningLog(s, ex));

            if (!found)
                ListOfElements.Add(elements);
        }

        public void Error(string s)
        {
            bool found = false;
            var elements = FindCurrentActionElements(LogType.Error.ToString(), out found);
            elements.Add(new ExceptionLog(s));

            if (!found)
                ListOfElements.Add(elements);
        }

        public void Error(Exception ex)
        {
            bool found = false;
            var elements = FindCurrentActionElements(LogType.Error.ToString(), out found);
            elements.Add(new ExceptionLog(ex));

            if (!found)
                ListOfElements.Add(elements);
        }

        public void Error(string s, Exception ex)
        {
            bool found = false;
            var elements = FindCurrentActionElements(LogType.Error.ToString(), out found);
            elements.Add(new ExceptionLog(s, ex));

            if (!found)
                ListOfElements.Add(elements);
        }

        public void Custom(ILogElement log)
        {
            bool found = false;
            var elements = FindCurrentActionElements("Action", out found);
            elements.Add(log);

            if (!found)
                ListOfElements.Add(elements);
        }

        public void Custom(object obj)
        {
            var xml = JsonConvert.DeserializeXNode(SimpleJson.SerializeObject(obj), obj.GetType().Name).Root;

            bool found = false;
            var elements = FindCurrentActionElements("Action", out found);
            elements.Add(new XmlLog(xml.ToString()));

            if (!found)
                ListOfElements.Add(elements);
        }

        //public void Custom(ILogElement log)
        //{
        //    bool found = false;
        //    var elements = FindList(type, out found);
        //    elements.Add(log);

        //    if (!found)
        //        ListOfElements.Add(elements);
        //}

        public void Add(LogElements elements)
        {
            ListOfElements.Add(elements);
        }

        public void ClearElements()
        {
            //clear the log
            this.listOfElements.Clear();
        }

        public string WriteXml
        {
            get
            {
                XDocument log = CreateLogFile();
                XElement rootElement = new XElement("Log");

                foreach (var elements in listOfElements)
                {
                    XElement x_elements = new XElement(elements.Type.ToString(), new XAttribute("Created", elements.Created));

                    if (elements.Action != null)
                    {
                        x_elements.Add(new XAttribute("ActionId", elements.Action.Id));
                        x_elements.Add(new XAttribute("ActionType", elements.Action.Type.Name));
                        x_elements.Add(new XAttribute("Assembly", new ApplicationInfo(Assembly.GetAssembly(elements.Action.GetType())).ToString()));

                        //var xml = JsonConvert.DeserializeXNode(SimpleJson.SerializeObject(new ApplicationInfo(Assembly.GetAssembly(elements.Action.GetType()))), "Assembly").Root;
                        //x_elements.Add(xml);
                    }

                    string actionId = "";

                    if (action != null)
                        actionId = action.Id;

                    foreach (ILogElement e in elements.Where(el => el != null))
                    {
                        var type = e.GetType();
                        var props = type.GetProperties();
                        var typename = type.Name;
                        XElement typeElement;

                        //if (typename.ToLower().EndsWith("log"))
                        //    typename = typename.Substring(0, typename.Length - 3);

                        switch (typename)
                        {
                            case "InformationLog":
                                {
                                    typename = typename.Substring(0, typename.Length - 3);
                                    typeElement = new XElement(typename);
                                    var prop = props.First();
                                    var name = prop.Name;
                                    var value = prop.GetValue(e, null);
                                    typeElement.Value = value.ToString();
                                    break;
                                }
                            case "ExceptionLog":
                            case "WarningLog":
                                {
                                    typename = typename.Substring(0, typename.Length - 3);
                                    string serialized = SimpleJson.SerializeObject(e);
                                    XDocument doc = JsonConvert.DeserializeXNode(serialized, typename);
                                    typeElement = doc.Root;
                                    break;
                                }
                            case "XmlLog":
                                {
                                    typename = typename.Substring(0, typename.Length - 3);
                                    typeElement = XDocument.Parse(e.Message).Root;
                                    break;
                                }
                            default:
                                {
                                    string serialized = SimpleJson.SerializeObject(e);
                                    XDocument doc = JsonConvert.DeserializeXNode(serialized, typename);
                                    typeElement = doc.Root;
                                    break;
                                }
                        }

                        x_elements.Add(typeElement);
                    }

                    rootElement.Add(x_elements);
                }

                XElement logs = log.Elements().First();
                logs.Add(rootElement);

                //save the log to disk
                string path = Path.Combine(ActionHelper.GetDirectoryPath(), "Logs");
                string file = new GlobalActionFunctions().GetCurrentFormatDateTimeString() + ".xml";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //removing empty elements
                log.Descendants()
                        .Where(e => e.IsEmpty || String.IsNullOrWhiteSpace(e.Value))
                        .Remove();

                //log.Save(Path.Combine(path, file));

                //ClearElements();

                return log.Root.ToString();
            }
        }

        public string WriteJson
        {
            get
            {
                //todo create good json objects from the list of elements
                throw new NotImplementedException();
            }
        }
    }
}
