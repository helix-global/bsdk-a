using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryStudio.PortableExecutable.Win32;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public abstract class CodeViewSection : CommonObjectFileSection
        {
        public abstract CV_SIGNATURE Signature { get; }
        public virtual Encoding Encoding { get { return Encoding.ASCII; }}
        public virtual Boolean IsLengthPrefixedString { get { return true; }}
        public IList<CodeViewPrimarySSection> Sections { get; }

        internal unsafe CodeViewSection(CommonObjectFileSource o, Int32 index, Byte* mapping, IMAGE_SECTION_HEADER* section)
            : base(o, index, mapping, section)
            {
            Sections = new List<CodeViewPrimarySSection>();
            var r = mapping + section->PointerToRawData;
            var sz = (Int32)section->SizeOfRawData;
            r += sizeof(Int32);
            for (;(*(Int32*)r > 0) && (sz > 4);) {
                var record = (DEBUG_SSECTION_HEADER*)r;
                var count = (Int32)Math.Ceiling(record->Length/4.0)*4;
                var content = (Byte*)(record + 1);
                Sections.Add(CodeViewPrimarySSection.From(
                    this,
                    (Int32)(content - (mapping + section->PointerToRawData)),
                    record->Type,
                    record->Length,
                    content
                    ));
                r  += sizeof(DEBUG_SSECTION_HEADER) + count;
                sz -= sizeof(DEBUG_SSECTION_HEADER) + count;
                }
            #if DEBUG
            RawDataUtilization = ((Double)((Int32)section->SizeOfRawData - sz))/section->SizeOfRawData;
            #endif
            Sections = new List<CodeViewPrimarySSection>(Sections);
            var H = Sections.OfType<CodeViewFileHashTableSSection>().FirstOrDefault();
            if (H != null) {
                var S = Sections.OfType<CodeViewStringTableSSection>().FirstOrDefault();
                if (S != null) {
                    foreach (var hashvalue in H.Values) {
                        hashvalue.FileName = S.Values[hashvalue.FileNameOffset];
                        }
                    }
                }
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer) {
            base.WriteJsonOverride(writer, serializer);
            writer.WriteValue(serializer, nameof(Signature), Signature.ToString());
            if (Sections.Any()) {
                writer.WritePropertyName(nameof(Sections));
                using (writer.ObjectScope(serializer)) {
                    writer.WriteValue(serializer, nameof(Sections.Count), Sections.Count);
                    writer.WritePropertyName("[Self]");
                    using (writer.ArrayScope(serializer)) {
                        foreach (var section in Sections) {
                            section.WriteJson(writer, serializer);
                            }
                        }
                    }
                }
            }

        protected override void WriteTextHeader(Int32 offset, TextWriter writer) {
            base.WriteTextHeader(offset, writer);
            var IndentString = new String(' ', offset);
            writer.WriteLine($"{IndentString}{(Int32)Signature,8:X}: signature ({Signature})");
            writer.WriteLine($"{IndentString}{Sections.Count,8:X}: number of sections");
            }

        protected override void WriteTextBody(Int32 offset, TextWriter writer) {
            var IndentString = new String(' ', offset);
            if (Sections.Any()) {
                writer.WriteLine();
                foreach (var section in Sections) {
                    writer.WriteLine($"{IndentString}  SYMBOL SECTION {section.Offset:X6} ({section.Type})");
                    section.WriteText(offset + 2, writer);
                    }
                }
            }
        }
    }