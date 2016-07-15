using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SitemapGenerator.Core
{

    public interface IDataWriter
    {
        void Write(string fileName, XDocument xml);
    }

}