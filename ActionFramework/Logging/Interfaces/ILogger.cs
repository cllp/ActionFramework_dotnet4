using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Entities;
using ActionFramework.Enum;
using ActionFramework.Interfaces;
using System.Xml.Linq;

namespace ActionFramework.Logging
{
    public interface ILogger
    {
        IAction Action
        {
            get;
            set;
        }

        List<LogElements> ListOfElements
        {
            get;
        }

        //void Add(LogType type, string message);
        //void Add(LogType type, Exception ex);
        //void Add(LogType type, string message, Exception ex);
        //void Add(LogElements elements);
        //void Add(LogType type, ILogElement log);
        //void Add(string xml, string description);

        string Write();

        string Write(bool remote);

        string Write(string path);

        string Write(string path, bool remote);

        XDocument ToXml
        {
            get;
        }

        void Info(string s);
        void Warning(string s);
        void Warning(string s, Exception ex);
        void Error(string s);
        void Error(string s, Exception ex);
        void Error(Exception ex);
        void Add(LogElements elements);
        void Custom(ILogElement log);
        void Custom(object obj);
        //void Custom(LogType type, ILogElement log);
        //void Custom(string type, ILogElement log);
        //void Custom(string xml, string description);
    }
}