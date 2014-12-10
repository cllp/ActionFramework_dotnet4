using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Domain.Model;
using ActionFramework.Entities;
using ActionFramework.Enum;
using ActionFramework.Model;

namespace ActionFramework.Interfaces
{
    public interface IActionDataSource
    {
        //IActionList ActionList
        //{
        //    get;
        //    set;
        //}

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
