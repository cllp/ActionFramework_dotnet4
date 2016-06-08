using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApps
{
    public class GetAgentUri : ActionBase
    {
        public string Uri { get; set; }

        public override object Execute()
        {
            try
            {
                if (string.IsNullOrEmpty(Uri))
                    throw new Exception(string.Format("Property 'Uri' is not configured"));
                else
                    return Uri;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return HandleException(ex);
            }
        }
    }
}
