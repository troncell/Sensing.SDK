using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.Setting
{
    public class SettingsManager
    {
        public static Settings Settings;
        private const string SettingFile = "Settings/Settings.json";
        static SettingsManager()
        {
            var path = Path.Combine(Environment.CurrentDirectory, SettingFile);
            Settings = ReadSettings(path);
        }

        private SettingsManager()
        {

        }

        private static Settings ReadSettings(string jsonFile)
        {
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<Settings>(jsondata);
            }
            return null;
        }
        public static string GetFilePath(string relativePath)
        {
            return Path.Combine(Environment.CurrentDirectory, "Settings", relativePath);
        }
    }

    public class CommonConfig
    {
        public string Background_V { get; set; }
        public string Background_H { get; set; }
        public string DefaultAppIcon { get; set; }
        public string RegMode { get; set; }
    }

    public class Apps
    {
        public bool IsLocalOnly { get; set; } = false;
        public bool IsSupportLocalApps { get; set; } = false;
    }

    public class Products
    {
        public bool IsLocalOnly { get; set; } = false;
    }

    public class Ads
    {
        public bool IsLocalOnly { get; set; } = false;
    }

    public class TargetResolution
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class Settings
    {
        public  Apps Apps { get; set; }
        public Products Products { get; set; }
        public CommonConfig CommonConfig { get; set; }
        public Ads Ads { get; set; }
        public TargetResolution TargetResolution { get; set; }
    }

}
