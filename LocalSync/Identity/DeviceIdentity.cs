namespace LocalSync.Identity
{
    public class DeviceIdentity
    {
        public required string DeviceId { get; set; }
        public string DeviceName { get; set; } = Environment.MachineName;
    }
}
