using Microsoft.Win32;

namespace WebEssentials
{
    public class RegistryKeyWrapper : IRegistryKey
    {
        private RegistryKey _key;

        public RegistryKeyWrapper(RegistryKey key)
        {
            _key = key;
        }
        public IRegistryKey CreateSubKey(string subKey)
        {
            return new RegistryKeyWrapper(_key.CreateSubKey(subKey));
        }

        public object GetValue(string name)
        {
            return _key.GetValue(name);
        }

        public void SetValue(string name, object value)
        {
            _key.SetValue(name, value);
        }

        public void Dispose()
        {
            _key.Dispose();
        }
    }
}
