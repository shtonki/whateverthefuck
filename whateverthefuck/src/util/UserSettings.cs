using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using whateverthefuck.src.network;

namespace whateverthefuck.src.util
{
    public static class UserSettings
    {
        public static ConfigInfo Config { get; set; }

        private static string ConfigPath = "config.json";

        public static void LoadUserSettings()
        {
            try
            {
                Config = JsonIO.ReadFromJsonFile<ConfigInfo>(ConfigPath);
                Logging.Log("Loaded config from file");
            }
            catch (Exception)
            {
                Config = new ConfigInfo();
                Config.Username = "default";
                JsonIO.WriteToJsonFile<ConfigInfo>(ConfigPath, Config);
                Logging.Log("Created default config file");
            }
        }
    }

    public class ConfigInfo
    {
        //public LoginCredentials LoginCredentials;
        public string Username;

        public bool ConnectToLocalHost;
    }
}
