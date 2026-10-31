using System.Text.Json;

namespace LocalSync.Identity
{
    public class LocalDeviceIdentity : IDeviceIdentityProvider
    {
        private const string ConfigFileName = "localsync_config.json";
        private readonly string _storagePath;

        private DeviceIdentity? _currentIdentity;

        public LocalDeviceIdentity(string storagePath)
        {
            _storagePath = storagePath;
        }

        public async Task<DeviceIdentity> GetOrInitializeIdentityAsync()
        {
            if (_currentIdentity != null)
                return _currentIdentity;

            string fullPath = Path.Combine(_storagePath, ConfigFileName);

            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                var identity = JsonSerializer.Deserialize<DeviceIdentity>(json);

                if (identity != null)
                {
                    _currentIdentity = identity;
                    return _currentIdentity;
                }
            }

            _currentIdentity = new DeviceIdentity 
            {
                DeviceId = Guid.NewGuid().ToString(),
                DeviceName = Environment.MachineName
            };

            var newJson = JsonSerializer.Serialize(_currentIdentity);
            Directory.CreateDirectory(_storagePath);
            await File.WriteAllTextAsync(fullPath, newJson);

            return _currentIdentity;
        }

        public async Task UpdateDisplayNameAsync(string newName)
        {
            var identity = await GetOrInitializeIdentityAsync();

            if (identity.DeviceName == newName)
                return;

            identity.DeviceName = newName;
            string fullPath = Path.Combine(_storagePath, ConfigFileName);
            string json = JsonSerializer.Serialize(identity);

            await File.WriteAllTextAsync(fullPath, json);
        }

    }
}

