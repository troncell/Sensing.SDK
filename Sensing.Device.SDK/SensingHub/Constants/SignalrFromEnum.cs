using System.ComponentModel.DataAnnotations;

namespace Sensing.Device.SDK.SensingHub.Constants
{
    public enum SignalrFromEnum
    {
        [Display(Name = "无人超市")]
        StoreHub,
        [Display(Name = "展厅-pad")]
        HallAreaPad,
        [Display(Name = "展厅-设备")]
        HallAreaDevice,
    }
}