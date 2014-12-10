using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Domain.Model.Api
{
    [Serializable]
    public class LogModel
    {
        public string AgentId { get; set; }
        public string ActionFile { get; set; }
        public string Description { get; set; }
        public string Data { get; set; }
    }
}
