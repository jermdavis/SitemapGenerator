using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SitemapGenerator.Core;
using System.IO;
using System.Xml.Linq;

namespace SitemapGenerator.Tests
{

    [TestClass]
    public class DiskDataWriterTests
    {
        public readonly string TestFileName = "test.xml";

        [TestCleanup]
        public void Cleanup()
        {
            if(File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
        }

        [TestMethod]
        public void Write_GeneratesNonZeroLengthFile()
        {
            var xml = new XDocument();
            xml.Add(new XElement("Hello"));

            var sdw = new DiskDataWriter();
            sdw.Write(TestFileName, xml);

            Assert.IsTrue(File.Exists(TestFileName));

            var fi = new FileInfo(TestFileName);
            Assert.AreNotEqual(0, fi.Length);
        }
    }

}