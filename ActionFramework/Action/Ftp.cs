using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.IO;
using ActionFramework.Events;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using ActionFramework.Enum;

namespace ActionFramework.Actions
{
    public class Ftp : ActionBase
    {
        public override object Execute()
        {
            try
            {
                string fileName = Prop("FileName");
                CreateXmlFile(fileName);
                SendXmlFile(fileName);
                return HandleSuccess();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return base.Execute();
        }

        private void CreateXmlFile(string fileName)
        {
            string directory = Prop("Directory");
            
            using (SqlConnection conn = new SqlConnection(Prop("ConnectionString")))
            {
                string cmd = Prop("Query");
                SqlDataAdapter daImport = new SqlDataAdapter(cmd, conn);

                using (DataSet ds = new DataSet())
                {
                    conn.Open();
                    daImport.Fill(ds);
                    conn.Close();
                    ds.Tables[0].WriteXml(directory + "\\" + fileName + ".xml");
                }
            }
        }

        private void SendXmlFile(string fileName)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://cllp.asuscomm.com/sdb1/Downloads/" + fileName + ".xml");
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("admin", "lillHH77");

            // Copy the contents of the file to the request stream.
            StreamReader sourceStream = new StreamReader(Prop("Directory") + "\\" + fileName + ".xml");
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Log.Info(String.Format("Upload File Complete, status {0}", response.StatusDescription));

            response.Close();
        }
    }
}
