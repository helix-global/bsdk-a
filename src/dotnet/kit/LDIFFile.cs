using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryStudio.Security.Cryptography.DataInterchangeFormat
    {
    public class LDIFFile
        {
        public Version Version { get;private set; }
        public IList<LDIFFileEntry> Entries { get; }

        public LDIFFile(String filename) {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            if (String.IsNullOrWhiteSpace(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            if (!File.Exists(filename)) { throw new ArgumentOutOfRangeException(filename); }
            Entries = Load(LDIFReader.Read(filename));
            }

        private String ToString(Object source) {
            var r = source.ToString();
            if (source is Byte[]) { r = Encoding.UTF8.GetString((Byte[])source); }
            r = r.Replace("\\=", "=");
            r = r.Replace("\\,", ",");
            return r;
            }

        private IList<LDIFFileEntry> Load(IEnumerable<LDIFEntry> entries) {
            var r = new List<LDIFFileEntry>();
            LDIFFileEntry dn = null;
            foreach (var e in entries) {
                     if (e.Name == "version") { Version = new Version(Int32.Parse(e.Value.ToString()), 0); }
                else if (e.Name == "dn") {
                    if (dn != null) { r.Add(dn); }
                    dn = new LDIFFileEntry(ToString(e.Value), e.LineNumber);
                    }
                else
                    {
                    dn.Add(e.Name, e.Value);
                    }
                }
            if (dn != null) {
                r.Add(dn);
                }
            return r;
            }
        }
    }