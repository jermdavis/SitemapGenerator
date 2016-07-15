using SitemapGenerator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SitemapGenerator.Tests.Mocks
{

    public class MockISitemapDataWriter : IDataWriter
    {
        public string FileName { get; set; }
        public XDocument Xml { get; set; }

        public int Calls { get; set; }

        public void Write(string fileName, XDocument xml)
        {
            FileName = fileName;
            Xml = xml;
            Calls += 1;
        }
    }

}
