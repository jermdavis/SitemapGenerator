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
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if(xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            string fileWithoutPath = System.IO.Path.GetFileName(fileName);
            string diskFileName;

            if (System.Web.Hosting.HostingEnvironment.IsHosted)
            {
                diskFileName = System.Web.Hosting.HostingEnvironment.MapPath("/" + fileWithoutPath);
            }
            else
            {
                diskFileName = fileWithoutPath;
            }

            xml.Save(diskFileName, SaveOptions.OmitDuplicateNamespaces);
        }
    }

}