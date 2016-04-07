using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Helpers
{
    public static class ObjectSerializer
    {
        public static byte[] ToBytes<T>(T obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            byte[] byteArr;
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                byteArr = ms.ToArray();
            }
            
            return byteArr;

        }

        public static T ToObject<T>(byte[] byteArr)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            using (var ms = new MemoryStream(byteArr))
            {
                var obj = serializer.ReadObject(ms);
                return (T)obj;
            }
        }
    }
}
