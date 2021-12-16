using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.DataInterchangeFormat
    {
    public class LDIFFileEntry
        {
        public String Name { get; }
        public Int64 LineNumber { get; }
        private readonly IDictionary<String, IList<Object>> properties = new ConcurrentDictionary<String, IList<Object>>();
        public LDIFFileEntry(String name, Int64 lineno) {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            Name = name;
            LineNumber = lineno;
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

        public Boolean TryGetValue(String propertyname, Int32 index, out Object value) {
            value = null;
            IList<Object> r;
            if (properties.TryGetValue(propertyname, out r)) {
                if (index < r.Count) {
                    value = r[index];
                    return true;
                    }
                }
            return false;
            }
        }
    }