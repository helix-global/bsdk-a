using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using BinaryStudio.Numeric;

namespace shell
    {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
        {
        [StructLayout(LayoutKind.Explicit,Pack=1)]
        struct U64
            {
            [FieldOffset(0)] public UInt64 u64;
            [FieldOffset(0)] public unsafe fixed UInt32 u32[2];
            [FieldOffset(0)] public unsafe fixed UInt16 u16[4];
            [FieldOffset(0)] public unsafe fixed Byte u8[8];
            };

        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override unsafe void OnStartup(StartupEventArgs e)
            {
            var x64_1  = 0x99AABBCCDDEEFF88UL;
            var r64_1  = (UInt64*)&x64_1;
            var x128_1 = new UInt128(x64_1,0x1122334455667788UL);
            var r128_1 = (UInt64*)&x128_1;
            var s_128_1 = x128_1.ToString();
            var u_1 = new U64();
            u_1.u64 = x64_1;
            var x32_0 = u_1.u32[0];
            var x32_1 = u_1.u32[1];
            var x16_0 = u_1.u16[0];
            var x16_1 = u_1.u16[1];
            var x16_2 = u_1.u16[2];
            var x16_3 = u_1.u16[3];
            var x16_4 = UInt16.MaxValue;
            var x32_5 = x32_0 + UInt32.MaxValue;
            var x32_6 = (UInt64)x32_0 + UInt32.MaxValue;
            var x128_2 = (UInt128)x32_6;
            var x128_3 = (UInt128)1;
            var x128_4 = x128_3 << 65;
            var x256_1 = (UInt256)1;
            var x512_1 = (UInt512)1;
            var x192_1 = (UInt192)1;
            var x224_1 = (UInt224)1;
            var x384_1 = (UInt384)1;
            var x64_2  = 0UL;
            var x64_3  = x64_2 - 1UL;
            //var x1 = 100000000000L;
            //var x2 = -x1;
            //var x3 = -100000000000L;
            //var x4 = -x3;
            //var y1 = new LongInteger(x1);
            //var y2 = -y1;
            //var y3 = new LongInteger(x3);
            //var y4 = -y3;
            //var z1 = y1.ToString("x");
            //var z2 = y2.ToString("x");
            //var z3 = y3.ToString("x");
            //var z4 = y4.ToString("x");
            //var x5 = x3 + 1;
            //var y5 = y3 + 1;
            //var z5 = y5.ToString("x");
            //var x6 = x1 - 1;
            //var y6 = y1 - 1;
            //var z6 = y6.ToString("x");
            //var x7 = x6/256;
            //var y7 = y6/256;
            //var z7 = y7.ToString("x");
            //var li1 = (new LongInteger(+123456789)).ToString();
            //var li2 = new LongInteger(SByte.MinValue);
            //var o = Convert.ToBase64String(bytes);
            base.OnStartup(e);
            }
        }
    }
