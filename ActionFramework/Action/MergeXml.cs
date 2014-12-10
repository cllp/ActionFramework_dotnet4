using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.Xml;
using System.Data;

namespace ActionFramework.Action
{
    public class MergeXml : ActionBase
    {
        public override object Execute()
        {
            try
            {
                string[] files = Prop("Files").Split(';');
                string output = Prop("Output");
                DataSet dsOutput = new DataSet();
                
                foreach (string file in files)
                {
                    DataSet ds = new DataSet();
                    XmlTextReader xmlreader = new XmlTextReader(file);
                    ds.ReadXml(xmlreader);
                    dsOutput.Merge(ds);
                }

                dsOutput.WriteXml(output);
                Status = "OK";
            }
            catch (System.Exception ex)
            {
                Status = HandleException(ex);
            }

            return Status;
        }
    }
}
