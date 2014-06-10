using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Wordbook.Data
{
    public static class SettingService
    {
        static SettingService()
        {
            var name = ConfigurationManager.AppSettings["SettingFileName"];

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Could not found application setting name");
            }

            SettingService._filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);

            if (!File.Exists(SettingService._filePath))
            {
                using (var file = File.CreateText(SettingService._filePath))
                {
                    file.WriteLine("{}");
                    file.Flush();
                }
            }

            SettingService.Current = Newtonsoft.Json.JsonConvert.DeserializeObject<Setting>(SettingService.ReadSettingFile());
        }

        private static string ReadSettingFile()
        {
            return File.ReadAllText(SettingService._filePath);
        }

        private static string _filePath;

        public static Setting Current { get; set; }

        public static void Save()
        {
            File.WriteAllText(SettingService._filePath, Newtonsoft.Json.JsonConvert.SerializeObject(SettingService.Current));
        }
    }
}