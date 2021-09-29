using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.BlockService
{
    class Common
    {
        public static string LOG_FILE_NAME = "win-focus-service.log";
        public static string APP_NAME = "Morphic.Focus";
        public static string SESSION_FILE_NAME = "session.json";
        public static string MakeFilePath(string fileName)
        {
            return Path.Combine(GetAppDataFolder(), fileName);
        }

        static string GetAppDataFolder()
        {
            //string appData = Environment.GetFolderPath("C:\\Users\\kunal\\AppData\\Roaming\\");
            return Path.Combine("C:\\Users\\kunal\\AppData\\Roaming\\", APP_NAME);
        }
    }
}
