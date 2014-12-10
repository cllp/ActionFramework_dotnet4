using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Base;
using ActionFramework.Enum;
using ActionFramework.Interfaces;

namespace ActionFramework.Action
{
    public class SendMail : ActionBase
    {
        public override object Execute()
        {
            try 
            {
                //string resourcefile = Prop("MailTemplate");//Resource("MailBody");

                var resource = Resource("MailTemplate");
                

                //BusinessFactory.Dictionary().Translate("ActivationMailSubject", "sv");
                var replace = ActionFactory.Replace;
                replace.Add("%name%", "Claes-Philip");
                string body = replace.ReplaceFromFile(ActionFactory.Compression.DecompressFile(resource.CompressedFile));
                string ccc = ActionFactory.Compression.CompressString(body);
                Log.Info(ccc);
                return HandleSuccess();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
