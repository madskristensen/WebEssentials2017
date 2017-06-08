using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebEssentials
{
    public class DataStore
    {
        private const string _installed = "Installed";
        private const string _uninstalled = "Uninstalled";

        private static string _logFile;
        private IRegistryKey _key;

        public DataStore(IRegistryKey key, string filePath)
        {
            _logFile = filePath;
            _key = key;

            Initialize();
        }

        internal List<LogMessage> Log { get; private set; } = new List<LogMessage>();

        public void MarkInstalled(ExtensionEntry extension)
        {
            Log.Add(new LogMessage(extension, _installed));
        }

        public void MarkUninstalled(ExtensionEntry extension)
        {
            Log.Add(new LogMessage(extension, _uninstalled));
        }

        public bool HasBeenInstalled(string id)
        {
            return Log.Any(ext => ext.Id == id && ext.Action == _installed);
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(Log);
            File.WriteAllText(_logFile, json);

            UpdateRegistry();
        }

        public bool Reset()
        {
            try
            {
                File.Delete(_logFile);
                Log.Clear();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return false;
            }
        }

        private void Initialize()
        {
            try
            {
                if (File.Exists(_logFile))
                {
                    Log = JsonConvert.DeserializeObject<List<LogMessage>>(File.ReadAllText(_logFile));
                    UpdateRegistry();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                File.Delete(_logFile);
            }
        }

        private void UpdateRegistry()
        {
            string uninstall = string.Join(";", Log.Where(l => l.Action == _uninstalled).Select(l => l.Id));

            using (_key.CreateSubKey(Constants.RegistrySubKey))
            {
                _key.SetValue("disable", uninstall);
            }
        }

        internal struct LogMessage
        {
            public LogMessage(ExtensionEntry entry, string action)
            {
                Id = entry.Id;
                Name = entry.Name;
                Action = action;
                Date = DateTime.Now;
            }

            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public string Action { get; set; }

            public override string ToString()
            {
                return $"{Date.ToString("yyyy-MM-dd")} {Action} {Name}";
            }
        }
    }
}
