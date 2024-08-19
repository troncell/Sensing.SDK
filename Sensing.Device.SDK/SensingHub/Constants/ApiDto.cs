namespace Sensing.Device.SDK.SensingHub.Constants
{
    public class ApiDto<T>
    {
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public object error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public T result { get; set; }
    }
    
}