using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ActionFramework.Interfaces
{
    public interface IReplace
    {
        void Add(string key, string value);

        string ReplaceFromText(string text);

        string ReplaceFromFile(string file);

        string ReplaceFromFile(XDocument doc);
        
        string ReplaceFromFile(byte[] file);

        string ReplaceFromFile(MemoryStream file);
    }
}
