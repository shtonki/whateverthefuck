namespace whateverthefuck.src.util
{
    using System;

    public static class UserSettings
    {
        private static string configPath = "config.json";

        public static ConfigInfo Config { get; set; }

        public static void LoadUserSettings()
        {
            try
            {
                Config = JsonIO.ReadFromJsonFile<ConfigInfo>(configPath);
                Logging.Log("Loaded config from file");
            }
            catch (Exception)
            {
                Config = new ConfigInfo();
                Config.Username = "default";
                JsonIO.WriteToJsonFile<ConfigInfo>(configPath, Config);
                Logging.Log("Created default config file");
            }
        }
    }

    public class ConfigInfo
    {
        public string Username { get; set; }

        public bool ConnectToLocalHost { get; set; }
    }
}
