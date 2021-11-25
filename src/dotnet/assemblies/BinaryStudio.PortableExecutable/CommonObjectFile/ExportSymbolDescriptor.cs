using System;

namespace BinaryStudio.PortableExecutable
    {
    public class ExportSymbolDescriptor
        {
        public String Name { get; }
        public Int32 Ordinal { get; }
        public UInt32 EntryPoint { get;internal set; }
        public String EntryPointName { get;internal set; }

        internal ExportSymbolDescriptor(String name, Int32 ordinal) {
            Name = name;
            Ordinal = ordinal;
            }

        internal ExportSymbolDescriptor(Int32 ordinal, UInt32 entrypoint) {
            EntryPoint = entrypoint;
            Ordinal = ordinal;
            }

        internal ExportSymbolDescriptor(Int32 ordinal, String entrypoint) {
            EntryPointName = entrypoint;
            Ordinal = ordinal;
            }

        public override String ToString()
            {
            return String.Format("{{{2}}}:{{{0}}}:{1}", Ordinal.ToString("X4"), Name,
                String.IsNullOrEmpty(EntryPointName)
                    ? EntryPoint.ToString("X8")
                    : EntryPointName);
            }
        }

    }