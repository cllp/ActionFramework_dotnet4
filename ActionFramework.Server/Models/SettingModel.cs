using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Server.Models
{
    public class SettingModel
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
    }
}