using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SitemapGenerator.Core.Models
{

    public class FileDefinition
    {
        public static readonly ID SitemapFileDefinitionID = new ID("{99C99FA5-9B90-46F3-9A7B-DB52808B1EA4}");

        public static FileDefinition Empty { get { return new FileDefinition(); } }

        public string FilenameToGenerate { get; set; }

        public string SourceDatabase { get; set; }

        public Guid RootItem { get; set; }

        public List<Guid> LanguagesToInclude { get; private set; }
        public List<string> LanguageCodesToInclude { get; private set; }

        public List<Guid> TemplatesToInclude { get; private set; }

        public bool LanguagesResolved { get; private set; }

        private FileDefinition()
        {
            FilenameToGenerate = string.Empty;
            SourceDatabase = string.Empty;
            RootItem = Guid.Empty;
            LanguagesToInclude = new List<Guid>();
            TemplatesToInclude = new List<Guid>();
            LanguageCodesToInclude = new List<string>();
        }

        public FileDefinition(Item configItem)
        {
            if(configItem == null)
            {
                throw new ArgumentNullException("configItem");
            }
            if(configItem.TemplateID != SitemapFileDefinitionID)
            {
                throw new ArgumentException("The Sitecore Item passed is not based on the correct Template", "configItem");
            }

            FilenameToGenerate = configItem.Fields[nameof(FilenameToGenerate)].Value;
            SourceDatabase = configItem.Fields[nameof(SourceDatabase)].Value;

            var rootItm = configItem.Fields[nameof(RootItem)].Value;
            if(!string.IsNullOrWhiteSpace(rootItm))
            {
                RootItem = Guid.Parse(rootItm);
            }

            var languages = configItem.Fields[nameof(LanguagesToInclude)].Value;
            if (!string.IsNullOrWhiteSpace(languages))
            {
                LanguagesToInclude = languages
                    .Split('|')
                    .Select(l => Guid.Parse(l))
                    .ToList();
            }
            else
            {
                LanguagesToInclude = new List<Guid>();
            }
            
            var templates = configItem.Fields[nameof(TemplatesToInclude)].Value;
            if(!string.IsNullOrWhiteSpace(templates))
            {
                TemplatesToInclude = templates
                    .Split('|')
                    .Select(t => Guid.Parse(t))
                    .ToList();
            }
            else
            {
                TemplatesToInclude = new List<Guid>();
            }

            LanguageCodesToInclude = new List<string>();
        }

        public void ResolveLanguages(Database db)
        {
            foreach(var id in LanguagesToInclude)
            {
                Item itm = db.GetItem(new ID(id));
                string code = itm.Fields["Iso"].Value;
                LanguageCodesToInclude.Add(code);
            }

            LanguageCodesToInclude = LanguagesToInclude
                .Select(id => db.GetItem(new ID(id)))
                .Select(itm => itm.Fields["Iso"].Value)
                .ToList();

            LanguagesResolved = true;
        }
    }

}