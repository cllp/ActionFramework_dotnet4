using ActionFramework.Logging;
using ActionFramework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Entities
{
    public class Resources : List<ResourceParameter>
    {
        //public Resource Resource(string name)
        //{
        //    try
        //    {
        //        return this.Find(o => o.Name.Trim() == name);
        //        //return System.Text.Encoding.UTF8.GetString(ActionFactory.Compression.DecompressFile(.Object));
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = "Could not find resource value with name: '" + name + "'"; ;
        //        LogContext.Current().Error(msg, ex);
        //        throw ex;
        //    }
        //}
    }
}
