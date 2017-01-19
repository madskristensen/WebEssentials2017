using System.Collections.Generic;

namespace WebExtensionPack
{
    class ExtensionList
    {
        public static IDictionary<string, string> Products()
        {
            return new Dictionary<string, string> {
                //{ "5fb7364d-2e8c-44a4-95eb-2a382e30fec9", "Web Essentials" },
                { "148ffa77-d70a-407f-892b-9ee542346862", "Web Compiler"},
                { "bf95754f-93d3-42ff-bfe3-e05d23188b08", "Image optimizer"},
                //{ "950d05f7-bb25-43ce-b682-44b377b5307d", "Glyphfriend"},
                { "f4ab1e64-5d35-4f06-bad9-bf414f4b3bbb", "Open Command Line"},
                { "fdd64809-376e-4542-92ce-808a8df06bcc", "Package Installer"},
                { "10d9b3af-1338-4c45-bc99-4ec38c3a11fb", "Browser Sync"},
                { "2d8aa02a-8810-421f-97b9-86efc573fea3", "Browser Reload on Save"},
                { "2a20580c-7be4-4440-bcd6-8dcf5aa2004e", "JavaScript Snippet Pack" },
                //{ "51b81721-cf4e-4ce0-a595-972b1ca2a186", "Suggested Extensions" },
                { "2E78AA18-E864-4FBB-B8C8-6186FC865DB3", "Add New File" },
                { "25a79d25-0fff-4748-afaa-3a67ed116bc9", "Web Accessibility Checker" },
                { "a0ae318b-4f07-4f71-93cb-f21d3f03c6d3", "Bundler & Minifier" },
                { "6c799bc4-0d4c-4172-98bc-5d464b612dca", "File Nesting" },
                { "a3112f81-e423-4f88-9f2c-e089a309e48e", "Editor Enhancements" },
                { "cd92c0c6-2c32-49a3-83ca-0dc767c7d78e", "Image Sprites" },
                { "9ca64947-e9ca-4543-bfb8-6cce9be19fd6", "Markdown Editor" },
                { "7f30a50b-8211-40cf-b881-bd1eb2866478", "HTML Snippet Pack" },
                { "3a7b4930-a5fb-46ec-a9b8-9610c8f953b8", "File Icons" },
                { "4773ce75-6f30-4269-9557-1f7c30a47be2", "Syntax Highlighting Pack" },
                { "3cef2919-d8c7-4e9f-a809-5a0ba9c61fac", "HTML Tools" },
                { "0020efc9-e999-4e6f-a2b6-604127f480bc", "CSS Tools" },
                { "1fd37423-142f-4267-8221-93163d573b90", "Package Security Alerts" },
                { "cad7b20b-4b83-4ca6-bf24-ca36a494241c", "TypeScript Definition Generator" },
            };
        }
    }
}
