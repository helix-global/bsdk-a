using System.Text;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using NUnit.Framework;

namespace UnitTests.BinaryStudio.AbstractSyntaxNotation
    {
    public class UnitTestTime
        {
        [SetUp]
        public void Setup()
            {
            }

        [Test]
        public void UtcTime()
            {
            Assert.AreEqual("2011-11-09T09:30:13-08:00", new Asn1UtcTime("111109093013-0800").ToString());
            Assert.AreEqual("2011-11-09T09:30:13+08:00", new Asn1UtcTime("111109093013+0800").ToString());
            Assert.AreEqual("2011-11-09T09:30:13Z",      new Asn1UtcTime("111109093013Z").ToString());
            Assert.AreEqual("2011-11-09T09:30:13-08:00", new Asn1GeneralTime("20111109093013-0800").ToString());
            Assert.AreEqual("2011-11-09T09:30:13+08:00", new Asn1GeneralTime("20111109093013+0800").ToString());
            Assert.AreEqual("2011-11-09T09:30:13Z",      new Asn1GeneralTime("20111109093013Z").ToString());
            Assert.AreEqual("2011-11-09T09:30:13",       new Asn1GeneralTime("20111109093013").ToString());
            }
        }
    }