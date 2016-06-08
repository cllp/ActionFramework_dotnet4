using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ActionFramework.Model;
using ActionFramework.Logging;
using ActionFramework.Model;

namespace ActionFramework.Interfaces
{
    public interface IAction
    {
        string Id
        {
            get;
            set;
        }

        IActionList ActionList
        {
            get;
            set;
        }

        //IActionDataSource DataSource
        //{
        //    get;
        //    set;
        //}

        bool Enabled
        {
            get;
            set;
        }

        bool ClientExecute
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        Type Type
        {
            get;
            set;
        }

        Assembly Assembly
        {
            get;
            set;
        }

        string Status
        {
            get;
            set;
        }

        bool BreakOnError
        {
          get;
          set;
        }

        List<ActionProperty> DynamicProperties
        {
            get;
            set;
        }

        List<ResourceParameter> Resources
        {
            get;
            set;
        }

        ILogger Log
        {
            get;
        }

        ICommon Common
        {
          get;
        }

        void AddDynamicProperties(List<ActionProperty> properties);

        void AddResources(List<ResourceParameter> resources);

        void AddResource(ResourceParameter resource);

        /// <summary>
        /// Returns the dynamic property from name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string Prop(string name);

        IAction FindActionByType(string type);
        
        IAction FindActionById(string type);

        object Invoke(object instance, string method, object[] parameters);

        object Invoke(object instance, string method);

        object Execute();

        object Execute(out object output);

        object Execute(object input);

        object Execute(object input, out object output);

        object Execute(params object[] parameters);

        object Execute(out object output, params object[] parameters);
    }
}
