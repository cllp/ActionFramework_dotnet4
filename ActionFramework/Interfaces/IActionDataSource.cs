using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Model;
using ActionFramework.Enum;

namespace ActionFramework.Interfaces
{
    public interface IActionDataSource
    {
        List<App> Apps
        {
            get;
            set;
        }

        List<IAction> GetActions(ActionStatus status);

        List<ActionProperty> GlobalSettings
        {
            get;
        }

        void FillActions(IActionList actionList, ActionStatus status);
    }
}
