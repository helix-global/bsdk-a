using System.Globalization;
using System.Numerics;
using BinaryStudio.Numeric;
using NUnit.Framework;

namespace UnitTests.BinaryStudio.Numeric
    {
    public class UnitTestUInt128
        {
        [SetUp]
        public void Setup()
            {
            }

        [Test]
        public void Shl()
            {
            Assert.AreEqual(new UInt128(0x00000200,0x200), (new UInt128(1,1)) <<  9);
            Assert.AreEqual(new UInt128(0x00000200,0x000), (new UInt128(1,1)) << 73);
            Assert.AreEqual(new UInt128(2,0), ((UInt128)1) << 65);
            Assert.AreEqual((UInt128)2, ((UInt128)1) <<  1);
            Assert.AreEqual(new UInt128(1,0), ((UInt128)1) << 64);
            }

        [Test]
        public void Div()
            {
            Assert.AreEqual(new UInt128(0x00000200,0), (new UInt128(0xeb1200,0)) /  0x7589);
            }

        [Test]
        public void Sub()
            {
            var x64_1 = 0x0200UL;
            var x64_2 = 0x0300U;
            var x64_3 = 0x0200000000000000UL;
            var x64_4 = 0x0300000000000000UL;
            Assert.AreEqual(0xffffffffffffff00, x64_1-x64_2);
            Assert.AreEqual(0xff00000000000000, x64_3-x64_4);
            Assert.AreEqual(0x01fffffffffffd00, x64_3-x64_2);
            Assert.AreEqual(new UInt128(0xffffffffffffffff,0xffffffffffffff00), (UInt128)x64_1-x64_2);
            }

        [Test]
        public void Mod()
            {
            var x64_1 = 0x0200UL;
            var x64_2 = 0x0300U;
            var x64_3 = 0x0200000000000000UL;
            var x64_4 = 0x0300000000000000UL;
            var x64_5 = 0x0300000000000010UL;
            var x64_6 = 0x0300000000001110UL;
            Assert.AreEqual(0x210U, x64_6%x64_2);
            Assert.AreEqual(0x010U, x64_5%x64_2);
            Assert.AreEqual(0x000U, x64_4%x64_2);
            Assert.AreEqual(0x200U, x64_3%x64_2);
            Assert.AreEqual(0x200U, x64_1%x64_2);
            Assert.AreEqual(0x210U, ((UInt128)(x64_6))%x64_2);
            Assert.AreEqual(0x010U, ((UInt128)(x64_5))%x64_2);
            Assert.AreEqual(0x000U, ((UInt128)(x64_4))%x64_2);
            Assert.AreEqual(0x200U, ((UInt128)(x64_3))%x64_2);
            Assert.AreEqual(0x200U, ((UInt128)(x64_1))%x64_2);
            }

        [Test]
        public void Str()
            {
            var x64_1 = 0x0200UL;
            var x64_2 = 0x0300U;
            var x64_3 = 0x0200000000000000UL;
            var x64_4 = 0x0300000000000000UL;
            var x64_5 = 0x0300000000000010UL;
            var x64_6 = 0x0300000000001110UL;
            Assert.AreEqual("99aabbccddeeff881122334455667788", new UInt128(0x99AABBCCDDEEFF88UL,0x1122334455667788UL).ToString("x"));
            var xb_1 = BigInteger.Parse("0099aabbccddeeff881122334455667788", NumberStyles.HexNumber);
            var xb_2 = BigInteger.Parse("000f5ddf947c97e65a681d05206ef0a58d", NumberStyles.HexNumber);
            Assert.AreEqual("204258382862869203252457285150038194056", xb_1.ToString());
            Assert.AreEqual("20425838286286920325245728515003819405",  xb_2.ToString());
            Assert.AreEqual("20425838286286920325245728515003819405",  new UInt128(0x0f5ddf947c97e65aUL,0x681d05206ef0a58dUL).ToString("r"));
            Assert.AreEqual("204258382862869203252457285150038194056", new UInt128(0x99AABBCCDDEEFF88UL,0x1122334455667788UL).ToString("r"));
            Assert.AreEqual("144115188075855872", ((UInt128)x64_3).ToString("r"));
            Assert.AreEqual("512", x64_1.ToString());
            Assert.AreEqual("768", x64_2.ToString());
            Assert.AreEqual("144115188075855872", x64_3.ToString());
            Assert.AreEqual("216172782113783808", x64_4.ToString());
            Assert.AreEqual("216172782113783824", x64_5.ToString());
            Assert.AreEqual("216172782113788176", x64_6.ToString());
            Assert.AreEqual("512", ((UInt128)x64_1).ToString("r"));
            Assert.AreEqual("768", ((UInt128)x64_2).ToString("r"));
            Assert.AreEqual("216172782113783808", ((UInt128)x64_4).ToString("r"));
            Assert.AreEqual("216172782113783824", ((UInt128)x64_5).ToString("r"));
            Assert.AreEqual("216172782113788176", ((UInt128)x64_6).ToString("r"));
            Assert.AreEqual("0f5ddf947c97e65a681d05206ef0a58d", new UInt128(0x0f5ddf947c97e65aUL,0x681d05206ef0a58dUL).ToString("x"));
            }
        }
    }