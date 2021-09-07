using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.Models
{
    public class AppInfo
    {
        public long Id { get; set; }
        /// <summary>
        /// MainPage App Logo.
        /// </summary>
        public string Logo { get; set; }
        public string Name { get; set; }

        public long SoftwareId { get; set; }
        public string ExePath { get; set; }
        public string Package { get; set; }
        public string Code { get; set; }
        public bool IsDefault { get; set; }

        public string MaterialPackage { get; set; }
        public string Alias { get; set; }
        public string TenantMaterialPackage { get; set; }
    }
}
