using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ActionFramework.Interfaces;

namespace ActionFramework.Classes
{
    internal class Compressor : ICompressor
    {
        public string CompressFile(byte[] file)
        {
            byte[] compress = Compress(file);
            return base64_encode(compress);
        }

        public string CompressFile(MemoryStream stream)
        {
            //buffer to write Compressed data
            byte[] buffer;

            //create buffer of total content Length
            buffer = new byte[stream.Length];
            byte[] compress = Compress(buffer);
            return base64_encode(compress);
        }

        public string CompressFile(XDocument doc)
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

        public string CompressFile(string fileName)
        {
            byte[] file = File.ReadAllBytes(fileName);
            byte[] compress = Compress(file);
            return base64_encode(compress);
        }

        public string CompressFile(Stream file, int length)
        {
            byte[] compress = Compress(LoadFile(file, length));
            return base64_encode(compress);
        }

        public byte[] DecompressFile(string encoded)
        {
            byte[] decoded = base64_decode(encoded);
            byte[] decompressed = Decompress(decoded);
            return decompressed;
            //File.WriteAllBytes(@"c:\out.pdf", decompressed);
        }

        public Assembly DecompressAssembly(string encoded)
        {
            var file = ActionFactory.Compression.DecompressFile(encoded);
            return LoadAssembly(file);
        }

        public Assembly LoadAssembly(byte[] file)
        {
            return Assembly.Load(file);
        }

        public Assembly LoadAssembly(Stream file, int length)
        {
            return Assembly.Load(LoadFile(file, length));
        }

        public byte[] LoadFile(Stream file, int length)
        {
            byte[] filedata = null;
            using (var binaryReader = new BinaryReader(file))
            {
                filedata = binaryReader.ReadBytes(length);
            }

            return filedata;
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
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
