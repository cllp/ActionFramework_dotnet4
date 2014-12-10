using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.IO;
using ActionFramework.Events;
using ActionFramework.Entities;
using ActionFramework.Enum;
using ActionFramework.Logging;
using ActionFramework.Model;

namespace ActionFramework.Actions
{
    public class PropertiesLog : ILogElement
    {
        public List<PropertyLogItem> Properties { get; set; }
        public string Message { get; set; }
    }

    public class PropertyLogItem
    {
        public string Name { get; set; }
        public string OriginalValue { get; set; }
        public string CalculatedValue { get; set; }
    }

    public class ListProperties : ActionBase
    {
        public override object Execute()
        {
            try
            {
                PropertiesLog plog = new PropertiesLog();
                plog.Message = "Logging properties";
                plog.Properties = new List<PropertyLogItem>();

                foreach (ActionProperty be in this.DynamicProperties)
                {
                    PropertyLogItem pli = new PropertyLogItem();
                    pli.Name = be.Name;
                    pli.OriginalValue = be.Value;
                    pli.CalculatedValue = Prop(be.Name);
                    plog.Properties.Add(pli);   
                }

                //Log.Custom(plog);
                PropertyLogItem mi = new PropertyLogItem();
                mi.CalculatedValue = "wer";
                mi.Name = "This is nams";
                mi.OriginalValue = "Origin";
                Log.Custom(mi);

                PropertyLogItem mi2 = new PropertyLogItem();
                mi2.CalculatedValue = "mi2";
                mi2.Name = "This is mi2";
                mi2.OriginalValue = "mi2mi2mi2";
                Log.Custom(mi2);

                //Log.Custom(this.Resources);
                //Log.Info("another resource");
                //Log.Custom(this.Resources);


                foreach(var r in this.Resources)
                {
                    Log.Custom(r);
                }

                

                HandleSuccess();
            }
            catch (Exception ex)
            {
                HandleException(ex);

                if (BreakOnError)
                    throw;
            }

            return this.Status;
        }

        public override object Execute(params object[] parameters)
        {
            try
            {
                var resource = Resource("TestResource");

                string log = "";
                for (int i = 0; i < parameters.Length; i++)
                {
                    log += (parameters[i] + " ");
                }

                Log.Info("Parameters: " + log);

                //HandleSuccess();
            }
            catch (Exception ex)
            {
                HandleException(ex);

                if (BreakOnError)
                    throw;
            }

            return this.Status;
        }

    }
}
