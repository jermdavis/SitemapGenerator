using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SitemapGenerator.Core.Models;
using Sitecore.FakeDb;
using Sitecore.Data.Items;

namespace SitemapGenerator.Tests
{

    [TestClass]
    public class FileDefinitionTests
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileDefinition_Constructor_PassingNullThrows()
        {
            var sfd = new FileDefinition(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FileDefinition_Constructor_PassingWrongItemTypeThrows()
        {
            using (Db db = new Db
                {
                    new DbItem("Home") { { "Title", "Welcome!" } }
                })
            {
                Item home = db.GetItem("/sitecore/content/home");

                var sfd = new FileDefinition(home);
            }
        }

        [TestMethod]
        public void FileDefinition_Constructor_PassingValidConfigItemParsesOK()
        {
            var cfgTemplate = new DbTemplate("SitemapFileDefinition", FileDefinition.SitemapFileDefinitionID) { "FilenameToGenerate", "SourceDatabase", "RootItem", "LanguagesToInclude", "TemplatesToInclude" };
            var template1 = new DbTemplate("Content1");
            var template2 = new DbTemplate("Content2");
            var template3 = new DbTemplate("Content3");
            var dbItem = new DbItem("master");
            var home = new DbItem("Home");
            var language1 = new DbItem("en") { { "Iso", "en" }, { "Regional Iso Code", "" } };
            var language2 = new DbItem("cy") { { "Iso", "cy" }, { "Regional Iso Code", "cy" } };
            var cfgItem = new DbItem("ExampleSitemapFileDefinition")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "xyz" },
                    { "SourceDatabase", dbItem.Name },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", language1.ID.ToString() + "|" + language2.ID.ToString() },
                    { "TemplatesToInclude", template1.ID.ToString() + "|" + template2.ID.ToString() + "|" + template3.ID.ToString()}
                }
            };

            using (Db db = new Db
                {
                    cfgTemplate,
                    template1,
                    template2,
                    template3,
                    home,
                    dbItem,
                    language1,
                    language2,
                    cfgItem
                })
            {
                Item cfg = db.GetItem("/sitecore/content/ExampleSitemapFileDefinition");
                Assert.IsNotNull(cfg);

                var sfd = new FileDefinition(cfg);

                Assert.AreEqual("xyz", sfd.FilenameToGenerate);
                Assert.AreEqual("master", sfd.SourceDatabase);
                Assert.AreEqual(home.ID.ToGuid(), sfd.RootItem);
                Assert.AreEqual(2, sfd.LanguagesToInclude.Count);
                Assert.AreEqual(3, sfd.TemplatesToInclude.Count);

                sfd.ResolveLanguages(db.Database);
                Assert.AreEqual(sfd.LanguagesToInclude.Count, sfd.LanguageCodesToInclude.Count);
                Assert.AreEqual("en", sfd.LanguageCodesToInclude[0]);
                Assert.AreEqual("cy", sfd.LanguageCodesToInclude[1]);
            }
        }

        [TestMethod]
        public void FileDefinition_Constructor_PassingEmptyLanguagesAndTemplatesWorks()
        {
            var cfgTemplate = new DbTemplate("SitemapFileDefinition", FileDefinition.SitemapFileDefinitionID) { "FilenameToGenerate", "SourceDatabase", "RootItem", "LanguagesToInclude", "TemplatesToInclude" };
            var home = new DbItem("Home");
            var cfgItem = new DbItem("ExampleSitemapFileDefinition")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "xyz" },
                    { "SourceDatabase", "" },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", "" },
                    { "TemplatesToInclude", ""}
                }
            };

            using (Db db = new Db
                {
                    cfgTemplate,
                    home,
                    cfgItem
                })
            {
                Item cfg = db.GetItem("/sitecore/content/ExampleSitemapFileDefinition");
                Assert.IsNotNull(cfg);

                var sfd = new FileDefinition(cfg);

                Assert.AreEqual(0, sfd.LanguagesToInclude.Count);
                Assert.AreEqual(0, sfd.TemplatesToInclude.Count);
            }
        }

        [TestMethod]
        public void FileDefinition_Constructor_PassingEmptyRootWorks()
        {
            var cfgTemplate = new DbTemplate("SitemapFileDefinition", FileDefinition.SitemapFileDefinitionID) { "FilenameToGenerate", "SourceDatabase", "RootItem", "LanguagesToInclude", "TemplatesToInclude" };
            var cfgItem = new DbItem("ExampleSitemapFileDefinition")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "xyz" },
                    { "SourceDatabase", "" },
                    { "RootItem", "" },
                    { "LanguagesToInclude", "" },
                    { "TemplatesToInclude", ""}
                }
            };

            using (Db db = new Db
                {
                    cfgTemplate,
                    cfgItem
                })
            {
                Item cfg = db.GetItem("/sitecore/content/ExampleSitemapFileDefinition");
                Assert.IsNotNull(cfg);

                var sfd = new FileDefinition(cfg);

                Assert.AreEqual(Guid.Empty, sfd.RootItem);
            }
        }

        [TestMethod]
        public void FileDefinition_Constructor_LanguageResolvedFlagStartsFalseForEmpty()
        {
            var sfd = FileDefinition.Empty;
            Assert.AreEqual(false, sfd.LanguagesResolved);
        }

        [TestMethod]
        public void FileDefinition_Constructor_LanguagesResolvedStartsFalseWhenParsing()
        {
            var cfgTemplate = new DbTemplate("SitemapFileDefinition", FileDefinition.SitemapFileDefinitionID) { "FilenameToGenerate", "SourceDatabase", "RootItem", "LanguagesToInclude", "TemplatesToInclude" };
            var home = new DbItem("Home");
            var cfgItem = new DbItem("ExampleSitemapFileDefinition")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "xyz" },
                    { "SourceDatabase", "" },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", "" },
                    { "TemplatesToInclude", ""}
                }
            };

            using (Db db = new Db
                {
                    cfgTemplate,
                    home,
                    cfgItem
                })
            {
                Item cfg = db.GetItem("/sitecore/content/ExampleSitemapFileDefinition");
                Assert.IsNotNull(cfg);

                var sfd = new FileDefinition(cfg);

                Assert.AreEqual(false, sfd.LanguagesResolved);
            }
        }

        [TestMethod]
        public void FileDefinition_ResolveLanguages_SetsLanguagesResolvedFlagTrue()
        {
            var cfgTemplate = new DbTemplate("SitemapFileDefinition", FileDefinition.SitemapFileDefinitionID) { "FilenameToGenerate", "SourceDatabase", "RootItem", "LanguagesToInclude", "TemplatesToInclude" };
            var home = new DbItem("Home");
            var cfgItem = new DbItem("ExampleSitemapFileDefinition")
            {
                TemplateID = FileDefinition.SitemapFileDefinitionID,
                Fields = {
                    { "FilenameToGenerate", "xyz" },
                    { "SourceDatabase", "" },
                    { "RootItem", home.ID.ToString() },
                    { "LanguagesToInclude", "" },
                    { "TemplatesToInclude", ""}
                }
            };

            using (Db db = new Db
                {
                    cfgTemplate,
                    home,
                    cfgItem
                })
            {
                Item cfg = db.GetItem("/sitecore/content/ExampleSitemapFileDefinition");
                Assert.IsNotNull(cfg);

                var sfd = new FileDefinition(cfg);
                sfd.ResolveLanguages(db.Database);

                Assert.AreEqual(true, sfd.LanguagesResolved);
            }
        }
    }

}