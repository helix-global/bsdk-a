using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryStudio.PortableExecutable
    {
    public class ImportLibraryReference : IComparable<ImportLibraryReference>, IComparable
        {
        public String FileName { get; }
        public ImportSymbolDescriptor[] Symbols { get; }

        internal ImportLibraryReference(String filename, IEnumerable<ImportSymbolDescriptor> symbols) {
            FileName = filename.ToLower();
            Symbols = symbols.OrderBy(i => i).ToArray();
            }

        public override String ToString() {
            return FileName;
            }

        public Int32 CompareTo(ImportLibraryReference other) {
            if (ReferenceEquals(this, other)) { return 0; }
            if (ReferenceEquals(null, other)) { return 1; }
            return String.Compare(FileName, other.FileName, StringComparison.OrdinalIgnoreCase);
            }

        public Int32 CompareTo(Object other) {
            if (ReferenceEquals(null, other)) { return 1; }
            if (ReferenceEquals(this, other)) { return 0; }
            if (!(other is ImportLibraryReference)) throw new ArgumentException($"Object must be of type {nameof(ImportLibraryReference)}");
            return CompareTo((ImportLibraryReference)other);
            }
        }

    }