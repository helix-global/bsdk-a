using System;
using BinaryStudio.PortableExecutable.Win32;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    public class Relocation : IJsonSerializable
        {
        public Int64 Offset { get; }
        public Int32 SymbolIndex { get; }
        public Int16 Type { get; }
        public IMAGE_FILE_MACHINE Machine { get; }
        public String SymbolName { get; }

        internal unsafe Relocation(CommonObjectFileSource o, IMAGE_FILE_MACHINE machine, IMAGE_RELOCATION* source) {
            Offset = source->VirtualAddress;
            SymbolIndex = source->SymbolTableIndex;
            Type = source->Type;
            Machine = machine;
            if (o.SymbolTable.Count > 0)
                {
                SymbolName = ((Symbol)o.SymbolTable[SymbolIndex]).Name;
                }
            }

        private static String ToString(IMAGE_FILE_MACHINE machine, Int16 type)
            {
            switch (machine)
                {
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_UNKNOWN:   { return type.ToString();                    }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_AM33:      { return ((IMAGE_REL_AM)type).ToString();    }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_AMD64:     { return ((IMAGE_REL_AMD64)type).ToString(); }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARM:       
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARMNT:     { return ((IMAGE_REL_ARM)type).ToString();   }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARM64:     { return ((IMAGE_REL_ARM64)type).ToString(); }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_EBC:       { return ((IMAGE_REL_EBC)type).ToString();   }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_I386:      { return ((IMAGE_REL_I386)type).ToString();  }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_IA64:      { return ((IMAGE_REL_IA64)type).ToString();  }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_M32R:      { return ((IMAGE_REL_M32R)type).ToString();  }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_MIPS16:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_MIPSFPU:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_MIPSFPU16:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_WCEMIPSV2: { return ((IMAGE_REL_MIPS)type).ToString();  }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_POWERPC:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_POWERPCFP: { return ((IMAGE_REL_PPC)type).ToString();   }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_SH3:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_SH3DSP:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_SH4:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_SH5:       { return ((IMAGE_REL_SH3)type).ToString();   }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ALPHA:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ALPHA64:   { return ((IMAGE_REL_ALPHA)type).ToString(); }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_CEE:       { return ((IMAGE_REL_CEE)type).ToString();   }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_CEF:       { return ((IMAGE_REL_CEF)type).ToString();   }
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_THUMB:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_TRICORE:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_R3000:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_R10000:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_R4000:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_RISCV32:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_RISCV64:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_RISCV128:
                    { return ((IMAGE_REL_BASED)type).ToString();   }
                default: { throw new ArgumentOutOfRangeException(); }
                }
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return ToString(Machine, Type);
            }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartObject();
                writer.WriteValue(serializer, nameof(Offset), Offset);
                writer.WriteValue(serializer, nameof(Type), Type);
                writer.WriteValue(serializer, nameof(SymbolIndex), SymbolIndex);
                writer.WriteValue(serializer, nameof(SymbolName), SymbolName);
            writer.WriteEndObject();
            }
        }
    }