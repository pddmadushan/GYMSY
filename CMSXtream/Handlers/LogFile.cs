using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSXtream
{
    public class LogFile
    {
        public void MyLogFile(Exception ex)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = path + "//ERROR_LOG";
            
            if (!(Directory.Exists(path)))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            string fileName = path + "//ERRORS.txt";
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Close();
            }

            using (StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Append)))
            {
                writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace + "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }

        }


    }
}
