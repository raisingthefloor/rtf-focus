using System.IO;
using System.Text.Json;
using System.Threading;

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
                    string jsonString = string.Empty;
                    for (int i = 1; i <= 100; ++i)
                    {
                        try
                        {
                            jsonString = string.Empty;
                            jsonString = File.ReadAllText(path);
                            break;
                        }
                        catch (System.IO.IOException ex) when (i <= 100)
                        {
                            // You may check error code to filter some exceptions, not every error
                            // can be recovered.
                            Thread.Sleep(1000);
                        }
                    }
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
                string jsonString = string.Empty;
                for (int i = 1; i <= 100; ++i)
                {
                    try
                    {
                        jsonString = string.Empty;
                        jsonString = File.ReadAllText(path);
                        break;
                    }
                    catch (System.IO.IOException ex) when (i <= 100)
                    {
                        // You may check error code to filter some exceptions, not every error
                        // can be recovered.
                        Thread.Sleep(1000);
                    }
                }
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

                for (int i = 1; i <= 100; ++i)
                {
                    try
                    {
                        File.WriteAllText(path, jsonString);
                        break;
                    }
                    catch (System.IO.IOException ex) when (i <= 100)
                    {
                        // You may check error code to filter some exceptions, not every error
                        // can be recovered.
                        Thread.Sleep(1000);
                    }
                }
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
