using BinaryStudio.PortableExecutable.Win32;
using BinaryStudio.Serialization;
using System;
using System.IO;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public abstract class CodeViewPrimarySSection : IJsonSerializable
        {
        public abstract DEBUG_S Type { get; }
        public CodeViewSection Section { get; }
        public Int32 Length { get; }
        public Int32 Offset { get; }
        internal static unsafe CodeViewPrimarySSection From(CodeViewSection section, Int32 offset, DEBUG_S type, Int32 length, Byte* content) {
            switch (type) {
                case DEBUG_S.DEBUG_S_SYMBOLS:              { return new CodeViewSymbolsSSection(section, offset, content, length);              }
                case DEBUG_S.DEBUG_S_LINES:                { return new CodeViewLinesSSection(section, offset, content, length);                }
                case DEBUG_S.DEBUG_S_STRINGTABLE:          { return new CodeViewStringTableSSection(section, offset, content, length);          }
                case DEBUG_S.DEBUG_S_FILECHKSMS:           { return new CodeViewFileHashTableSSection(section, offset, content, length);        }
                case DEBUG_S.DEBUG_S_FRAMEDATA:            { return new CodeViewFrameDataSSection(section, offset, content, length);            }
                case DEBUG_S.DEBUG_S_INLINEELINES:         { return new CodeViewInlineELinesSSection(section, offset, content, length);         }
                case DEBUG_S.DEBUG_S_CROSSSCOPEIMPORTS:    { return new CodeViewCrossScopeImportsSSection(section, offset, content, length);    }
                case DEBUG_S.DEBUG_S_CROSSSCOPEEXPORTS:    { return new CodeViewCrossScopeExportsSSection(section, offset, content, length);    }
                case DEBUG_S.DEBUG_S_IL_LINES:             { return new CodeViewILLinesSSection(section, offset, content, length);              }
                case DEBUG_S.DEBUG_S_FUNC_MDTOKEN_MAP:     { return new CodeViewFuncMetadataTokenMapSSection(section, offset, content, length); }
                case DEBUG_S.DEBUG_S_TYPE_MDTOKEN_MAP:     { return new CodeViewTypeMetadataTokenMapSSection(section, offset, content, length); }
                case DEBUG_S.DEBUG_S_MERGED_ASSEMBLYINPUT: { return new CodeViewMergedAssemblyInputSSection(section, offset, content, length);  }
                case DEBUG_S.DEBUG_S_COFF_SYMBOL_RVA:      { return new CodeViewSymbolRVASSection(section, offset, content, length);            }
                default: { throw new ArgumentOutOfRangeException(nameof(type), type, null); }
                }
            }

        protected static Int32 CalculateLength(String value)
            {
            var c = 0;
            for (var i = 0; i < value.Length; i++) {
                c++;
                if (value[i] == '\\') {
                    c++;
                    }
                }
            return c;
            }

        protected unsafe CodeViewPrimarySSection(CodeViewSection section, Int32 offset, Byte* content, Int32 length)
            {
            Offset = offset;
            Length = length;
            Section = section;
            }

        #region M:WriteJsonOverride(JsonWriter,JsonSerializer)
        protected virtual void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer) {
            writer.WriteValue(serializer, nameof(Type), Type.ToString());
            writer.WriteValue(serializer, nameof(Offset), Offset.ToString("X4"));
            writer.WriteValue(serializer, nameof(Length), Length);
            }
        #endregion
        #region M:IJsonSerializable.WriteJson(JsonWriter,JsonSerializer)
        void IJsonSerializable.WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            WriteJson(writer, serializer);
            }
        protected internal virtual void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            using (writer.ObjectScope(serializer)) {
                WriteJsonOverride(writer, serializer);
                }
            }
        #endregion
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
            }

        protected virtual void WriteTextBody(Int32 offset, TextWriter writer) {
            }

        protected internal void WriteText(Int32 offset, TextWriter writer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            WriteTextHeader(offset, writer);
            WriteTextBody(offset, writer);
            }

        #region M:ReadInt32([Ref]Byte*):Int32
        private static unsafe Int32 ReadInt32(ref Byte* source)
            {
            var r = *((Int32*)source);
            source += sizeof(Int32);
            return r;
            }
        #endregion
        #region M:ReadUInt32([Ref]Byte*):UInt32
        protected static unsafe UInt32 ReadUInt32(ref Byte* source)
            {
            var r = *((UInt32*)source);
            source += sizeof(UInt32);
            return r;
            }
        #endregion
        #region M:ValidateFirstItem(JsonWriter,[Ref]Boolean)
        protected static void ValidateFirstItem(JsonWriter writer, ref Boolean firstitem) {
            if (firstitem)
                {
                //writer.WriteIndent();
                firstitem = false;
                }
            else
                {
                writer.Formatting = Formatting.Indented;
                }
            }
        #endregion
        }
    }