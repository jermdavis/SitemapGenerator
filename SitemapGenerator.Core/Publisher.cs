using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitemapGenerator.Core
{

    public class Publisher
    {
        public static string ConfigDatabaseSetting = "SitemapGenerator.ConfigDatabase";

        private IDataWriter _writer;

        public IDataWriter Writer { get { return _writer; } }

        public string ConfigDatabase { get; private set; }

        public Publisher()
        {
            _writer = new DiskDataWriter();

            fetchConfigDatabase();
        }

        public Publisher(IDataWriter writer)
        {
            if(writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            _writer = writer;

            fetchConfigDatabase();
        }

        private void fetchConfigDatabase()
        {
            ConfigDatabase = Sitecore.Configuration.Settings.GetSetting(ConfigDatabaseSetting);
            if (string.IsNullOrWhiteSpace(ConfigDatabase))
            {
                ConfigDatabase = "master";
            }
        }

        public void Publish(object sender, EventArgs args)
        {
            var db = Sitecore.Configuration.Factory.GetDatabase("master");

            var config = new Configuration();
            var sitemaps = config.Fetch(db);

            var gen = new DataGenerator();
            var us = new UrlSetSerialiser();

            foreach(var cfg in sitemaps)
            {
                cfg.ResolveLanguages(db);

                var urlSet = gen.Generate(cfg);
                var xDoc = us.Serialise(urlSet);

                _writer.Write(cfg.FilenameToGenerate, xDoc);
            }
        }
    }

}