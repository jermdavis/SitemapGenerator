using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SitemapGenerator.Core;
using Sitecore.FakeDb;
using SitemapGenerator.Core.Models;
using System.Linq;

namespace SitemapGenerator.Tests
{

    [TestClass]
    public class PublisherTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Publisher_Constructor_NullWriterThrows()
        {
            var p = new Publisher(null);
        }

        [TestMethod]
        public void Publisher_Constructor_MockWriterIsAllowed()
        {
            var w = new Mocks.MockISitemapDataWriter();
            var p = new Publisher(w);

            Assert.IsNotNull(p);
        }

        [TestMethod]
        public void Publisher_Constructor_DefaultConfigDatabaseIsMaster()
        {
            var w = new Mocks.MockISitemapDataWriter();
            var p = new Publisher(w);

            Assert.AreEqual("master", p.ConfigDatabase);
        }

        [TestMethod]
        public void Publisher_Constructor_CustomConfigDatabaseWorks()
        {
            using (Db db = new Db("web"))
            {
                db.Configuration.Settings[Publisher.ConfigDatabaseSetting] = "web";

                var w = new Mocks.MockISitemapDataWriter();
                var p = new Publisher(w);

                Assert.AreEqual("web", p.ConfigDatabase);
            }
        }

        [TestMethod]
        public void Publisher_Constructor_DefaultIsNormalWriter()
        {
            var w = new Publisher();
            Assert.IsInstanceOfType(w.Writer, typeof(DiskDataWriter));
        }

        [TestMethod]
        public void Publisher_Execute_SingleItemGeneratesCorrectXML()
        {
            var home = new DbItem("Home");

            var database = new DbItem("master");

            var cfgItem = new DbItem("ExampleSitemapFileDefinition")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "sitemap.xml" },
                    { "SourceDatabase", database.Name },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", "" },
                    { "TemplatesToInclude", "" }
                }
            };

            var moduleInfo = new DbItem("Modules")
            {
                ParentID = Sitecore.ItemIDs.SystemRoot,
                Children = {
                    new DbItem("SitemapGenerator") {
                        new DbItem("Databases") {
                            database
                        },
                        new DbItem("SitemapDefinitions", Configuration.SitemapDefinitionsFolderID) {
                            cfgItem
                        }
                    }
                }
            };

            using (Db db = new Db("master") {
                    home,
                    moduleInfo
                })
            {
                var sdw = new Mocks.MockISitemapDataWriter();
                var sp = new Publisher(sdw);

                sp.Publish(null, null);

                Assert.AreEqual(1, sdw.Calls);
                Assert.AreEqual("sitemap.xml", sdw.FileName);
                Assert.IsNotNull(sdw.Xml);

                string url = sdw.Xml.Document
                    .Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlSetElementName)
                    .Elements(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlElementName)
                    .First()
                    .Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlLocationElemenName)
                    .Value;
                Assert.AreEqual("/en/sitecore/content/Home.aspx", url);
            }

        }
    }

}