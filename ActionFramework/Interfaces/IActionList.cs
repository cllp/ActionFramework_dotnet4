using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Enum;
using System.Xml.Linq;
using System.Reflection;
using ActionFramework.Model;
using ActionFramework.Logging;
using ActionFramework.Classes;

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
    }
}
