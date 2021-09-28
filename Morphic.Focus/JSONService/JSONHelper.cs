using Morphic.Focus.Models;
using Morphic.Focus.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Morphic.Focus.JSONService
{
    public class JSONHelper
    {
        private static readonly object locker = new object();
        public string FileName { get; set; }

        public JSONHelper(string fileName)
        {
            FileName = fileName;
        }

        internal string Save<T>(T obj)
        {
            //Get Json File Path
            string path = GetJsonFilePath();

            //Write to Log File
            lock (locker)
            {
                string jsonString = JsonSerializer.Serialize<T>(obj);
                File.WriteAllText(path, jsonString);
                return jsonString;
            }
        }

        internal T Get<T>()
        {
            //Get Json File Path
            string path = GetJsonFilePath();

            //Write to Log File
            lock (locker)
            {
                string jsonString = File.ReadAllText(path);
                return JsonSerializer.Deserialize<T>(jsonString);
            }
        }
        

        private string GetJsonFilePath()
        {
            //Get Json File Path
            string path = Common.MakeFilePath(FileName);
            string folder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(folder);
            return path;
        }
    }
}
