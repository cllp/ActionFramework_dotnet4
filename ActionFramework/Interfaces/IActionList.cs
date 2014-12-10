using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Enum;
using System.Xml.Linq;
using System.Reflection;
using ActionFramework.Entities;
using ActionFramework.Domain.Model;
using ActionFramework.Logging;
using ActionFramework.Classes;
using ActionFramework.Model;

namespace ActionFramework.Interfaces
{
    public interface IActionList : IList<IAction>
    {
        Resources Resources
        {
            get;
            set;
        }

        IActionDataSource DataSource
        {
            get;
        }

        List<ActionProperty> GlobalSettings
        {
          get;
        }

        ILogger Log
        {
          get;
        }

        string GlobalSetting(string name);

        string Prop(string name);

        ActionResultLog Run(out string runtime);

        //Type[] GetExecutingAssemblyTypes();
        //Type[] GetCallingAssemblyTypes();
        //Assembly[] GetDomainAssemblies();

        //Type[] GetActionTypes();

        //List<BEActionFileCopy> GetFileCopyActions();
        //bool CopyFile(BEActionFileCopy fc);
        //List<IAction> GetActions(ActionStatus status);

        //List<XElement> GetActionElements(ActionStatus status);

        //List<XElement> GetSettingElements();
        //void AddAction(IAction action);
        //bool ExecuteAction(IAction action);
        //List<string> ExecuteActions(ActionStatus status);
    }
}
