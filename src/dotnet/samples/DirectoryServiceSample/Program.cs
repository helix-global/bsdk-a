using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using BinaryStudio.DirectoryServices;

namespace DirectoryServiceSample
    {
    class Program
        {
        static void Main(string[] args)
            {
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
