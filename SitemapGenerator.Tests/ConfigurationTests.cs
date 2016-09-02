using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SitemapGenerator.Core;
using Sitecore.FakeDb;
using Sitecore.Data.Items;
using System.Linq;
using SitemapGenerator.Core.Models;

namespace SitemapGenerator.Tests
{
    [TestClass]
    public class ConfigurationTests
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Configuration_Fetch_PassingNulDBThrows()
        {
            var sc = new Configuration();
            var result = sc.Fetch(null);
        }

        [TestMethod]
        public void Configuration_Fetch_PassingEmptyDBWorks()
        {
            var database = new DbItem("master");

            var moduleInfo = new DbItem("Modules")
            {
                ParentID = Sitecore.ItemIDs.SystemRoot,
                Children = {
                    new DbItem("SitemapGenerator") {
                        new DbItem("Databases") {
                            database
                        },
                        new DbItem("SitemapDefinitions", Configuration.SitemapDefinitionsFolderID)
                    }
                }
            };

            using (Db db = new Db
                {
                    new DbItem("Home") { { "Title", "Welcome!" } },
                    moduleInfo
                })
            {
                var sc = new Configuration();
                var result = sc.Fetch(db.Database);

                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void Configuration_Fetch_PassingDBWithOneDefinitionWorks()
        {
            var template1 = new DbTemplate("Content1");
            var template2 = new DbTemplate("Content2");
            var template3 = new DbTemplate("Content3");

            var home = new DbItem("Home");

            var database = new DbItem("master");

            var language1 = new DbItem("en") { { "Iso", "en" } };
            var language2 = new DbItem("cy") { { "Iso", "cy" } };

            var cfgItem1 = new DbItem("ExampleSitemapFileDefinition")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "xyz" },
                    { "SourceDatabase", database.Name },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", language1.ID.ToString() + "|" + language2.ID.ToString() },
                    { "TemplatesToInclude", template1.ID.ToString() + "|" + template2.ID.ToString() + "|" + template3.ID.ToString()}
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
                            cfgItem1
                        }
                    }
                }
            };

            using (Db db = new Db("master")
                {
                    template1,
                    template2,
                    template3,
                    home,
                    language1,
                    language2,
                    moduleInfo
                })
            {
                var cfg = new Configuration();
                var maps = cfg.Fetch(db.Database);

                Assert.IsNotNull(maps);
                Assert.AreEqual(1, maps.Count());
            }
        }

        [TestMethod]
        public void Configuration_Fetch_PassingDBWithThreeDefinitionsWorks()
        {
            var template1 = new DbTemplate("Content1");
            var template2 = new DbTemplate("Content2");
            var template3 = new DbTemplate("Content3");

            var home = new DbItem("Home");

            var database = new DbItem("master");

            var language1 = new DbItem("en") { { "Iso", "en" } };
            var language2 = new DbItem("cy") { { "Iso", "cy" } };

            var cfgItem1 = new DbItem("ExampleSitemapFileDefinition1")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "xyz" },
                    { "SourceDatabase", database.Name },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", language1.ID.ToString() + "|" + language2.ID.ToString() },
                    { "TemplatesToInclude", template1.ID.ToString() + "|" + template2.ID.ToString() + "|" + template3.ID.ToString()}
                }
            };

            var cfgItem2 = new DbItem("ExampleSitemapFileDefinition2")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "abc" },
                    { "SourceDatabase", database.Name },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", language1.ID.ToString() },
                    { "TemplatesToInclude", template1.ID.ToString() }
                }
            };

            var cfgItem3 = new DbItem("ExampleSitemapFileDefinition3")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "ijk" },
                    { "SourceDatabase", database.Name },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", language2.ID.ToString() },
                    { "TemplatesToInclude", template1.ID.ToString() + "|" + template3.ID.ToString()}
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
                            cfgItem1,
                            cfgItem2,
                            cfgItem3
                        }
                    }
                }
            };

            using (Db db = new Db("master")
                {
                    template1,
                    template2,
                    template3,
                    home,
                    language1,
                    language2,
                    moduleInfo
                })
            {
                var cfg = new Configuration();
                var maps = cfg.Fetch(db.Database);

                Assert.IsNotNull(maps);
                Assert.AreEqual(3, maps.Count());
            }
        }

    }
}
