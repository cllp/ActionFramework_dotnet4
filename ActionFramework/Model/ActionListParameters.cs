using ActionFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Model
{
    public class ActionListParameters : List<IParameter>
    {
        private IActionDataSource dataSource;

        public IActionDataSource DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        public ActionListParameters()
        {
        }

        public ActionListParameters(IActionDataSource dataSource)
        {
            this.dataSource = dataSource;
        }
    }
}
