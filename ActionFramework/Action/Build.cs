using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using Microsoft.Build;
using System.IO;
using System.Security;

namespace ActionFramework.Action
{
    public class Build : ActionBase
    {
        public override object Execute()
        {
            try
            {
                ////bool success = false;
                //Engine engine = Microsoft.Build.BuildEngine.Engine.GlobalEngine;
                //try
                //{
                //    string solution = Prop("ProjectFile");
                //    string configuration = Prop("Configuration");

                //    return HandleSuccess(Common.Build(solution, configuration));

                //    //BuildPropertyGroup props = new BuildPropertyGroup();
                //    //props.SetProperty("Configuration", Prop("Configuration"));

                //    //FileLogger logger = new FileLogger();
                //    //logger.Parameters = @"logfile=" + Prop("BuildLogFile") + "";

                //    //engine.RegisterLogger(logger);
                //    //success = engine.BuildProjectFile(solution, null, props);
                //    //return HandleSuccess(". Build status: " + success.ToString());
                //}
                //catch (Exception ex)
                //{
                //    return HandleException(ex);
                //}
                //finally
                //{
                //    //engine.UnregisterAllLoggers();
                //    //engine.UnloadAllProjects();
                //}
                return HandleSuccess();
              
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

        }
    }
}
