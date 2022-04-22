using System;
using System.Globalization;
using BinaryStudio.PlatformComponents.Win32;

namespace HResultSample
    {
    class Program
        {
        static void Main(string[] args)
            {
            CultureInfo culture = null;
            foreach (var value in Enum.GetValues(typeof(HRESULT)))
                {
                Console.WriteLine($"{value}:{HResultException.GetExceptionForHR((Int32)value,culture).Message}");
                }
            }
        }
    }
