using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using ActionFramework.Model;

namespace ActionFramework.DataSource
{
    public class SqlServerDataSource : IActionDataSource
    {

        public List<IAction> GetActions(Enum.ActionStatus status)
        {
            throw new NotImplementedException();
        }

        public List<ActionProperty> GlobalSettings
        {
            get { throw new NotImplementedException(); }
        }

        public IActionList ActionList
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public List<Domain.Model.App> Apps
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public void FillActions(IActionList actionList, Enum.ActionStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
