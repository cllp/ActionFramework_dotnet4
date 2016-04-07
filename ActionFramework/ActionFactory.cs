using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using ActionFramework.Classes;
using ActionFramework.Domain.Model;
using ActionFramework.Context;
using ActionFramework.Enum;
using ActionFramework.Logging;
using System.IO;
using ActionFramework.Model;


namespace ActionFramework
{
    public class ActionFactory
    {
        //public static IActionList ActionList()
        //{
        //    return new ActionList(new RESTDataSource());
        //}

        //public static IActionList ActionList(string xml, DataSourceFormat format)
        //{
        //    if (format == DataSourceFormat.Nested)
        //        return new ActionList(new RESTDataSource(xml));
        //    else
        //        return new ActionList(new XmlDataSource(xml));
        //}

        public static IActionList ActionList(ActionListParameters par)
        {
            IActionList list;

            list = new ActionList(par.DataSource);
            var resources = par.Where(p => p.GetType().Equals(typeof(ResourceParameter)));

            //list.Resources.AddRange(resources);
            foreach (ResourceParameter resource in resources)
            {
                list.Resources.Add(resource);
            }

            return list;
        }

        //public static IActionList ActionList(string xmlPath)
        //{
        //    return new ActionList(new XmlDataSource(xmlPath));
        //}

        public static IEventLogger EventLogger()
        {
            return new EventLogger();
        }

        public static IEventLogger EventLogger(string caller)
        {
            return new EventLogger(caller);
        }

        public static void InitializeLog()
        {
            LogContext.Initialize(new Logger());
        }

        public static void InitializeAgentConfiguration(IAgentConfiguration config)
        {
            AgentConfigurationContext.Initialize(config);
        }

        public static ILogger CurrentLog()
        {
            return LogContext.Current();
        }

        public static ILogger CurrentLog(IAction action)
        {
            return LogContext.Current(action);
        }

        public static ICompressor Compression
        {
            get { return new Compressor(); }
        }

        public static IReplace Replace
        {
            get { return new Replace(); }
        }

        public static ICommon Common
        {
            get { return new Common(); }
        }
    }
}
