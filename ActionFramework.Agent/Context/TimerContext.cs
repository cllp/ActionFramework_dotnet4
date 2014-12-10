using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.Interfaces;
using ActionFramework.Model;
using System.IO;
using ActionFramework.DataSource;

namespace ActionFramework.Agent.Context
{
    public class TimerContext : IDisposable
    {
        public static System.Threading.Timer actionTimer;
        private static int timerInterval = 0;
        //private static IEventLogger eventlog = ActionFactory.EventLogger();

        //public static void Initialize(System.Threading.Timer timer)
        //{
        //    actionTimer = timer;
        //}

        public static void Initialize(int interval)
        {
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "TimerContext Initialized. Interval: '" + interval.ToString() + "'", Constants.EventLogId);
            Activator.SetLastRunDate();//AgentConfigurationContext.Current.UpdateSetting("LastRunDate", DateTime.Now);
            timerInterval = interval;
            actionTimer = new System.Threading.Timer(new TimerCallback(actionTimer_Elapsed), null, 0, timerInterval);
            //actionTimer = timer;
        }

        public static void actionTimer_Elapsed(object sender)
        {
            try
            {
                var lastRun = Activator.GetLastRunDate().ToString("yyyy-MM-dd HH:mm");
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "ActionTimer Elapsed. Previous " + lastRun + " Interval " + AgentConfigurationContext.Current.Interval.ToString(), Constants.EventLogId);




                //var ds = ActionHelper.GetConfigurationDataSource();
                var ds = Activator.GetActionDataSource();


                var log = Activator.RunActions(new ActionListParameters(ds));
                Activator.SetLastRunDate(); //AgentConfigurationContext.Current.UpdateSetting("LastRunDate", DateTime.Now);
            }
            catch (Exception ex)
            {
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, "ActionTimer Elapsed with 'RunActions' caused an error. '" + ex.Message + "'", Constants.EventLogId);
            }

            //here is where configuration is read from xml
            //if the app is here and running shit. It should be shown in the applicationwindow.
            //Console.WriteLine("The Elapsed event was raised at "); //{0}", e.SignalTime);
        }

        /// <summary>
        /// Returns a agent configuration context that is applicable for this context.
        /// </summary>
        public static System.Threading.Timer Current
        {
            get
            {
                EnsureInitialized();
                return actionTimer;
            }
        }

        public static bool IsInitialized
        {
            get { return actionTimer != null; }
        }

        public static int TimerInterval
        {
            get { return timerInterval; }
        }

        private static void EnsureInitialized()
        {
            if (actionTimer == null)
            {
                throw new InvalidOperationException("ActionTimer provider has not been initialized.");
            }
        }

        public void Dispose()
        {
            actionTimer = null;
            actionTimer.Dispose();
        }

        public static void StopTimer()
        {
            actionTimer = null;
            //actionTimer.Dispose();
        }
    }
}
