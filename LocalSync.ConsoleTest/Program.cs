using LocalSync.Identity;


string testStoragePath = Path.Combine(Environment.CurrentDirectory, "TestStorage");

Console.WriteLine(testStoragePath);

IDeviceIdentityProvider identityProvider = new LocalDeviceIdentity(testStoragePath);

DeviceIdentity identity = await identityProvider.GetOrInitializeIdentityAsync();

Console.WriteLine(identity.DeviceId);
Console.WriteLine(identity.DeviceName);

await identityProvider.UpdateDisplayNameAsync("AnotherName");

Console.WriteLine(identity.DeviceId);
Console.WriteLine(identity.DeviceName);
