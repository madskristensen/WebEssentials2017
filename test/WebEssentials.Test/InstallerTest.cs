using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebEssentials.Test
{
    /// <summary>
    /// Summary description for InstallerTest
    /// </summary>
    [TestClass, ]
    public class InstallerTest
    {
        private Installer _installer;
        private string _cachePath;

        [TestInitialize]
        public void TestSetup()
        {
            _cachePath = Path.Combine(Path.GetTempPath(), "cache.json");
            var store = new DataStore(new StaticRegistryKey(), Constants.LogFilePath);
            var feed = new LiveFeed(Constants.LiveFeedUrl, _cachePath);

            _installer = new Installer(feed, store);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            File.Delete(_cachePath);
        }

        [TestMethod]
        public async Task CheckForUpdatesNoCacheAsync()
        {
            File.Delete(_cachePath);
            bool hasUpdates = await _installer.CheckForUpdatesAsync();

            Assert.IsTrue(hasUpdates);
        }

        [TestMethod]
        public async Task CheckForUpdatesValidCacheAsync()
        {
            File.WriteAllText(_cachePath, "{}");
            bool hasUpdates = await _installer.CheckForUpdatesAsync();

            Assert.IsFalse(hasUpdates);
        }

        [TestMethod]
        public async Task GetExtensionsMarkedForDeletionAsync()
        {
            string content = @"{
            ""Add New File"": {
                ""id"": ""2E78AA18-E864-4FBB-B8C8-6186FC865DB3"",
                ""minVersion"": ""15.0"",
                ""maxVersion"": ""15.2""
                }
            }";

            File.WriteAllText(_cachePath, content);
            await _installer.LiveFeed.ParseAsync();

            IEnumerable<ExtensionEntry> tooLow = _installer.GetExtensionsMarkedForDeletion(new Version(14, 0));
            Assert.AreEqual(1, tooLow.Count());
            Assert.AreEqual("2E78AA18-E864-4FBB-B8C8-6186FC865DB3", tooLow.ElementAt(0).Id);

            IEnumerable<ExtensionEntry> tooHigh = _installer.GetExtensionsMarkedForDeletion(new Version(16, 0));
            Assert.AreEqual(1, tooHigh.Count());
            Assert.AreEqual("2E78AA18-E864-4FBB-B8C8-6186FC865DB3", tooHigh.ElementAt(0).Id);

            IEnumerable<ExtensionEntry> lowerBounce = _installer.GetExtensionsMarkedForDeletion(new Version(15, 0));
            Assert.IsFalse(lowerBounce.Any());

            IEnumerable<ExtensionEntry> upperBounce = _installer.GetExtensionsMarkedForDeletion(new Version(15, 2));
            Assert.IsFalse(upperBounce.Any());

            IEnumerable<ExtensionEntry> middle = _installer.GetExtensionsMarkedForDeletion(new Version(15, 1));
            Assert.IsFalse(middle.Any());
        }
    }
}
