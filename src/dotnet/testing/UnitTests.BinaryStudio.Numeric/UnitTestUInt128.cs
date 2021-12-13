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
        }
    }