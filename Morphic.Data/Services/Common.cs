﻿using System;
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
        public static string LOG_FILE_NAME = "win-focus-{0:d}.log";
        public static string SERVICE_LOG_FILE_NAME = "win-focus-service-{0:d}.log";
        public static string SESSION_FILE_NAME = "session.json";
        public static string SETTINGS_FILE_NAME = "settings.json";
        public static string APP_NAME = "Morphic.Focus";

        public static string MakeFilePath(string fileName)
        {
            return Path.Combine(GetWinRootFolder(), string.Format(fileName, DateTime.Now));
        }

        static string GetWinRootFolder()
        {
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            //ManagementObjectCollection collection = searcher.Get();
            //string username = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(appData, APP_NAME);
        }


    }
}