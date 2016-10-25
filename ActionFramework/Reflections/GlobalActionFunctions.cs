using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;

namespace ActionFramework.Classes
{
    public class GlobalActionFunctions
    {
        public string GetExecutionRoot(string keyfolder)
        {
            string[] executionPath = System.Reflection.Assembly.GetExecutingAssembly().Location.Split('\\');
            StringBuilder sb = new StringBuilder();
            foreach (string s in executionPath)
            {
                if (!s.Contains(keyfolder))
                {
                    sb.Append(s + "\\");
                }
                else
                {
                    sb.Append(keyfolder + "\\");
                    break;
                }
            }

            return sb.ToString();
        }

        public string GetExecutionRoot()
        {
            string[] paths = System.Reflection.Assembly.GetExecutingAssembly().Location.Split('\\');
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < paths.Count() - 1; i++)
            {
                sb.Append(paths[i] + "\\");
            }

            return sb.ToString();
        }

        public string GetExecutionParentRoot()
        {
          string[] paths = System.Reflection.Assembly.GetExecutingAssembly().Location.Split('\\');
          StringBuilder sb = new StringBuilder();

          for(int i = 0; i < paths.Count() - 2; i++)
          {
            sb.Append(paths[i] + "\\");
          }

          return sb.ToString();
        }

        public string GetExecutionPath(int levelsFromEnd)
        {
            string[] paths = System.Reflection.Assembly.GetExecutingAssembly().Location.Split('\\');
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < paths.Count() - levelsFromEnd; i++)
            {
                sb.Append(paths[i] + "\\");
            }

            return sb.ToString();
        }

        public string GetCurrentFormatDateTimeString()
        {
            DateTime currentDate = DateTime.Now;
            return string.Format("{0:yyyy-MM-dd_HH:mm:ss}", currentDate).Replace(":", "");
        }
    }
}
