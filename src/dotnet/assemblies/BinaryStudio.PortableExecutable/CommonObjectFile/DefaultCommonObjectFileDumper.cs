using System;
using System.IO;
using System.Linq;
using System.Text;
using BinaryStudio.IO;
using BinaryStudio.PortableExecutable.CommonObjectFile.Sections;
using BinaryStudio.PortableExecutable.Win32;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable
    {
    internal class DefaultCommonObjectFileDumper : IFileDumper
        {
        private const Int16 IMAGE_SYM_UNDEFINED =  0;
        private const Int16 IMAGE_SYM_ABSOLUTE  = -1;
        private const Int16 IMAGE_SYM_DEBUG     = -2;

        public FileDumperFlags Flags { get; }
        public DefaultCommonObjectFileDumper(FileDumperFlags flags)
            {
            Flags = flags;
            }

        public void Write(TextWriter writer, Object o) {
            if (o is CommonObjectFileSource i) {
                Write(writer, i);
                return;
                }
            throw new NotSupportedException();
            }

        #region M:Write(TextWriter,CommonObjectFileSource)
        private void Write(TextWriter writer, CommonObjectFileSource o)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteLine("File Type: COFF OBJECT\n");
            writer.WriteLine("FILE HEADER VALUES");
            writer.WriteLine("    {0,8:X4} machine ({1})", (UInt16)o.Machine, o.Machine);
            writer.WriteLine("    {0,8:X} number of sections", o.Sections.Count);
            writer.WriteLine("    {0,8} time date stamp", o.TimeDateStamp);
            writer.WriteLine("    {0,8:X} number of symbols", o.NumberOfSymbols);
            writer.WriteLine("    {0,8:X} size of optional header", o.SizeOfOptionalHeader);
            writer.WriteLine("    {0,8:X} characteristics ({1})", (UInt16)o.Characteristics,o.Characteristics);
            var i = 1;
            foreach (var section in o.Sections) {
                writer.WriteLine("\nSECTION HEADER #{0:X}", i);
                Write(writer, section);
                i++;
                }
            if (o.SymbolTable.Any())
                {
                writer.WriteLine("\nCOFF SYMBOL TABLE");
                i = 0;
                foreach (var symbol in o.SymbolTable) {
                    writer.Write($"{i:X2}  ");
                    Write(writer, symbol);
                    i++;
                    }
                }
            }
        #endregion
        #region M:Write(TextWriter,Section)
        private void Write(TextWriter writer, CommonObjectFileSection o)
            {
            writer.WriteLine("    {0,8} name", o.Name); 
            writer.WriteLine("    {0,8:X} virtual address", o.VirtualAddress);
            writer.WriteLine("    {0,8:X} size of raw data", o.Content.Length);
            writer.WriteLine("    {0,8:X} file pointer to relocation table", o.PointerToRelocations);
            writer.WriteLine("    {0,8:X} file pointer to line numbers", o.PointerToLineNumbers);
            writer.WriteLine("    {0,8:X} number of relocations", o.NumberOfRelocations);
            writer.WriteLine("    {0,8:X} number of line numbers", o.NumberOfLineNumbers);
            writer.WriteLine("    {0,8:X} flags ({1})", (UInt16)o.Characteristics, o.Characteristics);
            if (o.Content.Length > 0) {
                writer.WriteLine("\nRAW DATA");
                foreach (var i in Base32Formatter.Format(o.Content, o.Content.Length, 0, Base32FormattingFlags.Offset)) {
                    writer.WriteLine("  {0}", i);
                    }
                }
            if (o is DirectiveSection drectve) {
                writer.Write("\n   Linker Directives");
                writer.Write("\n   -----------------\n");
                writer.WriteLine($"   {drectve.LinkerDirectives}");
                }
            if (o.Relocations.Count > 0) {
                writer.WriteLine("\n                                        Symbol    Symbol");
                writer.WriteLine(" Offset              Type               Index     Name");
                writer.WriteLine("-------- ------------------------------ -------- ------------------");
                foreach (var relocation in o.Relocations) {
                    Write(writer, relocation);
                    }
                }
            }
        #endregion
        #region M:Write(TextWriter,Relocation)
        private void Write(TextWriter writer, Relocation o)
            {
            writer.WriteLine("{0:X8} {1,-30} {2,8:X2} {3}",
                o.Offset,
                ToString(o.Machine, o.Type),
                o.SymbolIndex,
                o.SymbolName);
            }
        #endregion
        #region M:Write(TextWriter,ISymbol)
        private void Write(TextWriter writer, ISymbol o)
            {
            if (o is Symbol i) {
                var r = new StringBuilder();
                r.AppendFormat("{0:X8} ", i.Value);
                r.AppendFormat("{0,-32} ", i.StorageClass);
                switch (i.SectionNumber)
                    {
                    case IMAGE_SYM_UNDEFINED: { r.Append("IMAGE_SYM_UNDEFINED"); } break;
                    case IMAGE_SYM_ABSOLUTE:  { r.Append("IMAGE_SYM_ABSOLUTE "); } break;
                    case IMAGE_SYM_DEBUG:     { r.Append("IMAGE_SYM_DEBUG    "); } break;
                    default: { r.AppendFormat("{0,-19:X4}", i.SectionNumber); } break;
                    }
                r.Append(' ');
                r.AppendFormat("{0}", i.Name);
                writer.WriteLine(r.ToString());
                }
            else if (o is CommonLanguageInfrastructureTokenDefinition clr)
                {
                writer.WriteLine("{CLR Token Definition}:");
                writer.WriteLine("      Symbol table index:{0:X}", clr.SymbolTableIndex);
                }
            else if (o is SectionDefinitionSymbol section)
                {
                writer.WriteLine("{Section Definition}:");
                writer.WriteLine("      Length:{0:X}", section.Length);
                writer.WriteLine("      Number of relocations:{0:X}", section.NumberOfRelocations);
                writer.WriteLine("      Number of line numbers:{0:X}", section.NumberOfLineNumbers);
                writer.WriteLine("      Checksum:{0:X8}", section.CheckSum);
                writer.WriteLine("      Number:{0:X}", section.Number);
                writer.WriteLine("      Selection:{0:X}", section.Selection);
                }
            else
                {
                var r = new StringBuilder();
                writer.WriteLine("{none}");
                }
            }
        #endregion

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
        }
    }