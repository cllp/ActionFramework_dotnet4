using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ActionFramework.Interfaces
{
    public interface ICompressor
    {
        string CompressFile(byte[] file);
        string CompressFile(MemoryStream stream);
        string CompressFile(XDocument doc);
        string CompressFile(string fileName);
        string CompressFile(Stream file, int length);
        string CompressString(string text);
        byte[] DecompressFile(string encoded);
        byte[] LoadFile(Stream file, int length);
        Assembly DecompressAssembly(string encoded);
        Assembly LoadAssembly(byte[] file);
        Assembly LoadAssembly(Stream file, int length);
        string DecompressString(string compressedText);
    }
}
