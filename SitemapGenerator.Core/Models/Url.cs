using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitemapGenerator.Core.Models
{

    public class Url
    {
        public string Location { get; private set; }
        public DateTime? LastModified { get; set; }
        public ChangeFrequency? ChangeFrequency { get; set; }
        public float? Priority { get; set; }

        public List<AlternateUrl> AlternateUrls { get; private set; }

        public Url(string location)
        {
            Location = location;
            AlternateUrls = new List<AlternateUrl>();
        }
    }

}