using Sitecore.Data;
using Sitecore.Data.Items;
using SitemapGenerator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitemapGenerator.Core
{

    public class Configuration
    {
        public static readonly ID SitemapDefinitionsFolderID = new ID("{6C72F916-731C-499A-B62D-9BA9ECCC5EB5}");

        public IEnumerable<FileDefinition> Fetch(Sitecore.Data.Database db)
        {
            if(db == null)
            {
                throw new ArgumentNullException("db");
            }

            List<FileDefinition> defs = new List<FileDefinition>();

            Item definitionFolder = db.GetItem(SitemapDefinitionsFolderID);

            foreach(Item definition in definitionFolder.Children)
            {
                FileDefinition sd = new FileDefinition(definition);
                defs.Add(sd);
            }

            return defs;
        }

    }

}