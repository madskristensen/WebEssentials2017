using WebEssentials;

namespace WebEssentials.Test
{
    public class StaticRegistryKey : IRegistryKey
    {
        private object _value;

        public IRegistryKey CreateSubKey(string subKey)
        {
            return this;
        }

        public void Dispose()
        {
            //
        }

        public object GetValue(string name)
        {
            return _value;
        }

        public void SetValue(string name, object value)
        {
            _value = value;
        }
    }
}
