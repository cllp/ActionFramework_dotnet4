using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;

namespace ActionFramework.Classes
{
    public static class Serializer
    {
        // Static members.
        /// <summary>
        /// We use a hashtable instead of a Dictionary as the Hashtable can be created syncronised, i.e. thread safe.
        /// </summary>
        //private static Hashtable mSerializers = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Creates the requested object based on the XML. Note that the object needs to be serializable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        //public static T Deserialize<T>(string xml) where T : new()
        //{
        //    T obj;
        //    using (StringReader dataReader = new StringReader(xml))
        //    {
        //        obj = (T)GetSerializer(typeof(T)).Deserialize(dataReader);
        //    }
        //    return obj;
        //}

        /// <summary>
        /// Return a serializer if it has been created earlier, else creates one and adds it to the collection before returning.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //private static XmlSerializer GetSerializer(Type type)
        //{
        //    if (mSerializers.ContainsKey(type))
        //    {
        //        return (XmlSerializer)mSerializers[type];
        //    }
        //    else
        //    {
        //        XmlSerializer serializer = new XmlSerializer(type);
        //        mSerializers[type] = serializer;
        //        return serializer;
        //    }
        //}

        //private static void DataContractSeralizer<T>()
        //{
        //    //Stream fs = new FileStream(@"C:\Users\temelm\Desktop\XmlFile.xml", FileMode.Create, FileAccess.Write);
        //    //XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(fs, Encoding.UTF8);
        //    //xdw.WriteStartDocument();
        //    //dcs.WriteObject(xdw, b);
        //    //xdw.Close();
        //    //fs.Flush();
        //    //fs.Close();

        //    //MemoryStream stream1 = new MemoryStream();


        //    ////Serialize the Record object to a memory stream using DataContractSerializer.
        //    //DataContractSerializer serializer = new DataContractSerializer(typeof(T));
        //    //serializer.WriteObject(stream1, record1);

        //}

        public static T Deserialize<T>(string rawXml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(rawXml)))
            {
                DataContractSerializer formatter0 =
                    new DataContractSerializer(typeof(T));
                return (T)formatter0.ReadObject(reader);
            }
        }

        //public static object Deserialize(string xml, Type toType)
        //{

        //    using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
        //    {
        //        System.IO.StreamReader str = new System.IO.StreamReader(memoryStream);
        //        System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(toType);
        //        return xSerializer.Deserialize(str);
        //    }

        //}

        public static string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}
