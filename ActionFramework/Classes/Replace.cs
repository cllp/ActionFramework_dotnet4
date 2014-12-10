using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ActionFramework.Interfaces;

namespace ActionFramework.Classes
{
    public class Replacement
    {
        private string key;

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public Replacement(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Replace : List<Replacement>, IReplace
    {
        public void Add(string key, string value)
        {
            Replacement replacement = new Replacement(key, value);
            this.Add(replacement);
        }

        public string ReplaceFromText(string text)
        {
            foreach (Replacement r in this)
                text = RegExReplace(text, r.Key, r.Value);

            return text;
        }

        public string ReplaceFromFile(string file)
        {
            string text = ReadFile(file);

            foreach (Replacement r in this)
                text = RegExReplace(text, r.Key, r.Value);

            return text;
        }

        public string ReplaceFromFile(System.Xml.Linq.XDocument doc)
        {
            return ReplaceFromText(doc.ToString());
        }

        public string ReplaceFromFile(byte[] file)
        {
            return ReplaceFromText(System.Text.Encoding.UTF8.GetString(file));
        }

        public string ReplaceFromFile(MemoryStream file)
        {
            return ReplaceFromFile(file.ToArray());
        }

        private string ReadFile(string file)
        {
            StreamReader fp = new StreamReader(file, Encoding.Default);
            string filecontent = fp.ReadToEnd();
            fp.Close();
            return filecontent;
        }

        private string RegExReplace(string stringToReplace, string patternToReplace, string patternToReplaceWith)
        {
            return Regex.Replace(stringToReplace, patternToReplace, patternToReplaceWith, RegexOptions.CultureInvariant);
        }
    }
}
