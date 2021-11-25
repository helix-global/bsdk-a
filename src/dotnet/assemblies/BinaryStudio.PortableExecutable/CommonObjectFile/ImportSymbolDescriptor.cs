using System;

namespace BinaryStudio.PortableExecutable
    {
    public class ImportSymbolDescriptor : IEquatable<ImportSymbolDescriptor>,IComparable<ImportSymbolDescriptor>, IComparable
    {
        public String Name { get; }
        public Int32 Hint { get; }
        public Int32 Ordinal { get; }

        internal ImportSymbolDescriptor(String name, Int32 hint) {
            Name = name;
            Hint = hint;
            Ordinal = -1;
            }

        internal ImportSymbolDescriptor(Int32 ordinalnumber) {
            Ordinal = ordinalnumber;
            Hint = -1;
            }

        public override String ToString() {
            return (Ordinal > 0)
                ? String.Format("{{{0}}}", Ordinal.ToString("X4"))
                : String.Format("{{{0}}}:{1}", Hint.ToString("X4"), Name);
            }

        public override Int32 GetHashCode() {
            return HashCodeCombiner.GetHashCode(Name, Hint, Ordinal);
            }

        public override Boolean Equals(Object other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return Equals(other as ImportSymbolDescriptor);
            }

        public Boolean Equals(ImportSymbolDescriptor other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return String.Equals(Name, other.Name)
                && (Hint == other.Hint)
                && (Ordinal == other.Ordinal);
            }

        public Int32 CompareTo(ImportSymbolDescriptor other) {
            if (ReferenceEquals(this, other)) { return 0; }
            if (ReferenceEquals(null, other)) { return 1; }
            var r = String.Compare(Name, other.Name, StringComparison.Ordinal);
            if (r == 0) {
                r = Ordinal.CompareTo(other.Ordinal);
                return (r == 0)
                    ? Hint.CompareTo(other.Hint)
                    : r;
                }
            return r;
            }

        public Int32 CompareTo(Object other) {
            if (ReferenceEquals(null, other)) { return 1; }
            if (ReferenceEquals(this, other)) { return 0; }
            if (!(other is ImportSymbolDescriptor)) throw new ArgumentException($"Object must be of type {nameof(ImportSymbolDescriptor)}");
            return CompareTo((ImportSymbolDescriptor)other);
            }
        }
    }