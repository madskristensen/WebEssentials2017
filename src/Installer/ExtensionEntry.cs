using System;

namespace WebEssentials
{
    public class ExtensionEntry
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public Version MinVersion { get; set; }
        public Version MaxVersion { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
