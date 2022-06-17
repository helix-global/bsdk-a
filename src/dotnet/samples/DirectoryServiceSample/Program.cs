using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BinaryStudio.DirectoryServices;

namespace DirectoryServiceSample
    {
    class Program
        {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern Int32 GetShortPathName(String lpszLongPath, StringBuilder lpszShortPath, Int32 cchBuffer);
        private static String GetShortPathName(String lpszLongPath)
            {
            var lpszShortPath = new StringBuilder(1024);
            GetShortPathName(lpszLongPath,lpszShortPath,1024);
            return lpszShortPath.ToString();
            }

        static void Main(string[] args)
            {
            var x = GetShortPathName(@"D:\Program Files (x86)\Borland\Delphi7");
            var filename = $"countries-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.csv";
            using (var stream = File.OpenWrite(filename))
            using (var target = new StreamWriter(stream, Encoding.UTF8)) {
                var descriptors = TypeDescriptor.GetProperties(typeof(IcaoCountry)).OfType<PropertyDescriptor>().ToArray();
                target.WriteLine($"{String.Join(";", descriptors.Select(i => i.DisplayName))}");
                foreach (var country in IcaoCountry.TwoLetterCountries.OrderBy(i=>i.Key))
                    {
                    target.WriteLine($"{String.Join(";", descriptors.Select(i => i.GetValue(country.Value) ?? "nullptr"))}");
                    }
                }
            }
        }
    }
