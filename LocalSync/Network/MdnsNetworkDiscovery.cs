using Makaretu.Dns;

namespace LocalSync.Network
{
    public class MdnsNetworkDiscovery : INetworkDiscovery, IDisposable
    {
        private const string ServiceName = "_localsync._tcp";
        private readonly ServiceDiscovery _serviceDiscovery;
        private ServiceProfile? _myProfile;

        public event EventHandler<PeerInfo>? PeerDiscovered;

        public MdnsNetworkDiscovery()
        {
            _serviceDiscovery = new ServiceDiscovery();

            _serviceDiscovery.ServiceInstanceDiscovered += OnServiceDiscovered;
        }

        public void StartBroadcasting(string deviceId, string deviceName, int tcpPort)
        {
            string instanceName = $"{deviceName}_{deviceId.Substring(0, 8)}";

            _myProfile = new ServiceProfile(instanceName, ServiceName, (ushort)tcpPort);

            _myProfile.AddProperty("DeviceId", deviceId);
            _myProfile.AddProperty("DeviceName", deviceName);

            _serviceDiscovery.Advertise(_myProfile);
        }

        public void StartListening()
        {
            _serviceDiscovery.QueryServiceInstances(ServiceName);
        }

        public void OnServiceDiscovered(object? sender, ServiceInstanceDiscoveryEventArgs e)
        {
            if (!e.ServiceInstanceName.ToString().Contains(ServiceName))
                return;

            var ipAddress = e.Message.AdditionalRecords
                .OfType<ARecord>()
                .Select(a => a.Address)
                .FirstOrDefault();

            var txtRecords = e.Message.AdditionalRecords.OfType<TXTRecord>().FirstOrDefault();
            string peerDeviceId = string.Empty;
            string peerDeviceName = e.ServiceInstanceName.ToString();

            if (txtRecords != null)
            {
                foreach (var str in txtRecords.Strings)
                {
                    if (str.StartsWith("DeviceId="))
                        peerDeviceId = str.Substring(9);
                    if (str.StartsWith("DeviceName="))
                        peerDeviceName= str.Substring(11);
                }
            }

            var srvRecord = e.Message.AdditionalRecords.OfType<SRVRecord>().FirstOrDefault();
            int port = srvRecord?.Port ?? 0;

            if (ipAddress != null && port > 0 && !String.IsNullOrEmpty(peerDeviceId))
            {
                var peer = new PeerInfo
                {
                    DeviceId = peerDeviceId,
                    DeviceName = peerDeviceName,
                    Port = port,
                    IpAddress = ipAddress
                };

                PeerDiscovered?.Invoke(this, peer);
            }
        }

        public void Stop()
        {
            if (_myProfile != null)
            {
                _serviceDiscovery.Unadvertise(_myProfile);
                _myProfile = null;
            }
        }

        public void Dispose() 
        {
            Stop();
            _serviceDiscovery.Dispose();
        }
    }
}
