using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.IO;
using ActionFramework.Events;

namespace ActionFramework.Actions
{
    public class ExecuteCommand : ActionBase
    {
      
        public override object Execute()
        {           

            try
            {
                string command = Prop("CommandFile");
                string args = Prop("Arguments");
               
                System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;

                if (!string.IsNullOrEmpty(args))
                  procStartInfo.Arguments = args;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                //Console.WriteLine(result);

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
    }
}
