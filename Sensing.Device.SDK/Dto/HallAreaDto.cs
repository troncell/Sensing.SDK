using System;
using System.Collections.Generic;

namespace Sensing.Device.SDK.Dto
{
    public class HallAreaDto
    {
        public long Id { get; set; }
        public string HallAreaName { get; set; }
        public string IconUrl { get; set; }
        public string BackgroundUrl { get; set; }
        public long StoreId { get; set; }
        public string Describe { get; set; }
        public string OuterId { get; set; }
        public string XLocation { get; set; }
        public string YLocation { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public int OrderNumber { get; set; }
        public DateTime CreationTime { get; set; }
        public List<HallAreaDevicesDto> Devices { get; set; }
        public List<HallAreaControlDto> HallAreaControlDtos { get; set; }
    }
    
    public class HallAreaDevicesDto
    {
        public long DeviceId { get; set; }
        public long HallAreaId { get; set; }
    }
    
    public class HallAreaControlDto
    {
        public long Id { get; set; }
        public long HallAreaId { get; set; }
        public string HallAreaControlName { get; set; }
        public string SendType { get; set; }
        public bool Enable { get; set; }
        public string HallAction { get; set; }
        public string Data { get; set; }
        public int OrderNumber { get; set; }
        public string Describe { get; set; }
        public string IconUrl { get; set; }
    }
}