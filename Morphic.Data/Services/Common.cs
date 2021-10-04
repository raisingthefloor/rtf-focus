using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Services
{
    public class Common
    {
        public static string LOG_FILE_NAME = "win-focus.log";
        public static string SERVICE_LOG_FILE_NAME = "win-focus-service.log";
        public static string SESSION_FILE_NAME = "session.json";
        public static string SETTINGS_FILE_NAME = "settings.json";
        public static string APP_NAME = "Morphic.Focus";

        public static string MakeFilePath(string fileName)
        {
            return Path.Combine(GetWinRootFolder(), fileName);
        }

        static string GetWinRootFolder()
        {
            string appData = Path.GetPathRoot(Environment.SystemDirectory);
            return Path.Combine(appData, APP_NAME);
        }
    }
}
