using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SitemapGenerator.Core.Models;
using SitemapGenerator.Core;
using System.Xml.Linq;
using System.Linq;

namespace SitemapGenerator.Tests
{

    [TestClass]
    public class UrlSerialiserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialise_NullUrlThrows()
        {
            var serialiser = new UrlSetSerialiser();

            serialiser.serialiseUrl(null);
        }

        [TestMethod]
        public void Serialise_DefaultUrlReturnsOnlyLocation()
        {
            string exampleUrl = "http://blah/";

            var serialiser = new UrlSetSerialiser();

            var url = new Url(exampleUrl);

            var xUrl = serialiser.serialiseUrl(url);

            Assert.IsNotNull(xUrl);

            Assert.AreEqual(1, xUrl.Elements().Count());

            var xLoc = xUrl.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlLocationElemenName);
            Assert.IsNotNull(xLoc);
            Assert.AreEqual(exampleUrl, xLoc.Value);
        }

        [TestMethod]
        public void Serialise_ProvidingLastModifiedDateOnlyFormatsCorrectly()
        {
            DateTime dt = new DateTime(2015, 01, 25);

            var serialiser = new UrlSetSerialiser();

            var url = new Url("123");
            url.LastModified = dt;

            var xUrl = serialiser.serialiseUrl(url);

            Assert.IsNotNull(xUrl);

            Assert.AreEqual(2, xUrl.Elements().Count());

            var xMod = xUrl.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlModifiedElementName);
            Assert.IsNotNull(xMod);
            Assert.AreEqual("2015-01-25", xMod.Value);
        }

        [TestMethod]
        public void Serialise_ProvidingLastModifiedDateWithTimeFormatsCorrectly()
        {
            DateTime dt = new DateTime(2015, 02, 27, 13, 12, 11, DateTimeKind.Utc);

            var serialiser = new UrlSetSerialiser();

            var url = new Url("123");
            url.LastModified = dt;

            var xUrl = serialiser.serialiseUrl(url);

            Assert.IsNotNull(xUrl);

            Assert.AreEqual(2, xUrl.Elements().Count());

            var xMod = xUrl.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlModifiedElementName);
            Assert.IsNotNull(xMod);
            Assert.AreEqual("2015-02-27T13:12:11Z", xMod.Value);
        }

        [TestMethod]
        public void Serialise_ProvidingChangeFrequencyFormatsCorrectly()
        {
            ChangeFrequency cf = ChangeFrequency.Hourly;

            var serialiser = new UrlSetSerialiser();

            var url = new Url("123");
            url.ChangeFrequency = cf;

            var xUrl = serialiser.serialiseUrl(url);

            Assert.IsNotNull(xUrl);

            Assert.AreEqual(2, xUrl.Elements().Count());

            var xChange = xUrl.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlChangeFrequencyElementName);
            Assert.IsNotNull(xChange);
            Assert.AreEqual("hourly", xChange.Value);
        }

        [TestMethod]
        public void Serialise_ProvidingPriorityFormatsCorrectly()
        {
            float priority = 0.7124f;

            var serialiser = new UrlSetSerialiser();

            var url = new Url("123");
            url.Priority = priority;

            var xUrl = serialiser.serialiseUrl(url);

            Assert.IsNotNull(xUrl);

            Assert.AreEqual(2, xUrl.Elements().Count());

            var xPriority = xUrl.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlPriorityElementName);
            Assert.IsNotNull(xPriority);
            Assert.AreEqual("0.7", xPriority.Value);
        }

        [TestMethod]
        public void Serialise_ProvidingEverythingFormatsCorrectly()
        {
            float priority = 0.195f;
            ChangeFrequency cf = ChangeFrequency.Weekly;
            DateTime dt = new DateTime(2014, 12, 05, 07, 55, 0, DateTimeKind.Utc);

            var serialiser = new UrlSetSerialiser();

            var url = new Url("123");
            url.Priority = priority;
            url.ChangeFrequency = cf;
            url.LastModified = dt;

            var xUrl = serialiser.serialiseUrl(url);

            Assert.IsNotNull(xUrl);

            Assert.AreEqual(4, xUrl.Elements().Count());

            var xPriority = xUrl.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlPriorityElementName);
            Assert.IsNotNull(xPriority);
            Assert.AreEqual("0.2", xPriority.Value);

            var xChange = xUrl.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlChangeFrequencyElementName);
            Assert.IsNotNull(xChange);
            Assert.AreEqual("weekly", xChange.Value);

            var xMod = xUrl.Element(UrlSetSerialiser.SitemapNamespace + UrlSetSerialiser.UrlModifiedElementName);
            Assert.IsNotNull(xMod);
            Assert.AreEqual("2014-12-05T07:55:00Z", xMod.Value);
        }

        [TestMethod]
        public void Serialise_AlternateURLsFormatCorrectly()
        {
            var serialiser = new UrlSetSerialiser();

            var url = new Url("123");
            url.AlternateUrls.Add(new AlternateUrl() { Language = "lng", Url = "url" });

            var xUrl = serialiser.serialiseUrl(url);
            Assert.IsNotNull(xUrl);

            var xLinks = xUrl.Elements(UrlSetSerialiser.XhtmlNamespace + "link");
            Assert.AreEqual(1, xLinks.Count());

            var xLink = xLinks.First();
            Assert.AreEqual("lng", xLink.Attribute("hreflang").Value);
            Assert.AreEqual("url", xLink.Attribute("href").Value);
        }
    }

}