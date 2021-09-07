using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class DeviceAppPodVersionModel
    {
        public int CurrentAppPodVersionId { get; set; }
        public string CurrentVersion { get; set; }
        public string CurrentDownloadUrl { get; set; }
        public int TargetAppPodVersionId { get; set; }
        public string TargetVersion { get; set; }
        public string TargetDownloadUrl { get; set; }
        public string AppPodName { get; set; }
        public string OS { get; set; }
        public string TargetVersionAppSetting { get; set; }
        public string Description { get; set; }
    }


    public class ChangeDeviceApppodVersionInput
    {
        public string Subkey { get; set; }
        public int CurrentAppPodVersionId { get; set; }
        public string OS { get; set; }
        public string AppPodName { get; set; }
    }

}


