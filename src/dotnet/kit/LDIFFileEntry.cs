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
                if (fieldname == "certificateRevocationList") { return true; }
                if (fieldname == "userCertificate") { return true; }
                var values = fieldname.Split(new []{ ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Contains("binary")) {
                    return true;
                    }
                }
            return false;
            }}

        public String FullName { get {
            var values = new List<String>();
            values.AddIfNotNull(DomainComponent);
            values.AddIfNotNull(Organization);
            values.AddIfNotNull(Country?.ToLower());
            values.AddIfNotNull(FileName);
            return String.Join("\\", values);
            }}

        private static String UpdateFileName(String value) {
            if (value == null) { return null; }
            return value.
                Replace('`', '\'').
                Replace('"','\'');
            }

        public String FileName { get {
            var r = new StringBuilder(CommonName);
                 if (properties.ContainsKey("userCertificate;binary"))           { r.Append(".cer"); }
            else if (properties.ContainsKey("certificateRevocationList;binary")) { r.Append(".crl"); }
            else if (properties.ContainsKey("CscaMasterListData"))               { r.Append(".ml");  }
            else if (properties.ContainsKey("userCertificate"))                  { r.Append(".cer"); }
            else if (properties.ContainsKey("certificateRevocationList"))        { r.Append(".crl"); }
            return UpdateFileName(r.ToString());
            }}

        public Byte[] ReadAllBytes()
            {
            Object r;
            if (TryGetFieldValue("userCertificate;binary", out r))           { return (Byte[])r; }
            if (TryGetFieldValue("certificateRevocationList;binary", out r)) { return (Byte[])r; }
            if (TryGetFieldValue("CscaMasterListData", out r))               { return (Byte[])r; }
            if (TryGetFieldValue("userCertificate", out r))                  { return (Byte[])r; }
            if (TryGetFieldValue("certificateRevocationList", out r))        { return (Byte[])r; }
            throw new NotSupportedException();
            }

        public Stream OpenRead()
            {
            return new MemoryStream(ReadAllBytes());
            }

        void IFileService.MoveTo(String target)
            {
            ((IFileService)this).MoveTo(target, false);
            }

        /// <summary>Move an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void IFileService.MoveTo(String target, Boolean overwrite)
            {
            ((IFileService)this).CopyTo(target, overwrite);
            }

        /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void IFileService.CopyTo(String target, Boolean overwrite)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var sourcestream = OpenRead()) {
                if (File.Exists(target)) {
                    if (!overwrite) { throw new IOException(); }
                    File.Delete(target);
                    }
                var folder = Path.GetDirectoryName(target);
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                using (var targetstream = File.OpenWrite(target)) {
                    sourcestream.CopyTo(targetstream);
                    }
                }
            }

        private readonly IDictionary<String, IList<Object>> properties = new ConcurrentDictionary<String, IList<Object>>();
        private readonly IDictionary<String, IList<String>> identifier = new Dictionary<String, IList<String>>();
        }
    }