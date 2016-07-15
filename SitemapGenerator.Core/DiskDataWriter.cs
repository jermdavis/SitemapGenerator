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
            string file;

            if (System.Web.Hosting.HostingEnvironment.IsHosted)
            {
                file = System.Web.Hosting.HostingEnvironment.MapPath("/" + fileName);
            }
            else
            {
                file = fileName;
            }

            xml.Save(file, SaveOptions.OmitDuplicateNamespaces);
        }
    }

}