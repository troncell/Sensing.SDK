using System;
using System.Collections.Generic;

namespace Sensing.Device.SDK.Dto.DeviceControl
{
    public class DeviceControlDto
    {
        public long? Id { get; set; }
        public long? DeviceId { get; set; }
        public string DeviceName { get; set; }
        public List<DeviceModelControlDto> DeviceModelDtos { get; set; }
    }
    
    public class DeviceModelControlDto
    {
        public string ShowModelName { get; set; }
        public int ShowModelColumn { get; set; }
        public int ShowModelRow { get; set; }
        public long ShowModelId { get; set; }
        public string ShowModelOuterId { get; set; }
        public string ShowModelDescribe { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public List<DeviceShowAreaInfoDto>  DeviceShowAreaInfos { get; set; }

    }
    public class DeviceShowAreaControlDto
    {
        public long Id { get; set; }
        public string ShowAreaControlName { get; set; }
        public string AreaAction { get; set; }
        public string IconUrl { get; set; }
        public bool Enable { get; set; }
        public string Data { get; set; }
        public int OrderNumber { get; set; }
        public long DeviceShowAreaId { get; set; }
        public string Describe { get; set; }
        public string ControlGroup { get; set; }
    }
    
    public class DeviceShowAreaInfoDto: DeviceModelDto
    {
        public long Id { get; set; }
        public int StartColumn { get; set; }
        public int StartRow { get; set; }
        public int SpanRow { get; set; }
        public int SpanColumn { get; set; }
        public int OrderNumber { get; set; }
        public string ShowAreaName { get; set; }
        public string Proportion { get; set; }
        public string Describe { get; set; }
        public long DeviceId { get; set; }
        public string OuterId { get; set; }
        public List<DeviceShowAreaControlDto> DeviceShowAreaControlDtos { get; set; }
    }
    
    public class DeviceModelDto
    {
        public long? Id { get; set; }
        public long? DeviceId { get; set; }
        public string ShowModelName { get; set; }
        public int ShowModelColumn { get; set; }
        public int ShowModelRow { get; set; }
        public long ShowModelId { get; set; }
        public string ShowModelOuterId { get; set; }
        public string ShowModelDescribe { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}