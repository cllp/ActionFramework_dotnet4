using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ActionFramework.Enum;
using System.Reflection;
using ActionFramework.Interfaces;
using System.IO;
using System.Text.RegularExpressions;
using ActionFramework.Model;
using System.Configuration;
using RestSharp;
using ActionFramework.Context;
using ActionFramework.Logging;

namespace ActionFramework.Classes
{
    public static class ReflectionHelper
    {
        public static Type[] GetExecutingAssemblyTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes();
        }

        public static Type[] GetCallingAssemblyTypes()
        {

            return Assembly.GetCallingAssembly().GetTypes();

        }

        public static Assembly[] GetDomainAssemblies()
        {

            return AppDomain.CurrentDomain.GetAssemblies();

        }

        public static Assembly[] GetCustomAssemblies(string customAssemblyPath)
        {
            List<Assembly> assemblies = new List<Assembly>();
            if (!Directory.Exists(customAssemblyPath))
                throw new Exception("Path does not exists!");

            DirectoryInfo di = new DirectoryInfo(customAssemblyPath);
            FileInfo[] rgFiles = di.GetFiles("*.dll");

            foreach (FileInfo fi in rgFiles)
            {
                Assembly a = Assembly.LoadFrom(fi.FullName);
                assemblies.Add(a);
            }

            return assemblies.ToArray();
        }

        public static Assembly[] GetCustomAssemblies(string customAssemblyPath, List<App> appList)
        {
            List<Assembly> assemblies = new List<Assembly>();
            if (!Directory.Exists(customAssemblyPath))
                throw new Exception("Path does not exists!");

            DirectoryInfo di = new DirectoryInfo(customAssemblyPath);
            FileInfo[] rgFiles = di.GetFiles("*.dll");

            //TODO get only files dll that is included in as installed apps
            //i think we can check by name even if version is included in the physical filename

            foreach (FileInfo fi in rgFiles) //.Where(f => f.Name.IsInList())
            {
                if (appList.Any(x => string.Format(Constants.AssemblyFileName, x.Name.Trim(), x.Version.Trim()).Equals(fi.Name)))
                {
                    Assembly a = Assembly.LoadFrom(fi.FullName);
                    assemblies.Add(a);
                }
            }

            return assemblies.ToArray();
        }

        public static Type GetActionType(Type[] types, string name)
        {
            return (from t in types where t.Name.Equals(name.Trim()) select t).FirstOrDefault();
        }

        public static Type[] GetActionTypes(List<XElement> settingElements)
        {

            List<Type> types = new List<Type>();
            Type actionInterface = typeof(IAction);

            string customAssemblyPath = "";
            ActionProperty customXmlAssemblyPath = (from p in ActionHelper.GetSettingProperties(settingElements) where p.Name.Equals(Constants.CustomAssemblyFolderName) select p).FirstOrDefault();

            string customExeAssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (customXmlAssemblyPath == null)
                customAssemblyPath = customExeAssemblyPath;
            else
            {
                if (string.IsNullOrEmpty(customXmlAssemblyPath.Value))
                    customAssemblyPath = customExeAssemblyPath;
                else
                    customAssemblyPath = customXmlAssemblyPath.Value;
            }

            Assembly[] customAssemblies = GetCustomAssemblies(customAssemblyPath);

            foreach (Assembly a in customAssemblies)
            {
                try
                {
                    foreach (Type type in a.GetTypes())
                    {
                        bool isAssignedFrom = IsActionType(type);
                        bool containsActionBase = type.Name.Contains("ActionBase");
                        bool containsIAction = type.Name.Contains("IAction");

                        if (isAssignedFrom && !containsIAction && !containsActionBase)
                            types.Add(type);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("An error occured when executing 'GetTypes()' method in assembly: '{0}'", a.FullName), ex.InnerException);
                }
            }

            return types.ToArray();
        }

        private static PropertyInfo[] GetPropertyInfoName(Type obj)
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = obj.GetProperties(BindingFlags.GetProperty);
            // sort properties by name
            Array.Sort(propertyInfos,
                    delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
                    { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            // write property names
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var info = propertyInfo.Name;
            }

            return propertyInfos;
        }

        /// <summary>
        /// here we can set the values of properties runtime
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(Type actionType, object instance, string propertyName, object value)
        {
            // create instance of class Action
            //object instance = Activator.CreateInstance(actionType); // set when it is already instanced
            // get info about property: public double Number
            PropertyInfo prop = actionType.GetProperty(propertyName);

            // set value of property
            prop.SetValue(instance, value, null);
        }

        public static PropertyInfo[] GetActionProperties(Type actionType)
        {
            return actionType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        }

        public static PropertyInfo[] GetAllActionProperties(Type actionType)
        {
            return actionType.GetProperties();
        }

        public static object InvokeMember(object instance, string method)
        {
            return instance.GetType().InvokeMember(method, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, instance, null);
        }

        public static List<Type> GetAssemblyActions(Assembly assembly)
        {
            List<Type> types = new List<Type>();
            Type actionInterface = typeof(IAction);

            foreach (Type type in assembly.GetTypes())
            {
                var props = GetPropertyInfoName(type);
                //properties
                MethodInfo[] methods = type.GetMethods();//.GetMethod("Method1");
                MemberInfo[] members = type.GetMembers();//.GetMethod("Method1");
                // Obtain a reference to the parameters collection of the MethodInfo instance.

                foreach (var mi in methods)
                {
                    ParameterInfo[] Params = mi.GetParameters();

                    foreach (var info in Params)
                    {
                        var iii = info.Member.Name;
                    }


                }


                bool isAssignedFrom = IsActionType(type);
                bool containsActionBase = type.Name.Contains("ActionBase");
                bool containsIAction = type.Name.Contains("IAction");

                if (isAssignedFrom && !containsIAction && !containsActionBase)
                    types.Add(type);
            }

            return types;
        }

        public static Type[] GetActionTypes(List<ActionProperty> settings)
        {
            List<Type> types = new List<Type>();
            Type actionInterface = typeof(IAction);

            string customAssemblyPath = "";
            ActionProperty customXmlAssemblyPath = (from p in settings where p.Name.Equals(Constants.CustomAssemblyFolderName) select p).FirstOrDefault();

            string customExeAssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (customXmlAssemblyPath == null)
                customAssemblyPath = customExeAssemblyPath;
            else
            {
                if (string.IsNullOrEmpty(customXmlAssemblyPath.Value))
                    customAssemblyPath = customExeAssemblyPath;
                else
                    customAssemblyPath = customXmlAssemblyPath.Value;
            }

            Assembly[] customAssemblies = GetCustomAssemblies(customAssemblyPath);

            foreach (Assembly a in customAssemblies)
            {
                types.AddRange(GetAssemblyActions(a));
            }

            return types.ToArray();
        }

        public static Type[] GetActionTypes(List<ActionProperty> settings, List<App> appList)
        {
            List<Type> types = new List<Type>();
            Type actionInterface = typeof(IAction);

            string customAssemblyPath = "";
            ActionProperty customXmlAssemblyPath = (from p in settings where p.Name.Equals(Constants.CustomAssemblyFolderName) select p).FirstOrDefault();

            string customExeAssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (customXmlAssemblyPath == null)
                customAssemblyPath = customExeAssemblyPath;
            else
            {
                if (string.IsNullOrEmpty(customXmlAssemblyPath.Value))
                    customAssemblyPath = customExeAssemblyPath;
                else
                    customAssemblyPath = customXmlAssemblyPath.Value;
            }

            Assembly[] customAssemblies = GetCustomAssemblies(customAssemblyPath, appList);

            foreach (Assembly a in customAssemblies)
            {
                types.AddRange(GetAssemblyActions(a));
            }

            return types.ToArray();
        }

        private static bool IsActionType(Type type)
        {
            System.Reflection.TypeFilter actionFilter = new System.Reflection.TypeFilter(ActionTypeFilter);
            Type[] interfaces = type.FindInterfaces(actionFilter, typeof(IAction));
            return interfaces.Count() > 0;
        }

        private static bool ActionTypeFilter(Type m, object filterCriteria)
        {
            return m.ToString().Equals(filterCriteria.ToString());
        }

        private static List<string> GetCustomAssemblyNames(string customAssemblyPath)
        {
            List<string> assemblies = new List<string>();
            if (!Directory.Exists(customAssemblyPath))
                throw new Exception("Path does not exists!");

            DirectoryInfo di = new DirectoryInfo(customAssemblyPath);
            FileInfo[] rgFiles = di.GetFiles("*.dll");

            foreach (FileInfo fi in rgFiles)
            {
                assemblies.Add(fi.Name);
            }

            return assemblies;
        }
    }
}
