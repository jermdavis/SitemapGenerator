using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using SitemapGenerator.Core.Models;
using System;
using System.Xml.Linq;

namespace SitemapGenerator.Core
{

    public class DataGenerator
    {
        public static readonly ID ChangeFrequencyFieldID = new ID("C24998E6-F08D-4283-8C3B-53D9AAF136CD");
        public static readonly ID PriorityFieldID = new ID("260442BD-4E99-473E-82EF-2853A012FAA4");

        public UrlSet Generate(FileDefinition definition)
        {
            if(definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            if(!definition.LanguagesResolved)
            {
                throw new ArgumentException("The SitemapFileDefinition passed has not resolved languages prior to sitemap generation.", "definition");
            }

            UrlSet urlSet = new UrlSet();

            if (definition.RootItem != Guid.Empty)
            {
                Database db = Sitecore.Configuration.Factory.GetDatabase(definition.SourceDatabase);

                Item rootItem = db.GetItem(new ID(definition.RootItem));

                process(rootItem, urlSet, definition);
            }

            return urlSet;
        }

        private void process(Item itm, UrlSet urlSet, FileDefinition definition)
        {
            if (definition.TemplatesToInclude.Count == 0 || definition.TemplatesToInclude.Contains(itm.TemplateID.ToGuid()))
            {
                var url = makeUrl(itm, definition);
                urlSet.Add(url);
            }

            foreach(Item child in itm.Children)
            {
                process(child, urlSet, definition);
            }
        }

        private Url makeUrl(Item itm, FileDefinition definition)
        {
            string url = LinkManager.GetItemUrl(itm);

            var u = new Url(url);

            foreach (var language in itm.Languages)
            {
                if(definition.LanguageCodesToInclude.Contains(language.Name))
                {
                    var alt = createAltUrl(itm, language);

                    u.AlternateUrls.Add(alt);
                }
            }

            ChangeFrequency cf;
            tryParseEnumField(itm, ChangeFrequencyFieldID, ChangeFrequency.Daily, out cf);
            u.ChangeFrequency = cf;

            float p;
            tryParseFloatField(itm, PriorityFieldID, 0.5f, out p);
            u.Priority = p;

            DateTime dt = DateTime.MinValue;
            tryParseDateTimeField(itm, FieldIDs.Updated, DateTime.MinValue, out dt);
            u.LastModified = dt;

            return u;
        }

        private AlternateUrl createAltUrl(Item itm, Language language)
        {
            var alt = new AlternateUrl();

            UrlOptions uo = UrlOptions.DefaultOptions;
            uo.Language = language;

            alt.Url = LinkManager.GetItemUrl(itm, uo);
            alt.Language = language.Name;

            return alt;
        }

        private bool tryParseDateTimeField(Item itm, ID fieldID, DateTime defaultValue, out DateTime dt)
        {
            if (itm.Fields.Contains(fieldID))
            {
                string value = itm.Fields[fieldID].Value;
                return DateTime.TryParse(value, out dt);
            }

            dt = defaultValue;
            return false;
        }

        private bool tryParseFloatField(Item itm, ID fieldID, float defaultValue, out float v)
        {
            if (itm.Fields.Contains(fieldID))
            {
                string value = itm.Fields[fieldID].Value;
                return float.TryParse(value, out v);
            }

            v = defaultValue;
            return false;
        }

        private bool tryParseEnumField<T>(Item itm, ID fieldID, T defaultValue, out T cf) where T : struct
        {
            if (itm.Fields.Contains(fieldID))
            {
                string value = itm.Fields[fieldID].Value;
                return Enum.TryParse(value, true, out cf);
            }

            cf = defaultValue;

            return false;
        }
    }

}