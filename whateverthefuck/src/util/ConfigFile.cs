using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using whateverthefuck.src.network;

namespace whateverthefuck.src.util
{
    public class ConfigFile
    {
        public ConfigInfo ConfigInfo { get; set; }

        public ConfigFile(string path)
        {
            try
            {
                ConfigInfo = JsonIO.ReadFromJsonFile<ConfigInfo>(path);
            }
            catch (Exception e)
            {
                ConfigInfo = new ConfigInfo();
                ConfigInfo.LoginCredentials = new LoginCredentials("Mankey");
            }
        }
    }

    public class ConfigInfo
    {
        public LoginCredentials LoginCredentials;
    }
}
