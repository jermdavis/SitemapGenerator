using SitemapGenerator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SitemapGenerator.Core
{

    public class UrlSetSerialiser
    {
        public static readonly XNamespace SitemapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        public static readonly XNamespace XhtmlNamespace = "http://www.w3.org/1999/xhtml";

        public static readonly string UrlSetElementName = "urlset";
        public static readonly string UrlElementName = "url";
        public static readonly string UrlLocationElemenName = "loc";
        public static readonly string UrlModifiedElementName = "lastmod";
        public static readonly string UrlChangeFrequencyElementName = "changefreq";
        public static readonly string UrlPriorityElementName = "priority";

        public static readonly string ModifiedDateFormatString = "{0:yyyy-MM-dd}";
        public static readonly string ModifiedDateTimeFormatString = "{0:yyyy-MM-ddTHH:mm:ssZ}";

        public static readonly string PriorityFormatString = "{0:0.0}";

        public XDocument Serialise(UrlSet urlSet)
        {
            if (urlSet == null)
            {
                throw new ArgumentNullException("urlSet");
            }

            XDocument xd = new XDocument();

            XElement root = new XElement(UrlSetElementName);
            root.Add(new XAttribute(XNamespace.Xmlns + "xhtml", XhtmlNamespace));
            root.Name = SitemapNamespace + root.Name.LocalName;
            xd.Document.Add(root);

            foreach(Url sitemapUrl in urlSet)
            {
                var xUrl = serialiseUrl(sitemapUrl);
                xUrl.Name = SitemapNamespace + xUrl.Name.LocalName;

                root.Add(xUrl);
            }

            return xd;
        }

        internal XElement serialiseUrl(Url url)
        {
            if(url == null)
            {
                throw new ArgumentNullException("url");
            }

            XElement xe = new XElement(UrlElementName);

            var xLoc = new XElement(
                UrlLocationElemenName, 
                url.Location);
            xLoc.Name = SitemapNamespace + xLoc.Name.LocalName;
            xe.Add(xLoc);

            if (url.LastModified.HasValue)
            {
                string fs;

                if (url.LastModified.Value.Hour > 0 || url.LastModified.Value.Minute > 0 || url.LastModified.Value.Second > 0)
                {
                    fs = ModifiedDateTimeFormatString;
                }
                else
                {
                    fs = ModifiedDateFormatString;
                }

                var xMod = new XElement(
                    UrlModifiedElementName, 
                    string.Format(fs, url.LastModified.Value.ToUniversalTime()));
                xMod.Name = SitemapNamespace + xMod.Name.LocalName;
                xe.Add(xMod);
            }

            if(url.ChangeFrequency.HasValue)
            {
                var xFreq = new XElement(
                    UrlChangeFrequencyElementName, 
                    url.ChangeFrequency.Value.ToString().ToLower());
                xFreq.Name = SitemapNamespace + xFreq.Name.LocalName;
                xe.Add(xFreq);
            }

            if(url.Priority.HasValue)
            {
                var xPriority = new XElement(
                    UrlPriorityElementName, 
                    string.Format(PriorityFormatString, url.Priority));
                xPriority.Name = SitemapNamespace + xPriority.Name.LocalName;
                xe.Add(xPriority);
            }

            foreach(var alternate in url.AlternateUrls)
            {
                xe.Add(serialiseAlternate(alternate));
            }

            return xe;
        }

        internal XElement serialiseAlternate(AlternateUrl alternate)
        {
            if(alternate == null)
            {
                throw new ArgumentNullException("alternate");
            }

            XElement xAlt = new XElement(XhtmlNamespace + "link");
            xAlt.Add(new XAttribute("rel", "alternate"));

            xAlt.Add(new XAttribute("hreflang", alternate.Language));
            xAlt.Add(new XAttribute("href", alternate.Url));

            return xAlt;
        }
    }

}