using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Services
{
    public class LoggingService
    {
        private static readonly object locker = new object();

        public static void WriteAppLog(string message)
        {
            //Get Log File Path
            string path = Common.MakeFilePath(Common.LOG_FILE_NAME); 
            WriteLog(message, path);
        }

        public static void WriteServiceLog(string message)
        {
            //Get Log File Path
            string path = Common.MakeFilePath(Common.SERVICE_LOG_FILE_NAME);
            WriteLog(message, path);
        }

        public static void WriteLightAppLog(string message)
        {
            //Get Log File Path
            string path = Common.MakeFilePath(Common.LIGHTAPP_LOG_FILE_NAME);
            WriteLog(message, path);
        }

        private static void WriteLog(string message, string path)
        {
            string folder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(folder);

            //Write to Log File
            lock (locker)
            {
                StreamWriter SW;
                SW = File.AppendText(path);
                SW.WriteLine();
                SW.WriteLine(DateTime.Now.ToString() + " " + message);
                SW.Close();
            }
        }

    }
}
