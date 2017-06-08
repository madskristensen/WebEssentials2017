using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebEssentials.Test
{
    [TestClass]
    public class LiveFeedTest
    {
        private string _localPath;

        [TestInitialize]
        public void Setup()
        {
            _localPath = Path.Combine(Path.GetTempPath(), "feed.json");
            File.Delete(_localPath);
        }

        [TestMethod]
        public async Task UpdateAsync()
        {
            var file = new FileInfo("..\\..\\artifacts\\feed.json");
            var feed = new LiveFeed(file.FullName, _localPath);

            await feed.UpdateAsync();
            File.Delete(_localPath);

            Assert.IsTrue(feed.Extensions.Count == 2);
            Assert.IsTrue(feed.Extensions[0].Name == "Add New File");
            Assert.IsTrue(feed.Extensions[0].Id == "2E78AA18-E864-4FBB-B8C8-6186FC865DB3");
            Assert.IsTrue(feed.Extensions[1].MaxVersion == new Version("16.0"));
        }

        [TestMethod]
        public async Task UpdateInvalidJSONAsync()
        {
            var feed = new LiveFeed("http://example.com", _localPath);

            await feed.UpdateAsync();

            Assert.IsTrue(feed.Extensions.Count == 0);
            Assert.IsFalse(File.Exists(_localPath));
        }

        [TestMethod]
        public async Task Update404Async()
        {
            var feed = new LiveFeed("http://asdlfkhasdflijsdflisjdfjoi23498734so08s0d8f.dk", _localPath);

            await feed.UpdateAsync();

            Assert.IsTrue(feed.Extensions.Count == 0);
            Assert.IsFalse(File.Exists(_localPath));
        }

        [TestMethod]
        public async Task ParsingAsync()
        {
            var feed = new LiveFeed("", _localPath);

            string content = @"{
            ""Add New File"": {
                ""id"": ""2E78AA18-E864-4FBB-B8C8-6186FC865DB3"",
                ""minVersion"": ""15.0""
                }
            }";

            File.WriteAllText(_localPath, content);

            await feed.ParseAsync();
            File.Delete(_localPath);

            Assert.IsTrue(feed.Extensions.Count == 1);
            Assert.IsTrue(feed.Extensions[0].Name == "Add New File");
            Assert.IsTrue(feed.Extensions[0].Id == "2E78AA18-E864-4FBB-B8C8-6186FC865DB3");
        }

        [TestMethod]
        public async Task ParsingInvalidJsonAsync()
        {
            var feed = new LiveFeed("", _localPath);

            string content = "invalid json";

            File.WriteAllText(_localPath, content);

            await feed.ParseAsync();
            File.Delete(_localPath);

            Assert.IsTrue(feed.Extensions.Count == 0);
        }
    }
}
