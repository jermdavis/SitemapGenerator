using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitemapGenerator.Core.Models
{

    // serialise to lowercase
    public enum ChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }

}