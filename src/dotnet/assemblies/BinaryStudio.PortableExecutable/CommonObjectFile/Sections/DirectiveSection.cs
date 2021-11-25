using System;
using System.Text;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable.CommonObjectFile.Sections
    {
    public class DirectiveSection : CommonObjectFileSection
        {
        public String LinkerDirectives { get; }
        internal unsafe DirectiveSection(CommonObjectFileSource o, Int32 index, Byte* mapping, IMAGE_SECTION_HEADER* section)
            : base(o, index, mapping, section)
            {
            var content = GetBytes(section->PointerToRawData + mapping, section->SizeOfRawData);
            LinkerDirectives = Encoding.UTF8.GetString(content).Trim();
            #if DEBUG
            RawDataUtilization = 1;
            #endif
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer) {
            base.WriteJsonOverride(writer, serializer);
            if (!String.IsNullOrWhiteSpace(LinkerDirectives)) {
                writer.WriteValue(serializer, nameof(LinkerDirectives), LinkerDirectives);
                }
            }
        }
    }