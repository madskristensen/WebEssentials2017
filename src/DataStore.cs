using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebExtensionPack
{
    public class DataStore
    {
        private static string _configFile;

        public DataStore()
        {
            _configFile = Environment.ExpandEnvironmentVariables("%userprofile%\\.webextensionpack");
            Initialize();
        }

        public List<string> PreviouslyInstalledExtensions { get; private set; } = new List<string>();

        public bool HasBeenInstalled(string productId)
        {
            return PreviouslyInstalledExtensions.Contains(productId);
        }

        public void Save()
        {
            File.WriteAllLines(_configFile, PreviouslyInstalledExtensions);
        }

        public bool Reset()
        {
            try
            {
                File.Delete(_configFile);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
        }

        private void Initialize()
        {
            try
            {
                if (File.Exists(_configFile))
                    PreviouslyInstalledExtensions = File.ReadAllLines(_configFile).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
