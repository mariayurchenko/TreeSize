using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeSize;

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for TreeSizeTests
    /// </summary>
    [TestClass]
    public class TreeSizeTests
    {
        [TestMethod]
        public void TestMethod()
        {
            FillFile test = new FillFile(AppDomain.CurrentDomain.BaseDirectory);
            test.GetFile();
            File file = test.file;
            Assert.AreEqual("1,3MB", file.Size);
            Assert.AreEqual("1,3MB", file.Allocated);
            Assert.AreEqual(19, file.Files);
            Assert.AreEqual(0, file.Folders);
            Assert.AreEqual(100, file.Percent);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithAnEmptyStringThrowsArgumentNullException()
        {
            FillFile test = new FillFile("");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WithNonExistentFileThrowsArgumenException()
        {
            FillFile test = new FillFile("dxfcghj");
        }

    }
}
