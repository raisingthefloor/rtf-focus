using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.BlockService
{
    class LoggingService
    {
        private static readonly object locker = new object();

        public static void WriteToLog(string message)
        {
            try
            {
                //Get Log File Path
                string path = Common.MakeFilePath(Common.LOG_FILE_NAME);
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
            catch (Exception ex)
            {

            }

        }
    }
}
