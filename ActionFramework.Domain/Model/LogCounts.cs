using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Domain.Model
{
    public class LogCounts : List<LogCount>
    {
        private int total;

        public int Total
        {
            get { return total; }
            set { total = value; }
        }

        public int GetCount(string type)
        {
            return this.Where(t => t.Type.Equals(type)).FirstOrDefault().Count;
        }
    }
}
