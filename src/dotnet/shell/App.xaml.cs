using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
            //var bytes = File.ReadAllBytes(@"C:\Users\Engineer\Documents\Untitled.png");
            //var o = Convert.ToBase64String(bytes);
            base.OnStartup(e);
            }
        }
    }
