using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sensing.SDK
{ 
    public class DeviceResource
    {
        public int Id { get; set; }

        public int Sequence { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileUrl { get; set; }

        public string Thumbnail { get; set; }

        /// <summary>
        /// 分辨率 如1024*768
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// 比例 如16:9
         public string Proportion { get; set; }

        public string FileType { get; set; }

        public string MD5 { get; set; }
        public string Tag1Value { get; set; }
    }
}