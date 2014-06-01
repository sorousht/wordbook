using System;
using System.Configuration;

namespace Wordbook.Services
{
    public static class ApplicationSettings
    {
        private static string _dbFilePath;
        public static string DbFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ApplicationSettings._dbFilePath))
                {
                    ApplicationSettings._dbFilePath = ConfigurationManager.AppSettings["DbFilePath"];
                }
                return ApplicationSettings._dbFilePath;
            }
        }

        private static bool? _isdbFilePathAbsolute;
        public static bool IsDbFilePathAbsolute
        {
            get
            {
                if (!ApplicationSettings._isdbFilePathAbsolute.HasValue)
                {
                    ApplicationSettings._isdbFilePathAbsolute = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDbFilePathAbsolute"]);
                }

                return ApplicationSettings._isdbFilePathAbsolute.Value;
            }
        }
    }
}