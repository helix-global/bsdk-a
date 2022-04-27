using BinaryStudio.DirectoryServices;
using NUnit.Framework;

namespace UnitTests.BinaryStudio.DirectoryServices
    {
    public class UnitTestPathUtils
        {
        [SetUp]
        public void Setup()
            {
            }

        [Test]
        public void IsMatch()
            {
            Assert.IsTrue(PathUtils.IsMatch("*.dll", "filename.dll"));
            Assert.IsTrue(PathUtils.IsMatch("*.dll", "filename.othername.dll"));
            }
        }
    }