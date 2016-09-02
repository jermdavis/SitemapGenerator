using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SitemapGenerator.Core.Models;
using SitemapGenerator.Core;

namespace SitemapGenerator.Tests
{

    [TestClass]
    public class AltUrlSerialiserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AltUrlSerialiser_Serialise_NullAlternativeThrows()
        {
            var serialiser = new UrlSetSerialiser();

            serialiser.serialiseAlternate(null);
        }

        [TestMethod]
        public void AltUrlSerialiser_Serialise_AlternateGeneratesCorrectElement()
        {
            var serialiser = new UrlSetSerialiser();

            AlternateUrl url = new AlternateUrl();
            url.Language = "l";
            url.Url = "u";

            var xAlt = serialiser.serialiseAlternate(url);

            Assert.IsNotNull(xAlt);
            Assert.AreEqual("<link rel=\"alternate\" hreflang=\"l\" href=\"u\" xmlns=\"http://www.w3.org/1999/xhtml\" />", xAlt.ToString(System.Xml.Linq.SaveOptions.None));
        }
    }

}