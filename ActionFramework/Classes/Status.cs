using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Classes
{
    public class Status : ActionBase
    {

        public override object Execute()
        {
            try
            {
                return HandleSuccess();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
