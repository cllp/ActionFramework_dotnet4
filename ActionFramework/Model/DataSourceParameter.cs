using ActionFramework.Enum;
using ActionFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Model
{
    public class DataSourceParameter : IParameter
    {
        private DataSourceFormat dataSourceFormat = DataSourceFormat.Simple;
        private string dataSourcePath;
        private DataSourceLocation dataSourceLocation = DataSourceLocation.Local;
        public string FileName { get; set; }

        public DataSourceFormat DataSourceFormat
        {
            get { return dataSourceFormat; }
            set { dataSourceFormat = value; }
        }
        public string DataSourcePath
        {
            get { return dataSourcePath; }
            set { dataSourcePath = value; }
        }

        public DataSourceLocation DataSourceLocation
        {
            get { return dataSourceLocation; }
            set { dataSourceLocation = value; }
        }
    }
}
