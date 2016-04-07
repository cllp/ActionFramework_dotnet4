using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Server.Models
{
    public class OrganizationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}