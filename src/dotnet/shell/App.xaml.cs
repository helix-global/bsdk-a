using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
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
        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
            {
            var x1 = 100000000000L;
            var x2 = -x1;
            var x3 = -100000000000L;
            var x4 = -x3;
            var y1 = new LongInteger(x1);
            var y2 = -y1;
            var y3 = new LongInteger(x3);
            var y4 = -y3;
            var z1 = y1.ToString("x");
            var z2 = y2.ToString("x");
            var z3 = y3.ToString("x");
            var z4 = y4.ToString("x");
            var x5 = x3 + 1;
            var y5 = y3 + 1;
            var z5 = y5.ToString("x");
            var x6 = x1 - 1;
            var y6 = y1 - 1;
            var z6 = y6.ToString("x");
            var x7 = x6/256;
            var y7 = y6/256;
            var z7 = y7.ToString("x");
            //var li1 = (new LongInteger(+123456789)).ToString();
            //var li2 = new LongInteger(SByte.MinValue);
            //var o = Convert.ToBase64String(bytes);
            base.OnStartup(e);
            }
        }
    }
