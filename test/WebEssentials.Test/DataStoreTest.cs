using WebEssentials;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;
using static WebEssentials.DataStore;

namespace WebEssentials.Test
{
    [TestClass]
    public class DataStoreTest
    {
        private string _logFile;
        private ExtensionEntry _entry;
        private IRegistryKey _registry;

        [TestInitialize]
        public void Setup()
        {
            _logFile = Path.Combine(Path.GetTempPath(), "logfile.json");
            _entry = new ExtensionEntry { Id = "id" };
            _registry = new StaticRegistryKey();
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(_logFile);
        }

        [TestMethod]
        public void ExtensionInstalledNoLogFile()
        {
            var store = new DataStore(_registry, _logFile);
            store.MarkInstalled(_entry);

            Assert.AreEqual(1, store.Log.Count);
            Assert.IsFalse(File.Exists(_logFile));
            Assert.AreEqual(_entry.Id, store.Log[0].Id);
            Assert.AreEqual("Installed", store.Log[0].Action);
            Assert.IsTrue(store.HasBeenInstalled(_entry.Id));

            store.Save();
            Assert.IsTrue(File.Exists(_logFile));
        }

        [TestMethod]
        public void ExtensionUninstalledNoLogFile()
        {
            var store = new DataStore(_registry, _logFile);
            store.MarkUninstalled(_entry);
            store.Save();

            Assert.AreEqual(1, store.Log.Count);
            Assert.AreEqual(_entry.Id, store.Log[0].Id);
            Assert.AreEqual("Uninstalled", store.Log[0].Action);
            Assert.AreEqual(_entry.Id, _registry.GetValue("disable"));
        }

        [TestMethod]
        public void LogFileExist()
        {
            var msg = new[] { new LogMessage(_entry, "Installed") };

            var json = JsonConvert.SerializeObject(msg);
            File.WriteAllText(_logFile, json);

            var store = new DataStore(_registry, _logFile);

            Assert.IsTrue(store.HasBeenInstalled(_entry.Id));
            Assert.AreEqual(1, store.Log.Count);
            Assert.AreEqual(_entry.Id, store.Log[0].Id);
        }

        [TestMethod]
        public void Reset()
        {
            var msg = new[] { new LogMessage(_entry, "Installed") };

            var json = JsonConvert.SerializeObject(msg);
            File.WriteAllText(_logFile, json);

            var store = new DataStore(_registry, _logFile);

            Assert.AreEqual(1, store.Log.Count);

            bool success = store.Reset();

            Assert.IsTrue(success);
            Assert.AreEqual(0, store.Log.Count);
            Assert.IsFalse(File.Exists(_logFile));
        }
    }
}
