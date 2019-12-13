namespace whateverthefuck.src.util
{
    using System;

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
        public string Username;

        public bool ConnectToLocalHost;
    }
}
