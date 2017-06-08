using WebEssentials;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            var registry = new StaticRegistryKey();
            var store = new DataStore(new StaticRegistryKey(), Constants.LogFile);
            var feed = new LiveFeed(registry, Constants.LiveFeedUrl, _cachePath);

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
            var hasUpdates = await _installer.CheckForUpdatesAsync();

            Assert.IsTrue(hasUpdates);
        }

        [TestMethod]
        public async Task CheckForUpdatesValidCacheAsync()
        {
            File.WriteAllText(_cachePath, "{}");
            var hasUpdates = await _installer.CheckForUpdatesAsync();

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

            var tooLow = _installer.GetExtensionsMarkedForDeletion(new Version(14, 0));
            Assert.AreEqual(1, tooLow.Count());
            Assert.AreEqual("2E78AA18-E864-4FBB-B8C8-6186FC865DB3", tooLow.ElementAt(0).Id);

            var tooHigh = _installer.GetExtensionsMarkedForDeletion(new Version(16, 0));
            Assert.AreEqual(1, tooHigh.Count());
            Assert.AreEqual("2E78AA18-E864-4FBB-B8C8-6186FC865DB3", tooHigh.ElementAt(0).Id);

            var lowerBounce = _installer.GetExtensionsMarkedForDeletion(new Version(15, 0));
            Assert.IsFalse(lowerBounce.Any());

            var upperBounce = _installer.GetExtensionsMarkedForDeletion(new Version(15, 2));
            Assert.IsFalse(upperBounce.Any());

            var middle = _installer.GetExtensionsMarkedForDeletion(new Version(15, 1));
            Assert.IsFalse(middle.Any());
        }
    }
}
