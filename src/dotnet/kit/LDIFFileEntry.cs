using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryStudio.DataProcessing;
using BinaryStudio.DirectoryServices;

namespace BinaryStudio.Security.Cryptography.DataInterchangeFormat
    {
    internal class LDIFFileEntry : IFileService
        {
        public String Name { get; }
        public Int64 LineNumber { get; }
        public IDictionary<String, IList<String>> DistinguishedName { get { return identifier; }}

        public LDIFFileEntry(String key, Int64 lineno) {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            using (var reader = new StringReader(key)) {
                while (true) {
                    Int32 c;
                    var flags = true;
                    var name  = new StringBuilder();
                    var value = new StringBuilder();
                    while (((c = reader.Read()) != -1) && (c != '=')) { name.Append((char)c); }
                    if (c == -1) { break; }
                    while ((flags) && (c = reader.Read()) != -1) {
                        switch (c) {
                            case '\\':
                                {
                                c = reader.Read();
                                if (c != -1) {
                                    value.Append((char)c);
                                    }
                                }
                                break;
                            case ',':
                                {
                                UpdateIdentifier(name.ToString(), value.ToString());
                                name.Clear();
                                value.Clear();
                                flags = false;
                                }
                                break;
                            default:
                                {
                                value.Append((char)c);
                                }
                                break;
                            }
                        }
                    if (value.Length > 0)
                        {
                        UpdateIdentifier(name.ToString(), value.ToString());
                        }
                    }
                }
            Name = key;
            LineNumber = lineno;
            }

        private void UpdateIdentifier(String name, String value) {
            if (!identifier.TryGetValue(name, out var values)) { identifier.Add(name, values = new List<String>()); }
            values.Add(value);
            }

        internal void Add(String name, Object value) {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            IList<Object> r;
            if (!properties.TryGetValue(name, out r)) { properties.Add(name, r = new List<Object>()); }
            r.Add(value);
            }

        public override String ToString()
            {
            return Name;
            }

        public Boolean TryGetFieldValue(String field, Int32 index, out Object value) {
            value = null;
            IList<Object> r;
            if (properties.TryGetValue(field, out r)) {
                if (index < r.Count) {
                    value = r[index];
                    return true;
                    }
                }
            return false;
            }

        public Boolean TryGetFieldValue(String field, out Object value) {
            value = null;
            IList<Object> r;
            if (properties.TryGetValue(field, out r)) {
                if (r.Count == 0) { value = null; return true; }
                if (r.Count == 1) { value = r[0]; return true; }
                value = String.Join(",", r);
                return true;
                }
            return false;
            }

        public String GetNamePartValue(String partname) {
            IList<String> r;
            if (identifier.TryGetValue(partname, out r)) {
                if (r.Count == 0) { return null; }
                if (r.Count == 1) { return r[0]; }
                return String.Join(",", r);
                }
            return null;
            }

        public String DomainComponent { get {
            return TryGetFieldValue("dc", out var r)
                ? r.ToString()
                : GetNamePartValue("dc");
            }}

        public String OrganizationalUnit { get {
            return TryGetFieldValue("ou", out var r)
                ? r.ToString()
                : GetNamePartValue("ou");
            }}

        public String CommonName { get {
            return TryGetFieldValue("cn", out var r)
                ? r.ToString()
                : GetNamePartValue("cn");
            }}

        public String Country { get {
            return TryGetFieldValue("c", out var r)
                ? r.ToString()
                : GetNamePartValue("c");
            }}

        public String Organization { get {
            return TryGetFieldValue("o", out var r)
                ? r.ToString()
                : GetNamePartValue("o");
            }}

        public Boolean IsFile { get {
            foreach (var fieldname in properties.Keys) {
                if (fieldname == "CscaMasterListData") { return true; }
                var values = fieldname.Split(new []{ ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Contains("binary")) {
                    return true;
                    }
                }
            return false;
            }}

        public String FileName { get {
            var values = new List<String>();
            values.AddIfNotNull(DomainComponent);
            values.AddIfNotNull(Organization);
            values.AddIfNotNull(Country?.ToLower());
            values.AddIfNotNull(CommonName);
            var r = new StringBuilder(String.Join("\\", values));
                 if (properties.ContainsKey("userCertificate;binary"))           { r.Append(".cer"); }
            else if (properties.ContainsKey("certificateRevocationList;binary")) { r.Append(".crl"); }
            else if (properties.ContainsKey("CscaMasterListData"))               { r.Append(".ml");  }
            return r.ToString();
            }}

        public Byte[] ReadAllBytes()
            {
            Object r;
            if (TryGetFieldValue("userCertificate;binary", out r))           { return (Byte[])r; }
            if (TryGetFieldValue("certificateRevocationList;binary", out r)) { return (Byte[])r; }
            if (TryGetFieldValue("CscaMasterListData", out r))               { return (Byte[])r; }
            throw new NotSupportedException();
            }

        public Stream OpenRead()
            {
            return new MemoryStream(ReadAllBytes());
            }

        public void MoveTo(String target)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var sourcestream = OpenRead()) {
                var folder = Path.GetDirectoryName(target);
                var filename = Path.GetTempFileName();
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                if (File.Exists(filename)) { File.Delete(filename); }
                var block = new Byte[1024];
                using (var output = File.OpenWrite(target)) {
                    for (;;) {
                        var blockcount = block.Length;
                        var sourcecount = sourcestream.Read(block, 0, blockcount);
                        if (sourcecount == 0) { break; }
                        output.Write(block, 0, sourcecount);
                        }
                    }
                }
            }

        private readonly IDictionary<String, IList<Object>> properties = new ConcurrentDictionary<String, IList<Object>>();
        private readonly IDictionary<String, IList<String>> identifier = new Dictionary<String, IList<String>>();
        }
    }