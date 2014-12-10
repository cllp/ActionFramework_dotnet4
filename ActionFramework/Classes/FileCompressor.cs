using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Woxion.Utility.ActionFramework.Classes
{
    public class FileCompressor
    {
        public string Compress(MemoryStream stream)
        {
            //buffer to write Compressed data
            byte[] buffer;

            //create buffer of total content Length
            buffer = new byte[stream.Length];
            byte[] compress = Compress(buffer);
            return base64_encode(compress);
        }

        public string Compress(XDocument doc)
        {
            //buffer to write Compressed data
            byte[] buffer;

            //Compress the XML DATA
            MemoryStream memoryStream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(memoryStream);

            //Save data to memoryStream
            doc.Save(xmlWriter);

            //writer Close
            xmlWriter.Close();

            //Reset Memorystream postion to 0
            memoryStream.Position = 0;

            //create buffer of total content Length
            buffer = new byte[memoryStream.Length];
            byte[] compress = Compress(buffer);
            return base64_encode(compress);
        }

        public string Compress(string fileName)
        {
            byte[] file = File.ReadAllBytes(fileName);
            byte[] compress = Compress(file);
            return base64_encode(compress);
        }

        public byte[] Decompress(string encoded)
        {
            byte[] decoded = base64_decode(encoded);
            byte[] decompressed = Decompress(decoded);
            return decompressed;
            //File.WriteAllBytes(@"c:\out.pdf", decompressed);
        }

        private byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        private byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                var buffer = new byte[4096];
                int read;

                while ((read = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    resultStream.Write(buffer, 0, read);
                }

                return resultStream.ToArray();
            }
        }

        private string base64_encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            return Convert.ToBase64String(data);
        }

        private byte[] base64_decode(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return encodedDataAsBytes;
        } 
    }
}
