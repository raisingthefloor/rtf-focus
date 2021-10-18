using System.IO;
using System.Text.Json;

namespace Morphic.Data.Services
{
    public class JSONHelper
    {
        private static readonly object locker = new object();
        public string FileName { get; set; }

        public JSONHelper(string fileName)
        {
            FileName = fileName;
        }

        #region GET Data from file
        public T Get<T>()
        {
            //Get Json File Path
            string path = GetJsonFilePath();

            //Get data from file
            lock (locker)
            {
                if (File.Exists(path))
                {
                    string jsonString = File.ReadAllText(path);
                    return JsonSerializer.Deserialize<T>(jsonString);
                }
                else
                {
                    return default;
                }
            }
        }

        public string GetJson<T>()
        {
            //Get Json File Path
            string path = GetJsonFilePath();

            //Write to Log File
            lock (locker)
            {
                string jsonString = File.ReadAllText(path);
                return jsonString;
            }
        }

        #endregion

        #region SET Data to file

        public string Save<T>(T obj)
        {
            //Get Json File Path
            string path = GetJsonFilePath();

            //Write to Log File
            lock (locker)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize<T>(obj, options);

                File.WriteAllText(path, jsonString);
                return jsonString;
            }
        }

        #endregion

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
