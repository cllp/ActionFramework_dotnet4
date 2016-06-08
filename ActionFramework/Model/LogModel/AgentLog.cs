using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Interfaces;

namespace ActionFramework.Model.LogModel
{
    public class AgentLog : ILogBase
    {
        public DateTime Created { get; set; }
        public string Message { get; set; }
        public string AssemblyInfo { get; set; }
        public string DataSource { get; set; }
        public string Count { get; set; }
        public string Runtime { get; set; }

        public string CountFormat
        {
            get
            {
                //REFACTORED
                throw new NotImplementedException(); //return Count.FormatCount();
            }
        }
    }
}
