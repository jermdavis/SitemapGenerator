using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SitemapGenerator.Core;
using SitemapGenerator.Core.Models;
using System.Xml.Linq;
using System.Linq;

namespace SitemapGenerator.Tests
{

    [TestClass]
    public class UrlSetSerialiserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialise_ThrowsWithNullSource()
        {
            var serialiser = new UrlSetSerialiser();

            serialiser.Serialise(null);
        }

        [TestMethod]
        public void Serialise_EmptyUrlSetGivesValidEmptyDocument()
        {
            var urlSet = new UrlSet();
            var serialiser = new UrlSetSerialiser();

            XDocument result = serialiser.Serialise(urlSet);
            Assert.IsNotNull(result, "Serialise() should always return a valid XML document");

            Assert.AreEqual("<urlset xmlns:xhtml=\"http://www.w3.org/1999/xhtml\" xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" />", result.ToString(SaveOptions.DisableFormatting));
        }

        [TestMethod]
        public void Serialise_SingleUrlGivesValidDocument()
        {
            var urlSet = new UrlSet();
            urlSet.Add(new Url("123"));

            var serialiser = new UrlSetSerialiser();

            XDocument result = serialiser.Serialise(urlSet);
            Assert.IsNotNull(result, "Serialise() should always return a valid XML document");

            var set = result.Document.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlSetElementName);
            var urls = set.Elements(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlElementName);

            Assert.AreEqual(1, urls.Count());

            var loc = urls.First().Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlLocationElemenName);

            Assert.AreEqual("123", loc.Value);
        }

        [TestMethod]
        public void Serialise_MultipleUrlsGivesCorrectDocument()
        {
            var urlSet = new UrlSet();
            for(int i=0; i<10; i++)
            {
                urlSet.Add(new Url(i.ToString()));
            }

            var serialiser = new UrlSetSerialiser();

            XDocument result = serialiser.Serialise(urlSet);
            Assert.IsNotNull(result, "Serialise() should always return a valid XML document");

            var set = result.Document.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlSetElementName);
            var urls = set.Elements(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlElementName);

            Assert.AreEqual(10, urls.Count());
        }

        [TestMethod]
        public void Serialise_OverallDocumentHasCorrectFormat()
        {
            var urlSet = new UrlSet();
            var url = new Url("http://www.com/") { ChangeFrequency = ChangeFrequency.Daily };
            url.AlternateUrls.Add(new AlternateUrl() { Url = "http://www.com/en/", Language = "EN-GB" });
            urlSet.Add(url);

            var serialiser = new UrlSetSerialiser();

            var xDoc = serialiser.Serialise(urlSet);

            string s = xDoc.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting);

            Assert.AreEqual("<urlset xmlns:xhtml=\"http://www.w3.org/1999/xhtml\" xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"><url><loc>http://www.com/</loc><changefreq>daily</changefreq><xhtml:link rel=\"alternate\" hreflang=\"EN-GB\" href=\"http://www.com/en/\" /></url></urlset>", s);
        }
    }

}