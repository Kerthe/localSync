namespace LocalSync.Identity
{
    public interface IDeviceIdentityProvider
    {
        public Task<DeviceIdentity> GetOrInitializeIdentityAsync();

        public Task UpdateDisplayNameAsync(string newName);
    }
}
