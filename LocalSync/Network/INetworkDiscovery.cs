namespace LocalSync.Network
{
    public interface INetworkDiscovery
    {
        event EventHandler<PeerInfo> PeerDiscovered;

        void StartBroadcasting(string deviceId, string deviceName, int tcpPort);

        void StartListening();

        void Stop();
    }
}
