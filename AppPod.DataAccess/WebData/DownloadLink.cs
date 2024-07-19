using System;
using System.Collections.Generic;
using System.Text;

namespace AppPod.DataAccess.WebData
{
    public class DownloadLink
    {
        public string Host { get; set; }
        public string RelativeFileName { get; set; }
        public string Type { get; set; }
        public string Md5 { get; set; }
    }
}
