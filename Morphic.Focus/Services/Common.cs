using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Services
{
    class Common
    {
        public static string LOG_FILE_NAME = "win-focus.log";
        public static string SESSION_FILE_NAME = "session.json";
        public static string SETTINGS_FILE_NAME = "settings.json";
        public static string APP_NAME = "Morphic.Focus";

        public static string MakeFilePath(string fileName)
        {
            return Path.Combine(GetAppDataFolder(), fileName);
        }

        static string GetAppDataFolder()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, APP_NAME);
        }
    }
}
