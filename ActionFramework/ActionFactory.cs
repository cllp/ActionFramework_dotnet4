using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using ActionFramework.Classes;
//using ActionFramework.Context;
using ActionFramework.Enum;
using ActionFramework.Logging;
using System.IO;
using ActionFramework.Model;
using ActionFramework.SystemLogger;

namespace ActionFramework
{
    public class ActionFactory
    {
        public static IActionList ActionList(ActionListParameters par)
        {
            IActionList list;

            list = new ActionList(par.DataSource);
            var resources = par.Where(p => p.GetType().Equals(typeof(ResourceParameter)));

            foreach (ResourceParameter resource in resources)
            {
                list.Resources.Add(resource);
            }

            return list;
        }

        public static ISystemLogger SysLog()
        {
            return new FileLogger();
        }

        public static void InitializeLog()
        {
            if(!LogContext.IsInitialized)
                LogContext.Initialize(new Logger());
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
    }
}
