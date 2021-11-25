using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using BinaryStudio.PortableExecutable.Win32;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace BinaryStudio.PortableExecutable
    {
    public class CommonObjectFileSection : IJsonSerializable
        {
        public CommonObjectFileSource CommonObjectFile { get; }
        public Int64 VirtualAddress { get; }
        public Int64 PointerToRelocations { get; }
        public Int64 PointerToLineNumbers { get; }
        public Int32 NumberOfRelocations { get; }
        public Int32 NumberOfLineNumbers { get; }
        public Int32 Index { get; }
        public String Name { get; }
        public IMAGE_SCN Characteristics { get; }
        public IList<Relocation> Relocations { get; }
        public Byte[] Content { get; }
        #if DEBUG
        public Double RawDataUtilization { get;protected set; }
        #endif

        internal unsafe CommonObjectFileSection(CommonObjectFileSource o, Int32 index, Byte* mapping, IMAGE_SECTION_HEADER* section)
            {
            CommonObjectFile = o;
            Index = index;
            #if DEBUG
            var n = 0;
            #endif
            Content = CommonObjectFileSource.GetBytes(section->PointerToRawData + mapping, section->SizeOfRawData);
            Name = section->ToString();
            VirtualAddress = section->VirtualAddress;
            PointerToRelocations = section->PointerToRawData;
            PointerToLineNumbers = section->PointerToRelocations;
            NumberOfRelocations = section->NumberOfRelocations;
            NumberOfLineNumbers = section->NumberOfLineNumbers;
            Characteristics = section->Characteristics;
            Relocations = EmptyList<Relocation>.Value;
            #if DEBUG
            n += sizeof(IMAGE_SECTION_HEADER);
            #endif
            if (section->NumberOfRelocations > 0) {
                var relocations = (IMAGE_RELOCATION*)(section->PointerToRelocations + mapping);
                var r = new List<Relocation>();
                for (var i = 0; i < section->NumberOfRelocations; i++) {
                    var relocation = &relocations[i];
                    r.Add(new Relocation(o, o.Machine, relocation));
                    }
                Relocations = new ReadOnlyCollection<Relocation>(r);
                }
            }

        protected virtual void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer) {
            writer.WriteValue(serializer, nameof(Name), Name);
            writer.WriteValue(serializer, nameof(Index), Index.ToString("X4"));
            //writer.WriteValue(serializer, nameof(VirtualAddress), VirtualAddress.ToString("X"));
            //writer.WriteValue(serializer, nameof(PointerToRelocations), PointerToRelocations.ToString("X"));
            //writer.WriteValue(serializer, nameof(PointerToLineNumbers), PointerToLineNumbers.ToString("X"));
            //writer.WriteValue(serializer, nameof(NumberOfLineNumbers), NumberOfLineNumbers.ToString());
            //writer.WriteValue(serializer, nameof(Characteristics), Characteristics.ToString());
            return;
            if (Relocations.Count > 0) {
                writer.WritePropertyName(nameof(Relocations));
                using (writer.ObjectScope(serializer)) {
                    writer.WriteValue(serializer, nameof(Relocations.Count), Relocations.Count);
                    writer.WritePropertyName("[Self]");
                    using (writer.ArrayScope(serializer)) {
                        const Int32 OFST = 0;
                        const Int32 INDX = 1;
                        const Int32 NAME = 2;
                        var W = new Int32[3];
                        var formatting = writer.Formatting;
                        writer.Formatting = Formatting.None;
                        var i = 0;
                        var Moffset = 0L;
                        var Wtype  = 0;
                        var Mindex = 0;
                        foreach (var relocation in Relocations)
                            {
                            Moffset = Math.Max(Moffset, relocation.Offset);
                            Mindex  = Math.Max(Mindex, relocation.SymbolIndex);
                            Wtype = Math.Max(Wtype, relocation.ToString().Length);
                            W[NAME] = Math.Max(W[NAME], relocation.SymbolName.Length);
                            }
                        W[OFST] = Moffset.ToString("X").Length;
                        W[INDX] = Mindex.ToString("X").Length;
                        foreach (var relocation in Relocations) {
                            if (i == 0)
                                {
                                //writer.WriteIndent();
                                }
                            else
                                {
                                writer.Formatting = Formatting.Indented;
                                }
                            using (writer.ObjectScope(serializer)) {
                                var type = relocation.ToString();
                                var name = relocation.SymbolName;
                                writer.Formatting = Formatting.None;
                                writer.WriteValue(serializer, "Offset",  relocation.Offset.ToString($"X{W[OFST]}"));
                                writer.WriteValue(serializer, "Type",  type);
                                //writer.WriteIndentSpace(Wtype - type.Length);
                                writer.WriteValue(serializer, "SymbolIndex",  relocation.SymbolIndex.ToString($"X{W[INDX]}"));
                                writer.WriteValue(serializer, "SymbolName",   relocation.SymbolName);
                                //writer.WriteIndentSpace(W[NAME] - name.Length);
                                }
                            i++;
                            }
                        writer.Formatting = formatting;
                        }
                    }
                }
            }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            using (writer.ObjectScope(serializer)) {
                WriteJsonOverride(writer, serializer);
                }
            }

        #region M:GetBytes(Byte*,Int64):Byte[]
        protected static unsafe Byte[] GetBytes(Byte* source, Int64 size)
            {
            if (source == null) { return null; }
            var r = new Byte[size];
            for (var i = 0;i < size;++i) {
                r[i] = source[i];
                }
            return r;
            }
        #endregion

        protected virtual void WriteTextHeader(Int32 offset, TextWriter writer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            var IndentString = new String(' ', offset);
            writer.WriteLine($"{IndentString}{Name,8}: name");
            writer.WriteLine($"{IndentString}{VirtualAddress,8:X}: virtual address");
            writer.WriteLine($"{IndentString}{Content.Length,8:X}: size of raw data");
            writer.WriteLine($"{IndentString}{PointerToRelocations,8:X}: file pointer to relocation table");
            writer.WriteLine($"{IndentString}{PointerToLineNumbers,8:X}: file pointer to line numbers");
            writer.WriteLine($"{IndentString}{NumberOfRelocations,8:X}: number of relocations");
            writer.WriteLine($"{IndentString}{NumberOfLineNumbers,8:X}: number of line numbers");
            writer.WriteLine($"{IndentString}{(UInt16)Characteristics,8:X}: flags ({Characteristics})");
            }

        protected virtual void WriteTextBody(Int32 offset, TextWriter writer) {
            }

        protected internal void WriteText(Int32 offset, TextWriter writer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            WriteTextHeader(offset, writer);
            WriteTextBody(offset, writer);
            }

        protected internal virtual void WriteXml(XmlWriter writer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Name;
            }
        }
    }