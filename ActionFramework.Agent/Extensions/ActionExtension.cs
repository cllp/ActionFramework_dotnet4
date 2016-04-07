using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.Domain.Model;
using ActionFramework.Enum;
using ActionFramework.Interfaces;

namespace ActionFramework.Agent.Extensions
{
    public static class ActionExtension
    {
        public static List<Domain.Model.Action> GetActionAndProperties(this Assembly ass)
        {
            return GetActionAndProperties(ass, null);
        }

        public static List<Domain.Model.Action> GetActionAndProperties(this Assembly ass, App app)
        {
            var actionTypes = ReflectionHelper.GetAssemblyActions(ass);
            List<Domain.Model.Action> dbActions = new List<Domain.Model.Action>();
            foreach (var a in actionTypes)
            {
                IAction instance = (IAction)System.Activator.CreateInstance(a);
                //var instanceProperties2 = a.GetInstanceProperties(); // kolla om det funkar!

                var instanceProperties = instance.GetInstanceProperties();

                var dbAction = new Domain.Model.Action();
                dbAction.App = app;
                dbAction.BreakOnError = false;
                dbAction.ClientExecute = true;
                dbAction.Description = "";
                dbAction.Enabled = true;
                dbAction.Type = instance.GetType().Name;
                dbActions.Add(dbAction);

                var dbProperties = new List<Property>();

                foreach (var property in instanceProperties)
                {
                    var dbProperty = new Property();
                    dbProperty.Name = property.Name;
                    dbProperty.Value = string.Empty;
                    dbProperty.PropertyTypeId = 1;
                    dbProperty.DataType = property.PropertyType.Name;
                    dbProperty.Enabled = true;
                    dbProperties.Add(dbProperty);
                }

                dbAction.Properties = dbProperties;
            }

            return dbActions;
        }


        public static IAction CreateActionInstance(this IAction source)
        {
            object actInstance = System.Activator.CreateInstance(source.GetType());
            source.CopyProperties(actInstance);
            return (IAction)actInstance;
        }

        public static object CreateActionInstance(this object source)
        {
            object actInstance = System.Activator.CreateInstance(source.GetType());
            source.CopyProperties(actInstance);
            return actInstance;
        }

        public static object Exec(this object source)
        {
            // invoke public instance method: public void Clear()
            return source.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, source, null);
        }

        public static PropertyInfo[] GetInstanceProperties(this object source)
        {
            return source.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        }

        public static PropertyInfo[] GetAllProperties(this object source)
        {
            return source.GetType().GetProperties();
        }

        public static void ResolveStaticProperties(this IAction source)
        {
            foreach (var p in source.GetInstanceProperties())
            {
                string configValue = source.Prop(p.Name);

                if (!string.IsNullOrEmpty(configValue))
                {
                    switch (p.PropertyType.Name)
                    {
                        case "Int32":
                            {
                                p.SetValue(source, Convert.ToInt32(configValue));
                                break;
                            }
                        case "Boolean":
                            {
                                p.SetValue(source, Convert.ToBoolean(configValue));
                                break;
                            }
                        case "DateTime":
                            {
                                p.SetValue(source, Convert.ToDateTime(configValue));
                                break;
                            }
                        case "Double":
                            {
                                p.SetValue(source, Convert.ToDouble(configValue));
                                break;
                            }
                        case "XDocument":
                            {
                                p.SetValue(source, XDocument.Parse(configValue));
                                break;
                            }
                        default:
                            {
                                p.SetValue(source, configValue);
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyProperties(this object source, object destination)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();

            // Iterate the Properties of the source instance and  
            // populate them from their desination counterparts  
            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                // Passed all tests, lets set the value
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }
        }
    }
}
