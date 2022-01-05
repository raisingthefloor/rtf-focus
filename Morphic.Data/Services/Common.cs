using Morphic.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Services
{
    public class Common
    {
        public static string LOG_FILE_NAME = "win-focus-{0:yy-MM-dd}.log";
        public static string SERVICE_LOG_FILE_NAME = "win-focus-service-{0:yy-MM-dd}.log";
        public static string SESSION_FILE_NAME = "session-{0}.json";
        public static string SESSION_SEARCH = "session*";
        public static string SETTINGS_FILE_NAME = "settings.json";
        public static string CATEGORIES_FILE_NAME = "categories.json";
        public static string APP_NAME = "Morphic Focus";

        private static int longBreakDuration = 60; //change to 120
        private static int min60 = 30; //change to 60
        private static int min30 = 15; //change to 30
        private static int min15 = 8; //change to 15
        private static int min10 = 5; //change to 10
        private static int min5 = 3; //change to 5
        private static int min3 = 3; 
        private static int min1 = 1; 
        private static int min20 = 10; //change to 20
        private static int min25 = 13; //change to 25
        private static int min45 = 20; //change to 45

        public static int LongBreakDuration { get => longBreakDuration; set => longBreakDuration = value; }
        public static int Min60 { get => min60; set => min60 = value; }
        public static int Min30 { get => min30; set => min30 = value; }
        public static int Min15 { get => min15; set => min15 = value; }
        public static int Min10 { get => min10; set => min10 = value; }
        public static int Min5 { get => min5; set => min5 = value; }
        public static int Min3 { get => min3; set => min3 = value; }
        public static int Min1 { get => min1; set => min1 = value; }
        public static int Min20 { get => min20; set => min20 = value; }
        public static int Min25 { get => min25; set => min25 = value; }
        public static int Min45 { get => min45; set => min45 = value; }

        public static string MakeFilePath(string fileName)
        {
            return Path.Combine(GetWinRootFolder(), string.Format(fileName, DateTime.Now));
        }

        public static string GetWinRootFolder()
        {
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            //ManagementObjectCollection collection = searcher.Get();
            //string username = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(appData, APP_NAME);
        }

        public static string[] GetSessionFiles()
        {
            return Directory.GetFiles(GetWinRootFolder(), SESSION_SEARCH);
        }

        public static string GetSessionFilePath(Session session)
        {
            return MakeFilePath(string.Format(SESSION_FILE_NAME, session.ActualStartTime.ToString("yyMMdd_HHmmss")));
        }
    }
}
