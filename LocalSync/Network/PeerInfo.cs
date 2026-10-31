using System.Net;

namespace LocalSync.Network
{
    public class PeerInfo
    {
        public required string DeviceId { get; set; }
        public required string DeviceName { get; set; }
        public required int Port { get; set; }
        public IPAddress IpAddress { get; set; } = IPAddress.None;
    }
}
