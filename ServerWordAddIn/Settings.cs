using System.Configuration;
using System.Reflection;

namespace ServerWordAddIn
{
    public static class Settings
    {
        private static readonly KeyValueConfigurationCollection Values = Load();

        private static KeyValueConfigurationCollection Load()
        {
            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config"
            };
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None)
                                       .AppSettings.Settings;
        }

        public static string BaseUrl => Values["BaseUrl"].Value;
        public static string ApiToken => Values["ApiToken"].Value;
    }
}