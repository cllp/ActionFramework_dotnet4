using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.Collections;
using System.IO;

namespace ActionFramework.Action
{
    public class ZipAction : ActionBase
    {
        private void ZipFiles(string inputFolderPath, string outputPathAndFile, string password)
        {
            //ArrayList ar = GenerateFileList(inputFolderPath); // generate file list
            //int TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
            //// find number of chars to remove     // from orginal file path
            //TrimLength += 1; //remove '\'
            //FileStream ostream;
            //byte[] obuffer;
            //string outPath = inputFolderPath + @"\" + outputPathAndFile;
            //ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream
            //if (password != null && password != String.Empty)
            //    oZipStream.Password = password;
            //oZipStream.SetLevel(9); // maximum compression
            //ZipEntry oZipEntry;
            //foreach (string Fil in ar) // for each file, generate a zipentry
            //{
            //    oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
            //    oZipStream.PutNextEntry(oZipEntry);

            //    if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
            //    {
            //        ostream = File.OpenRead(Fil);
            //        obuffer = new byte[ostream.Length];
            //        ostream.Read(obuffer, 0, obuffer.Length);
            //        oZipStream.Write(obuffer, 0, obuffer.Length);
            //    }
            //}
            //oZipStream.Finish();
            //oZipStream.Close();
        }


        private ArrayList GenerateFileList(string Dir)
        {
            //ArrayList fils = new ArrayList();
            //bool Empty = true;
            //foreach (string file in Directory.GetFiles(Dir)) // add each file in directory
            //{
            //    fils.Add(file);
            //    Empty = false;
            //}

            //if (Empty)
            //{
            //    if (Directory.GetDirectories(Dir).Length == 0)
            //    // if directory is completely empty, add it
            //    {
            //        fils.Add(Dir + @"/");
            //    }
            //}

            //foreach (string dirs in Directory.GetDirectories(Dir)) // recursive
            //{
            //    foreach (object obj in GenerateFileList(dirs))
            //    {
            //        fils.Add(obj);
            //    }
            //}
            //return fils; // return file list
            throw new NotImplementedException();
        }


       
    
    }
}
