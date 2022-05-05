using System;
using System.Collections.Generic;
using System.Text;
using BinaryStudio.DirectoryServices;

namespace BinaryStudio.Security.Cryptography.DataInterchangeFormat
    {
    internal class LDIFFile : IDirectoryService
        {
        public Version Version { get;private set; }
        public IList<LDIFFileEntry> Entries { get; }

        public LDIFFile(IFileService service) {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }
            Entries = Load(LDIFReader.Read(service.OpenRead()));
            }

        private String ToString(Object source) {
            var r = source.ToString();
            if (source is Byte[]) { r = Encoding.UTF8.GetString((Byte[])source); }
            //r = r.Replace("\\=", "=");
            //r = r.Replace("\\,", ",");
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

        public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption) {
            foreach (var entry in Entries) {
                if (entry.IsFile) {
                    if (PathUtils.IsMatch(searchpattern, entry.FileName)) {
                        yield return entry;
                        }
                    }
                }
            }
        }
    }