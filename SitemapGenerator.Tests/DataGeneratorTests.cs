using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SitemapGenerator.Core;
using SitemapGenerator.Core.Models;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using System.Linq;

namespace SitemapGenerator.Tests
{

    [TestClass]
    public class DataGeneratorTests
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DataGenerator_Generate_PassingNullThrows()
        {
            DataGenerator sc = new DataGenerator();
            sc.Generate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataGenerator_Generate_FailureToResolveLanguagesThrows()
        {
            var sfd = FileDefinition.Empty;

            DataGenerator sc = new DataGenerator();
            var xd = sc.Generate(sfd);
        }

        [TestMethod]
        public void DataGenerator_Generate_ReturnsAUrlSet()
        {
            var sfd = FileDefinition.Empty;

            using (Db db = new Db())
            {
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var xd = sc.Generate(sfd);

                Assert.IsNotNull(xd);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_SingleItemWorks()
        {
            var home = new DbItem("home");
            using (Db db = new Db
                {
                    home
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(1, urlSet.Count);

                var url = urlSet.First();

                Assert.AreEqual("/en/sitecore/content/home.aspx", url.Location);
                Assert.AreEqual(0, url.AlternateUrls.Count);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_ParentAndChildWorks()
        {
            var home = new DbItem("home")
            {
                new DbItem("child")
            };

            using (Db db = new Db
                {
                    home
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(2, urlSet.Count);

                var url1 = urlSet[0];

                Assert.AreEqual("/en/sitecore/content/home.aspx", url1.Location);
                Assert.AreEqual(0, url1.AlternateUrls.Count);

                var url2 = urlSet[1];

                Assert.AreEqual("/en/sitecore/content/home/child.aspx", url2.Location);
                Assert.AreEqual(0, url2.AlternateUrls.Count);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_OnlyTemplateItemsReturned()
        {
            var template = new DbTemplate("ContentTemplate");
            var home = new DbItem("home")
            {
                new DbItem("child")
                {
                    TemplateID = template.ID
                }
            };
            using (Db db = new Db
                {
                    template,
                    home
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.TemplatesToInclude.Add(template.ID.ToGuid());
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(1, urlSet.Count);

                var url = urlSet.First();

                Assert.AreEqual("/en/sitecore/content/home/child.aspx", url.Location);
                Assert.AreEqual(0, url.AlternateUrls.Count);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_NoItemsOfRightTemplateReturnsNothing()
        {
            var template = new DbTemplate("ContentTemplate");
            var home = new DbItem("home");
            using (Db db = new Db
                {
                    template,
                    home
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.TemplatesToInclude.Add(template.ID.ToGuid());
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(0, urlSet.Count);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_MultiLanguageItemGeneratesRightReturn()
        {
            var template = new DbTemplate("ContentTemplate");
            var home = new DbItem("home")
            {
                new DbField("Title")
                {
                    { "en", "English" },
                    { "cy", "Welsh" }
                }
            };
            var language1 = new DbItem("en") { { "Iso", "en" }, { "Regional Iso Code", "" } };
            var language2 = new DbItem("cy") { { "Iso", "cy" }, { "Regional Iso Code", "cy" } };
            using (Db db = new Db
                {
                    template,
                    home,
                    language1,
                    language2
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.LanguagesToInclude.Add(language1.ID.ToGuid());
                sfd.LanguagesToInclude.Add(language2.ID.ToGuid());
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(1, urlSet.Count);
                Assert.AreEqual(1, urlSet.First().AlternateUrls.Count);
                Assert.AreEqual("cy", urlSet.First().AlternateUrls.First().Language);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_MultiLanguageWithOnlyOneSelectedLanguageItemGeneratesRightReturn()
        {
            var template = new DbTemplate("ContentTemplate");
            var home = new DbItem("home")
            {
                new DbField("Title")
                {
                    { "en", "English" },
                    { "cy", "Welsh" }
                }
            };
            var language1 = new DbItem("en") { { "Iso", "en" }, { "Regional Iso Code", "" } };
            var language2 = new DbItem("cy") { { "Iso", "cy" }, { "Regional Iso Code", "cy" } };
            using (Db db = new Db
                {
                    template,
                    home,
                    language1,
                    language2
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.LanguagesToInclude.Add(language1.ID.ToGuid());
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(1, urlSet.Count);
                Assert.AreEqual(0, urlSet.First().AlternateUrls.Count);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_MultiLanguageWithIncorrectItemLanguageIsNotRecorded()
        {
            var template = new DbTemplate("ContentTemplate");
            var home = new DbItem("home")
            {
                new DbField("Title")
                {
                    { "fr-fr", "French" },
                    { "ru-ru", "Russian" }
                }
            };
            var language1 = new DbItem("en-us") { { "Iso", "en" }, { "Regional Iso Code", "en-US" } };
            var language2 = new DbItem("cy") { { "Iso", "cy" }, { "Regional Iso Code", "cy" } };
            using (Db db = new Db
                {
                    template,
                    home,
                    language1,
                    language2
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.LanguagesToInclude.Add(language1.ID.ToGuid());
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(0, urlSet.Count);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_MultiLanguageWithMixedItemLanguagesAreRecordedCorrectly()
        {
            var template = new DbTemplate("ContentTemplate");
            var home = new DbItem("home")
            {
                new DbField("Title")
                {
                    { "en", "English" }
                },
                new DbItem("child")
                {
                    new DbField("Title")
                    {
                        { "en", "English" },
                        { "mi-NZ", "Maori" }
                    }
                }
            };

            var language1 = new DbItem("en") { { "Iso", "en" }, { "Regional Iso Code", "" } };
            var language2 = new DbItem("mi-NZ") { { "Iso", "mi" }, { "Regional Iso Code", "mi-NZ" } };
            using (Db db = new Db
                {
                    template,
                    home,
                    language1,
                    language2
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.LanguagesToInclude.Add(language1.ID.ToGuid());
                sfd.LanguagesToInclude.Add(language2.ID.ToGuid());
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(2, urlSet.Count);

                Assert.AreEqual(0, urlSet[0].AlternateUrls.Count);
                Assert.AreEqual(1, urlSet[1].AlternateUrls.Count);
                Assert.AreEqual("mi-NZ", urlSet[1].AlternateUrls.First().Language);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_PriotityEtcExtractedIfAvailable()
        {
            var template = new DbTemplate("ContentTemplate") {
                new DbField("ChangeFrequency", DataGenerator.ChangeFrequencyFieldID),
                new DbField("Priority", DataGenerator.PriorityFieldID)
            };
            var home = new DbItem("home") {
                TemplateID = template.ID,
                Fields = {
                    { "ChangeFrequency", "hourly" },
                    { "Priority", "0.1" }
                }
            };
            using (Db db = new Db
                {
                    template,
                    home
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(1, urlSet.Count);

                var url = urlSet.First();
                Assert.AreEqual(0.1f, url.Priority);
                Assert.AreEqual(ChangeFrequency.Hourly, url.ChangeFrequency);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_DefaultsForPriorityEtcIfNotAvailable()
        {
            var template = new DbTemplate("ContentTemplate");
            var home = new DbItem("home");
            using (Db db = new Db
                {
                    template,
                    home
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(1, urlSet.Count);

                var url = urlSet.First();
                Assert.AreEqual(0.5f, url.Priority);
                Assert.AreEqual(ChangeFrequency.Daily, url.ChangeFrequency);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_CorrectLastModifiedFieldExtracted()
        {
            var template = new DbTemplate("ContentTemplate") {
                new DbField("__Updated", Sitecore.FieldIDs.Updated)
            };
            var home = new DbItem("home") {
                { "__Updated", "15 Feb 2017 01:17:20" }
            };
            using (Db db = new Db
                {
                    template,
                    home
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.ResolveLanguages(db.Database);

                DataGenerator sc = new DataGenerator();
                var urlSet = sc.Generate(sfd);

                Assert.IsNotNull(urlSet);
                Assert.AreEqual(1, urlSet.Count);

                var url = urlSet.First();
                Assert.IsTrue(url.LastModified.HasValue);
                Assert.AreEqual(15, url.LastModified.Value.Day);
                Assert.AreEqual(2, url.LastModified.Value.Month);
            }
        }

        [TestMethod]
        public void DataGenerator_Generate_ForeignLastModifiedDateWorks()
        {
            var template = new DbTemplate("ContentTemplate") {
                new DbField("__Updated", Sitecore.FieldIDs.Updated)
            };
            var home = new DbItem("home") {
                { "__Updated", "03/17/2017 01:17:20" }
            };
            using (Db db = new Db
                {
                    template,
                    home
                })
            {
                var sfd = FileDefinition.Empty;
                sfd.FilenameToGenerate = "sitemap.xml";
                sfd.RootItem = home.ID.ToGuid();
                sfd.SourceDatabase = "master";
                sfd.ResolveLanguages(db.Database);

                using (new CultureChanger("en-us"))
                {
                    DataGenerator sc = new DataGenerator();
                    var urlSet = sc.Generate(sfd);

                    Assert.IsNotNull(urlSet);
                    Assert.AreEqual(1, urlSet.Count);

                    var url = urlSet.First();
                    Assert.IsTrue(url.LastModified.HasValue);
                    Assert.AreEqual(17, url.LastModified.Value.Day);
                    Assert.AreEqual(3, url.LastModified.Value.Month);
                }
            }
        }
    }

}