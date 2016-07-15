using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SitemapGenerator.Core
{

    public class DiskDataWriter : IDataWriter
    {
        public void Write(string fileName, XDocument xml)
        {
            xml.Save(fileName, SaveOptions.OmitDuplicateNamespaces);
        }
    }

}