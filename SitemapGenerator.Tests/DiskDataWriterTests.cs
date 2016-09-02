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
        public readonly string AltTestFileName = "alt-test.xml";

        [TestCleanup]
        public void Cleanup()
        {
            if(File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
            if (File.Exists(AltTestFileName))
            {
                File.Delete(AltTestFileName);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiskDataWriter_Write_NullFilenameThrows()
        {
            var dw = new DiskDataWriter();
            var xml = new XDocument();
            dw.Write(null, xml);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiskDataWriter_Write_EmptyFilenameThrows()
        {
            var dw = new DiskDataWriter();
            var xml = new XDocument();
            dw.Write("", xml);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiskDataWriter_Write_EmptyXmlDocThrows()
        {
            var dw = new DiskDataWriter();
            dw.Write("test", null);
        }

        [TestMethod]
        public void DiskDataWriter_Write_GeneratesNonZeroLengthFile()
        {
            var xml = new XDocument();
            xml.Add(new XElement("Hello"));

            var sdw = new DiskDataWriter();
            sdw.Write(TestFileName, xml);

            Assert.IsTrue(File.Exists(TestFileName));

            var fi = new FileInfo(TestFileName);
            Assert.AreNotEqual(0, fi.Length);
        }

        [TestMethod]
        public void DiskDataWriter_Write_FilenameWithPathIgnoresPath()
        {
            var xml = new XDocument();
            xml.Add(new XElement("Hello"));

            var sdw = new DiskDataWriter();
            sdw.Write(@"..\fred\" + AltTestFileName, xml);

            Assert.IsTrue(File.Exists(AltTestFileName));

            var fi = new FileInfo(AltTestFileName);
            Assert.AreNotEqual(0, fi.Length);
        }
    }

}