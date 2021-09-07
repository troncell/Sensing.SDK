using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class DeviceOutput
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Mac { get; set; }

        public OperatingType? OperatingType { get; set; }

        public bool IsLocked { get; set; }

        public string DeviceTypeName { get; set; }

        public string Address { get; set; }

        public string IntranetIP { get; set; }

        public string InternetIP { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 设备是否已被注册，如果已注册，另外的机器不能再注册. 同时管理员可以清空，允许其它设备进行注册.
        /// </summary>
        public bool IsRegistered { get; set; }

        public string LicenseInfo { get; set; }

        public string HardwareCode { get; set; }

        public DateTime? ShutdownTime { get; set; }

        public double Resolution_Width { get; set; }

        public double Resolution_Height { get; set; }

        /// <summary>
        /// 访问此设备的唯一凭证，通过Subkey可调用平台api，拿到设备相关的信息，如,ads,apps,products等.同时可用户注册.
        /// </summary>
        public string SubKey { get; set; }
        public string OS { get; set; }
        public string OnlineTrafficTarget { get; set; }
        public string TenantName { get; set; }
        public int TenantId { get; set; }
        public string OrganizationUnitName { get; set; }
        public int OrganizationUnitId { get; set; }
        public bool IsStore { get; set; }
        public string StoreOuterId { get; set; }

        public string OutType { get; set; }

        public string TaobaoDeviceId { get; set; }
        public string DeviceOuterId { get; set; }
    }

    public class DeviceSdkLoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mac { get; set; }

        /// <summary>
        /// 店员所属店铺id(group id)
        /// </summary>
        public int StoreId { get; set; }
    }

    public class DeviceStaffLoginResultViewModel
    {
        public string SubKey { get; set; }
        public int StaffId { get; set; }
        public string StaffOutId { get; set; }

        public bool IsNeedRegister { get; set; }

    }

    #region Enumerations.
    public enum DeviceStatus
    {
        Stopped = 0,
        Running = 1,
    }

    public enum OperatingType
    {
        Official,
        Test
    }

    public enum BussinessType
    {
        SelfBuyFromOthers,
        SelfBuyFromTronCell,
        RentFromOthers,
        RentFromTronCell,
    }

    public enum DeviceOutType
    {
        SensingStore,
        Taobao
    }
    #endregion
}
