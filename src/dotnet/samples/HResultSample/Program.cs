using System;
using System.Globalization;
using System.IO;
using System.Text;
using BinaryStudio.PlatformComponents.Win32;

namespace HResultSample
    {
    class Program
        {
        static void Main(string[] args)
            {
            CultureInfo culture = null;
            using (var stream = File.OpenWrite("hresult.csv"))
            using (var target = new StreamWriter(stream, Encoding.Unicode)) {
                target.WriteLine("SCode;SCodeName;SCodeMessage");
                foreach (var value in Enum.GetValues(typeof(HRESULT)))
                    {
                    target.WriteLine($"{((Int32)((HRESULT)value)).ToString("x8")};{value};{HResultException.GetExceptionForHR((Int32)value,culture).Message}");
                    }
                }
            }
        }
    }
