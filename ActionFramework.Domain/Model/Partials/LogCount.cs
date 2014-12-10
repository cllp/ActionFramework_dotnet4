using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woxion.Utility.ActionFramework.Domain.Model
{
    public partial class LogCount
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public double Persent { get; set; }        
    }
}
