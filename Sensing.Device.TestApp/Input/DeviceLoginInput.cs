namespace Sensing.Device.TestApp.Input;

public class DeviceLoginInput
{
    public string Type { get; set; }
    public DeviceAuth CommonText { get; set; }
}

public class DeviceAuth
{
    public long DeviceId { get; set; }
    public string SubKey { get; set; }
    public long ControlId { get; set; }
}

public class AreaLockInput
{
    public string Type { get; set; }
    public AreaLock CommonText { get; set; }
}

public class AreaLock
{
    public long HallAreaId { get; set; }
}

public class AreaControlInput
{
    public string Type { get; set; }
    public AreaControl CommonText { get; set; }
}

public class AreaControl
{
    public long HallAreaId { get; set; }
    public long ControlId { get; set; }
}

public class DeviceControlInput
{
    public string Type { get; set; }
    public DeviceControl CommonText { get; set; }
}

public class DeviceControl
{
    public long DeviceId { get; set; }
    public long ControlId { get; set; }
    public string SubKey { get; set; }
}
public class DeviceLockInput
{
    public string Type { get; set; }
    public DeviceLock CommonText { get; set; }
}

public class DeviceLock
{
    public long DeviceId { get; set; }
    public string SubKey { get; set; }
}