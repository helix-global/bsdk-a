using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryStudio.PortableExecutable.Win32;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewSymbolsSSection : CodeViewPrimarySSection
        {
        public override DEBUG_S Type { get { return DEBUG_S.DEBUG_S_SYMBOLS; }}
        public CV_CPU_TYPE? Machine { get;internal set; }
        public IList<CodeViewSymbol> Symbols { get; }
        internal unsafe CodeViewSymbolsSSection(CodeViewSection section, Int32 offset, Byte* content, Int32 length)
            : base(section, offset, content, length)
            {
            Machine = section.CommonObjectFile.CPU;
            Symbols = new List<CodeViewSymbol>();
            try
                {
                var r = content;
                while (length > 0) {
                    var header = (DEBUG_SYMBOL_HEADER*)r;
                    length -= header->Length + sizeof(Int16);
                    Symbols.Add(CodeViewSymbol.From(
                        this,
                        offset,
                        header->Type,
                        (Byte*)(header + 1),
                        header->Length - sizeof(Int16)));
                    r += sizeof(DEBUG_SYMBOL_HEADER);
                    r += header->Length - sizeof(Int16);
                    offset += sizeof(DEBUG_SYMBOL_HEADER) + header->Length - sizeof(Int16);
                    }
                }
            finally
                {
                Symbols = Symbols.ToArray();
                }
            }

        #region M:ToString(DEBUG_TYPE_ENUM):String
        protected static String ToString(DEBUG_TYPE_ENUM value) {
            return (value >= CV_FIRST_NONPRIM)
                ? ((UInt32)value).ToString("X6")
                : value.ToString();
            }
        #endregion
        #region M:ToString(DEBUG_SYMBOL_INDEX):String
        protected static String ToString(DEBUG_SYMBOL_INDEX value) {
            return (Enum.IsDefined(typeof(DEBUG_SYMBOL_INDEX), value))
                ? value.ToString()
                : $"DEBUG_SYMBOL({(UInt16)value:X4})";
            }
        #endregion
        #region M:JsonCalculateStringLength(String):Int32
        private static Int32 JsonCalculateStringLength(String value) {
            var r = 0;
            if (value != null) {
                var c = value.Length;
                for (var i = 0; i < c; i++) {
                    r++;
                    switch(value[i]) {
                        case '"' :
                        case '\\':
                            {
                            r++;
                            }
                            break;
                        }
                    }
                }
            return r;
            }
        #endregion
        #region M:WriteJson(JsonWriter,JsonSerializer)
        protected internal override unsafe void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, nameof(Offset), Offset.ToString("X6"));
                writer.WriteValue(serializer, nameof(Length), Length.ToString("X6"));
                writer.WriteValue(serializer, nameof(Type), Type.ToString());
                if (Symbols.Count > 0) {
                    writer.WritePropertyName(nameof(Symbols));
                    using (writer.ObjectScope(serializer)) {
                        writer.WriteValue(serializer, nameof(Symbols.Count), Symbols.Count);
                        writer.WritePropertyName("[Self]");
                        using (writer.ArrayScope(serializer)) {
                            var T = stackalloc[] { 0, 0, 0, 0, 0, 0};
                            var symbols = Symbols.ToArray();
                            foreach (var I in symbols) {
                                T[0] = Math.Max(T[0], ToString(I.Type).Length);
                                }
                            var firstitem = true;
                            using (writer.SuspendFormatingScope()) {
                                foreach (var g in symbols.GroupBy(i => i.Type)) {
                                    switch(g.Key) {
                                        #region S_GDATA32,S_LDATA32,S_GTHREAD32
                                        case DEBUG_SYMBOL_INDEX.S_GDATA32:
                                        case DEBUG_SYMBOL_INDEX.S_LDATA32:
                                        case DEBUG_SYMBOL_INDEX.S_GTHREAD32:
                                            {
                                            break;
                                            T[1] = 0;
                                            T[2] = 0;
                                            foreach (var I in g.OfType<S_DATASYM32>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                T[2] = Math.Max(T[2], ToString(I.TypeIndex).Length);
                                                }
                                            foreach (var I in g.OfType<S_DATASYM32>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var name = I.Value;
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, "Address", $"{I.Segment:X4}:{I.Offset:X8}");
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[2] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_WITH32,S_BLOCK32
                                        case DEBUG_SYMBOL_INDEX.S_BLOCK32:
                                        case DEBUG_SYMBOL_INDEX.S_WITH32:
                                            {
                                            break;
                                            T[1] = 0;
                                            foreach (var I in g.OfType<S_BLOCKSYM32>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                }
                                            foreach (var I in g.OfType<S_BLOCKSYM32>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var name = I.Value;
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, "Address", $"{I.Segment:X4}:{I.Offset:X8}");
                                                    writer.WriteValue(serializer, nameof(I.pParent), I.pParent.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.pEnd), I.pEnd.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.len), I.len.ToString("X8"));
                                                    if (!String.IsNullOrEmpty(name)) {
                                                        writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_LABEL32
                                        case DEBUG_SYMBOL_INDEX.S_LABEL32:
                                            {
                                            break;
                                            T[1] = 0;
                                            T[2] = 0;
                                            T[3] = 0;
                                            foreach (var I in g.OfType<S_LABEL32>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                T[2] = Math.Max(T[2], ToString(I.TypeIndex).Length);
                                                T[3] = Math.Max(T[3], I.Flags.ToString().Length);
                                                }
                                            foreach (var I in g.OfType<S_LABEL32>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var flags = I.Flags.ToString();
                                                    var name = I.Value;
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, "Address", $"{I.Segment:X4}:{I.Offset:X8}");
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[2] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.Flags), flags, T[3] - flags.Length);
                                                    writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_GPROC32_ID,S_LPROC32_DPC,S_LPROC32_DPC_ID,S_LPROC32_ID,S_LPROC32,S_GPROC32
                                        case DEBUG_SYMBOL_INDEX.S_GPROC32_ID:
                                        case DEBUG_SYMBOL_INDEX.S_GPROC32:
                                        case DEBUG_SYMBOL_INDEX.S_LPROC32_ID:
                                        case DEBUG_SYMBOL_INDEX.S_LPROC32_DPC:
                                        case DEBUG_SYMBOL_INDEX.S_LPROC32_DPC_ID:
                                        case DEBUG_SYMBOL_INDEX.S_LPROC32:
                                            {
                                            break;
                                            T[1] = 0;
                                            T[2] = 0;
                                            T[3] = 0;
                                            foreach (var I in g.OfType<S_PROCSYM32>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                T[2] = Math.Max(T[2], ToString(I.TypeIndex).Length);
                                                T[3] = Math.Max(T[3], I.Flags.ToString().Length);
                                                }
                                            foreach (var I in g.OfType<S_PROCSYM32>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var flags = I.Flags.ToString();
                                                    var name = I.Value;
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, "Address", $"{I.Segment:X4}:{I.Offset:X8}");
                                                    writer.WriteValue(serializer, nameof(I.Length), I.Length.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.Parent), $"{I.Parent:X8}");
                                                    writer.WriteValue(serializer, nameof(I.End), $"{I.End:X8}");
                                                    writer.WriteValue(serializer, nameof(I.Next), $"{I.Next:X8}");
                                                    writer.WriteValue(serializer, "Debug", $"{I.DbgStart:X8}:{I.DbgEnd:X8}");
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[2] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.Flags), flags, T[3] - flags.Length);
                                                    writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_BPREL32
                                        case DEBUG_SYMBOL_INDEX.S_BPREL32:
                                            {
                                            break;
                                            T[1] = 0;
                                            T[2] = 0;
                                            foreach (var I in g.OfType<S_BPREL32>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                T[2] = Math.Max(T[2], ToString(I.TypeIndex).Length);
                                                }
                                            foreach (var I in g.OfType<S_BPREL32>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var name = I.Value;
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, "BP", $"[{I.Offset:X8}]");
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[2] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_REGREL32
                                        case DEBUG_SYMBOL_INDEX.S_REGREL32:
                                            {
                                            break;
                                            T[1] = 0;
                                            T[2] = 0;
                                            T[3] = 0;
                                            foreach (var I in g.OfType<S_REGREL32>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                T[2] = Math.Max(T[2], ToString(I.TypeIndex).Length);
                                                T[3] = Math.Max(T[3], $"[{I.Registry}+{I.Offset:X8}]".Length);
                                                }
                                            foreach (var I in g.OfType<S_REGREL32>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var name = I.Value;
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    var rg = $"[{I.Registry}+{I.Offset:X8}]";
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, "REL", rg, T[3] - rg.Length);
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[2] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_THUNK32
                                        case DEBUG_SYMBOL_INDEX.S_THUNK32:
                                            {
                                            break;
                                            T[1] = 0;
                                            T[2] = 0;
                                            T[3] = 0;
                                            foreach (var I in g.OfType<S_THUNK32>()) {
                                                T[1] = Math.Max(T[1], I.Name.Length);
                                                T[2] = Math.Max(T[2], I.ord.ToString().Length);
                                                }
                                            foreach (var I in g.OfType<S_THUNK32>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var name = I.Name;
                                                    var ord = I.ord.ToString();
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.len), I.len.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.pParent), I.pParent.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.pEnd), I.pEnd.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.pNext), I.pNext.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.ord), ord, T[2] - ord.Length);
                                                    writer.WriteValue(serializer, nameof(I.Name), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_UDT
                                        case DEBUG_SYMBOL_INDEX.S_UDT:
                                            {
                                            break;
                                            T[1] = 0;
                                            T[2] = 0;
                                            foreach (var I in g.OfType<S_UDT>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                T[2] = Math.Max(T[2], ToString(I.TypeIndex).Length);
                                                }
                                            foreach (var I in g.OfType<S_UDT>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var name = I.Value;
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[2] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_BUILDINFO
                                        case DEBUG_SYMBOL_INDEX.S_BUILDINFO:
                                            {
                                            break;
                                            T[1] = 0;
                                            foreach (var I in g.OfType<S_BUILDINFO>()) {
                                                T[1] = Math.Max(T[1], ToString(I.TypeIndex).Length);
                                                }
                                            foreach (var I in g.OfType<S_BUILDINFO>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[1] - fieldtypeindex.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_HEAPALLOCSITE
                                        case DEBUG_SYMBOL_INDEX.S_HEAPALLOCSITE:
                                            {
                                            break;
                                            T[1] = 0;
                                            foreach (var I in g.OfType<S_HEAPALLOCSITE>()) {
                                                T[1] = Math.Max(T[1], ToString(I.TypeIndex).Length);
                                                }
                                            foreach (var I in g.OfType<S_HEAPALLOCSITE>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[1] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.cbInstr), I.cbInstr.ToString("X4"));
                                                    writer.WriteValue(serializer, nameof(I.off), I.off.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.sect), I.sect.ToString("X4"));
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_CALLSITEINFO
                                        case DEBUG_SYMBOL_INDEX.S_CALLSITEINFO:
                                            {
                                            break;
                                            T[1] = 0;
                                            foreach (var I in g.OfType<S_CALLSITEINFO>()) {
                                                T[1] = Math.Max(T[1], ToString(I.TypeIndex).Length);
                                                }
                                            foreach (var I in g.OfType<S_CALLSITEINFO>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var fieldtypeindex = ToString(I.TypeIndex);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.TypeIndex), fieldtypeindex, T[1] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.off), I.off.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.sect), I.sect.ToString("X4"));
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_FRAMECOOKIE
                                        case DEBUG_SYMBOL_INDEX.S_FRAMECOOKIE:
                                            {
                                            break;
                                            T[1] = 0;
                                            foreach (var I in g.OfType<S_FRAMECOOKIE>()) {
                                                T[1] = Math.Max(T[1], I.cookietype.ToString().Length);
                                                }
                                            foreach (var I in g.OfType<S_FRAMECOOKIE>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var cookie = I.cookietype.ToString();
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), ((CodeViewSymbol)I).Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.cookietype), cookie, T[1] - cookie.Length);
                                                    writer.WriteValue(serializer, nameof(I.reg), I.reg.ToString("X4"));
                                                    writer.WriteValue(serializer, nameof(I.off), I.off.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.flags), I.flags.ToString("X2"));
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_UNAMESPACE
                                        case DEBUG_SYMBOL_INDEX.S_UNAMESPACE:
                                            {
                                            break;
                                            T[1] = 0;
                                            foreach (var I in g.OfType<S_UNAMESPACE>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                }
                                            foreach (var I in g.OfType<S_UNAMESPACE>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var name = I.Value;
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), I.Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_FRAMEPROC
                                        case DEBUG_SYMBOL_INDEX.S_FRAMEPROC:
                                            {
                                            break;
                                            T[1] = 0;
                                            foreach (var I in g.OfType<S_FRAMEPROC>()) {
                                                T[1] = Math.Max(T[1], I.Flags.ToString().Length);
                                                }
                                            writer.Formatting = Formatting.Indented;
                                            foreach (var I in g.OfType<S_FRAMEPROC>()) {
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var flags = I.Flags.ToString();
                                                    writer.WriteValue(serializer, nameof(I.Type), type);
                                                    writer.WriteValue(serializer, nameof(I.Offset), I.Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.Flags), flags);
                                                    writer.WriteValue(serializer, nameof(I.cbFrame), I.cbFrame.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.cbPad), I.cbPad.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.offPad), I.offPad.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.cbSaveRegs), I.cbSaveRegs.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.offExHdlr), I.offExHdlr.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.sectExHdlr), I.sectExHdlr.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.LocalBasePointer), I.LocalBasePointer);
                                                    writer.WriteValue(serializer, nameof(I.ParamBasePointer), I.ParamBasePointer);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_OBJNAME
                                        case DEBUG_SYMBOL_INDEX.S_OBJNAME:
                                            {
                                            break;
                                            T[1] = 0;
                                            foreach (var I in g.OfType<S_OBJNAME>()) {
                                                T[1] = Math.Max(T[1], I.Value.Length);
                                                }
                                            foreach (var I in g.OfType<S_OBJNAME>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var name = I.Value;
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), I.Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.Signature), I.Signature.ToString("X8"));
                                                    writer.WriteValue(serializer, nameof(I.Value), name, T[1] - name.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_COMPILE3
                                        case DEBUG_SYMBOL_INDEX.S_COMPILE3:
                                            {
                                            break;
                                            writer.Formatting = Formatting.Indented;
                                            foreach (var I in g.OfType<S_COMPILE3>()) {
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), I.Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.Language), I.Language.ToString());
                                                    writer.WriteValue(serializer, nameof(I.Machine), I.Machine.ToString());
                                                    writer.WriteValue(serializer, nameof(I.IsCompiledForEditAndContinue), I.IsCompiledForEditAndContinue);
                                                    writer.WriteValue(serializer, nameof(I.IsCompiledWithDebugInfo), I.IsCompiledWithDebugInfo);
                                                    writer.WriteValue(serializer, nameof(I.IsCompiledWithLTCG), I.IsCompiledWithLTCG);
                                                    writer.WriteValue(serializer, nameof(I.IsNoDataAlign), I.IsNoDataAlign);
                                                    writer.WriteValue(serializer, nameof(I.IsManagedPresent), I.IsManagedPresent);
                                                    writer.WriteValue(serializer, nameof(I.IsSecurityChecks), I.IsSecurityChecks);
                                                    writer.WriteValue(serializer, nameof(I.IsHotPatch), I.IsHotPatch);
                                                    writer.WriteValue(serializer, nameof(I.IsConvertedWithCVTCIL), I.IsConvertedWithCVTCIL);
                                                    writer.WriteValue(serializer, nameof(I.IsMSILModule), I.IsMSILModule);
                                                    writer.WriteValue(serializer, nameof(I.IsCompiledWithSDL), I.IsCompiledWithSDL);
                                                    writer.WriteValue(serializer, nameof(I.IsCompiledWithPGO), I.IsCompiledWithPGO);
                                                    writer.WriteValue(serializer, nameof(I.IsEXPModule), I.IsEXPModule);
                                                    writer.WriteValue(serializer, nameof(I.FrontEndVersion), I.FrontEndVersion.ToString());
                                                    writer.WriteValue(serializer, nameof(I.BackEndVersion), I.BackEndVersion.ToString());
                                                    writer.WriteValue(serializer, nameof(I.CompilerVersion), I.CompilerVersion);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_ENVBLOCK
                                        case DEBUG_SYMBOL_INDEX.S_ENVBLOCK:
                                            {
                                            break;
                                            writer.Formatting = Formatting.Indented;
                                            foreach (var I in g.OfType<S_ENVBLOCK>()) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    var nfirstitem = true;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), I.Offset.ToString("X6"));
                                                    writer.WritePropertyName(nameof(I.Values));
                                                    using (writer.ArrayScope(serializer)) {
                                                        T[1] = 0;
                                                        T[2] = 0;
                                                        foreach (var value in I.Values) {
                                                            T[1] = Math.Max(T[1], value.Key.Length);
                                                            T[2] = Math.Max(T[2], JsonCalculateStringLength(value.Value));
                                                            }
                                                        using (writer.SuspendFormatingScope()) {
                                                            foreach (var value in I.Values) {
                                                                ValidateFirstItem(writer, ref nfirstitem);
                                                                using (writer.ObjectScope(serializer)) {
                                                                    writer.Formatting = Formatting.None;
                                                                    writer.WriteValue(serializer, "Key", value.Key, T[1] - value.Key.Length);
                                                                    writer.WriteValue(serializer, "Value", value.Value, T[2] - JsonCalculateStringLength(value.Value));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        #region S_CONSTANT
                                        case DEBUG_SYMBOL_INDEX.S_CONSTANT:
                                            {
                                            break;
                                            T[1] = 0;
                                            T[2] = 0;
                                            T[3] = 0;
                                            T[4] = 0;
                                            T[5] = 0;
                                            foreach (var I in g.OfType<S_CONSTANT>()) {
                                                T[1] = Math.Max(T[1], String.Format("[{0}:{1}{2}]",
                                                    I.FieldValue.Length.ToString($"X3"),
                                                    String.Join(String.Empty, I.FieldValue.Select(i => i.ToString("X2"))),
                                                    (I.FieldValueType == null)
                                                        ? String.Empty
                                                        : String.Format(":{0}", I.FieldValueType.Value.ToString())).Length);
                                                T[2] = Math.Max(T[2], ToString(I.FieldTypeIndex).Length);
                                                T[5] = Math.Max(T[5], I.FieldName.Length);
                                                }
                                            foreach (var I in g.OfType<S_CONSTANT>().OrderBy(i => i.Offset)) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var fieldtypeindex = ToString(I.FieldTypeIndex);
                                                    var fieldname = I.FieldName;
                                                    var type = ToString(I.Type);
                                                    var fieldvalue = String.Format("[{0}:{1}{2}]",
                                                        I.FieldValue.Length.ToString($"X3"),
                                                        String.Join(String.Empty, I.FieldValue.Select(i => i.ToString("X2"))),
                                                        (I.FieldValueType == null)
                                                            ? String.Empty
                                                            : String.Format(":{0}", I.FieldValueType.Value.ToString()));
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), I.Offset.ToString("X6"));
                                                    writer.WriteValue(serializer, nameof(I.FieldTypeIndex), fieldtypeindex, T[2] - fieldtypeindex.Length);
                                                    writer.WriteValue(serializer, nameof(I.FieldValue), fieldvalue, T[1]-fieldvalue.Length);
                                                    writer.WriteValue(serializer, nameof(I.FieldName), fieldname, T[5] - fieldname.Length);
                                                    }
                                                }
                                            }
                                            break;
                                        #endregion
                                        default:
                                            {
                                            foreach (var I in g) {
                                                ValidateFirstItem(writer, ref firstitem);
                                                using (writer.ObjectScope(serializer)) {
                                                    var type = ToString(I.Type);
                                                    writer.Formatting = Formatting.None;
                                                    writer.WriteValue(serializer, nameof(I.Type), type, T[0] - type.Length);
                                                    writer.WriteValue(serializer, nameof(I.Offset), I.Offset.ToString("X6"));
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        #endregion

        protected override void WriteTextHeader(Int32 offset, TextWriter writer) {
            base.WriteTextHeader(offset, writer);
            var IndentString = new String(' ', offset);
            writer.WriteLine($"{IndentString}{Symbols.Count,8:X}: number of symbols");
            }

        protected override unsafe void WriteTextBody(Int32 offset, TextWriter writer) {
            base.WriteTextBody(offset, writer);
            var IndentString = new String(' ', offset);
            foreach (var g in Symbols.GroupBy(i => i.Type)) {
                writer.WriteLine($"\n{IndentString}  SYMBOLS({g.Key})");
                switch(g.Key) {
                    case DEBUG_SYMBOL_INDEX.S_CONSTANT:
                        {
                        var T = stackalloc[] { 7, 10 };
                        foreach (var symbol in g.OfType<S_CONSTANT>()) {
                            if (symbol.FieldValueType != null) {
                                T[0] = Math.Max(T[0], symbol.FieldValueType.GetValueOrDefault().ToString().Length);
                                }
                            T[1] = Math.Max(T[1], symbol.FieldName.Length);
                            }
                        writer.WriteLine($"{IndentString}           Field Type   Value    {String.Format($"{{0,-{T[0]}}}", " Value")}");
                        writer.WriteLine($"{IndentString}    Offset   Index      Length   {String.Format($"{{0,-{T[0]}}}", " Type")} Field Name");
                        writer.WriteLine($"{IndentString}    ------ ---------- --------- {new String('-', T[0])} {new String('-', T[1])}");
                        foreach (var symbol in g.OfType<S_CONSTANT>()) {
                            var r = new StringBuilder();
                            r.Append($"{IndentString}    {symbol.Offset:X6}   {(UInt32)symbol.FieldTypeIndex:X6}   {symbol.FieldValue.Length,9:X3} ");
                            r.Append((symbol.FieldValueType == null)
                                ? new String(' ', T[0])
                                : String.Format($"{{0,-{T[0]}}}", ((LEAF_ENUM)symbol.FieldValueType).ToString()));
                            r.Append(' ');
                            r.Append(symbol.FieldName);
                            writer.WriteLine(r);
                            }
                        }
                        break;
                    case DEBUG_SYMBOL_INDEX.S_UNAMESPACE:
                        {
                        var T = stackalloc[] { 11 };
                        foreach (var symbol in g.OfType<S_UNAMESPACE>()) {
                            T[0] = Math.Max(T[0], symbol.Value.Length);
                            }
                        writer.WriteLine($"{IndentString}    Offset  Namespace");
                        writer.WriteLine($"{IndentString}    ------ {new String('-', T[0])}");
                        foreach (var symbol in g.OfType<S_UNAMESPACE>()) {
                            writer.WriteLine($"{IndentString}    {symbol.Offset:X6}  {symbol.Value}");
                            }
                        }
                        break;
                    }
                }
            }

        public Object DecodeRegister(UInt16 value) {
            if (Machine != null) {
                switch (Machine.Value) {
                    case CV_CPU_TYPE.CV_CFL_8080:
                    case CV_CPU_TYPE.CV_CFL_8086:
                    case CV_CPU_TYPE.CV_CFL_80286:
                    case CV_CPU_TYPE.CV_CFL_80386:
                    case CV_CPU_TYPE.CV_CFL_80486:
                    case CV_CPU_TYPE.CV_CFL_PENTIUM:
                    case CV_CPU_TYPE.CV_CFL_PENTIUMII:
                    case CV_CPU_TYPE.CV_CFL_PENTIUMIII:
                        {
                        return (CV_REG)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_MIPS:
                    case CV_CPU_TYPE.CV_CFL_MIPS16:
                    case CV_CPU_TYPE.CV_CFL_MIPS32:
                    case CV_CPU_TYPE.CV_CFL_MIPS64:
                    case CV_CPU_TYPE.CV_CFL_MIPSI:
                    case CV_CPU_TYPE.CV_CFL_MIPSII:
                    case CV_CPU_TYPE.CV_CFL_MIPSIII:
                    case CV_CPU_TYPE.CV_CFL_MIPSIV:
                    case CV_CPU_TYPE.CV_CFL_MIPSV:
                        {
                        return (CV_M4)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_M68000:
                    case CV_CPU_TYPE.CV_CFL_M68010:
                    case CV_CPU_TYPE.CV_CFL_M68020:
                    case CV_CPU_TYPE.CV_CFL_M68030:
                    case CV_CPU_TYPE.CV_CFL_M68040:
                        {
                        return (CV_R68)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_ALPHA:
                    case CV_CPU_TYPE.CV_CFL_ALPHA_21164:
                    case CV_CPU_TYPE.CV_CFL_ALPHA_21164A:
                    case CV_CPU_TYPE.CV_CFL_ALPHA_21264:
                    case CV_CPU_TYPE.CV_CFL_ALPHA_21364:
                        {
                        return (CV_ALPHA)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_PPC601:
                    case CV_CPU_TYPE.CV_CFL_PPC603:
                    case CV_CPU_TYPE.CV_CFL_PPC604:
                    case CV_CPU_TYPE.CV_CFL_PPC620:
                    case CV_CPU_TYPE.CV_CFL_PPCFP:
                    case CV_CPU_TYPE.CV_CFL_PPCBE:
                        {
                        return (CV_PPC)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_SH3:
                    case CV_CPU_TYPE.CV_CFL_SH3E:
                    case CV_CPU_TYPE.CV_CFL_SH3DSP:
                    case CV_CPU_TYPE.CV_CFL_SH4:
                        {
                        return (CV_SH3)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_SHMEDIA:
                        {
                        return (CV_SHMEDIA)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_ARM3:
                    case CV_CPU_TYPE.CV_CFL_ARM4:
                    case CV_CPU_TYPE.CV_CFL_ARM4T:
                    case CV_CPU_TYPE.CV_CFL_ARM5:
                    case CV_CPU_TYPE.CV_CFL_ARM5T:
                    case CV_CPU_TYPE.CV_CFL_ARM6:
                    case CV_CPU_TYPE.CV_CFL_ARM_XMAC:
                    case CV_CPU_TYPE.CV_CFL_ARM_WMMX:
                    case CV_CPU_TYPE.CV_CFL_ARM7:
                    case CV_CPU_TYPE.CV_CFL_THUMB:
                    case CV_CPU_TYPE.CV_CFL_ARMNT:
                        {
                        return (CV_ARM)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_IA64:
                    case CV_CPU_TYPE.CV_CFL_IA64_2:
                        {
                        return (CV_IA64)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_AM33:
                        {
                        return (CV_AM33)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_M32R:
                        {
                        return (CV_M32R)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_TRICORE:
                        {
                        return (CV_TRI)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_ARM64:
                        {
                        return (CV_ARM64)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_X64:
                        {
                        return (CV_AMD64)value;
                        }
                    case CV_CPU_TYPE.CV_CFL_D3D11_SHADER:
                    case CV_CPU_TYPE.CV_CFL_OMNI:
                    case CV_CPU_TYPE.CV_CFL_CEE:
                    case CV_CPU_TYPE.CV_CFL_EBC:
                        {
                        return value.ToString("X4");
                        }
                    }
                }
            return value.ToString("X4");
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Type.ToString();
            }

        private const DEBUG_TYPE_ENUM CV_FIRST_NONPRIM = (DEBUG_TYPE_ENUM)0x1000;
        }
    }