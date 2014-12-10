using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using ActionFramework.Interfaces;
using ActionFramework.Base;
using ActionFramework.Enum;

namespace ActionFramework.Actions.Actions
{
    public class ServiceHandler : ActionBase
    {
        public override object Execute()
        {
            try
            {
                string serviceAction = Prop("ServiceAction");
                string serviceName = Prop("ServiceName");
                int timeout = 5000;

                if (!string.IsNullOrEmpty(Prop("TimeOut")))
                    timeout = Convert.ToInt32(Prop("TimeOut"));

                switch (serviceAction)
                {
                    case "Start":
                        {
                            StartService(serviceName, timeout);
                            break;
                        }
                    case "Stop":
                        {
                            StopService(serviceName, timeout);
                            break;
                        }
                    case "Restart":
                        {
                            RestartService(serviceName, timeout);
                            break;
                        }
                    default:
                        {
                            string message = "No ServiceAction provided. Please provide a ServiceAction of type: Start|Stop|Restart.";
                            var ex = new Exception(message);
                            Log.Error(ex);
                            throw ex;
                        }
                }

                Status = HandleSuccess();
            }
            catch (Exception ex)
            {
                Status = HandleException(ex);

                if (BreakOnError)
                    throw;
            }

            return Status;
        }
        private void StopIIS()
        {
            ServiceController iis = new ServiceController("W3SVC");
            if (null != iis)
            {
                do
                {
                    iis.Refresh();
                }
                while
                    (
                    iis.Status ==
                    ServiceControllerStatus.ContinuePending ||
                    iis.Status ==
                    ServiceControllerStatus.PausePending ||
                    iis.Status ==
                    ServiceControllerStatus.StartPending ||
                    iis.Status ==
                    ServiceControllerStatus.StopPending
                    );
                if (ServiceControllerStatus.Running ==
                    iis.Status ||
                    ServiceControllerStatus.Paused == iis.Status)
                {
                    iis.Stop();
                    iis.WaitForStatus(
                        ServiceControllerStatus.Stopped);
                }
                iis.Close();
            }
        }

        private void StartIIS()
        {
            ServiceController iis = new ServiceController("W3SVC");
            if (null != iis)
            {
                do
                {
                    iis.Refresh();
                }
                while
                    (
                    iis.Status ==
                    ServiceControllerStatus.ContinuePending ||
                    iis.Status ==
                    ServiceControllerStatus.PausePending ||
                    iis.Status ==
                    ServiceControllerStatus.StartPending ||
                    iis.Status ==
                    ServiceControllerStatus.StopPending
                    );
                if (ServiceControllerStatus.Stopped == iis.Status)
                {
                    iis.Start();
                    iis.WaitForStatus(
                        ServiceControllerStatus.Running);
                }
                else
                {
                    if (ServiceControllerStatus.Paused ==
                        iis.Status)
                    {
                        iis.Continue();
                        iis.WaitForStatus(
                            ServiceControllerStatus.Running);
                    }
                }
                iis.Close();
            }
        }

        private void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                Log.Error("StartService: '" + serviceName + "' failed.");
                Log.Error(ex);
                Status = HandleException(ex);
            }
        }

        private void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                Log.Error("StopService: '" + serviceName + "' failed.");
                Log.Error(ex);
                Status = HandleException(ex);
            }
        }

        private void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                Log.Error("RestartService: '" + serviceName + "' failed.");
                Log.Error(ex);
                Status = HandleException(ex);
            }
        }
    }
}
