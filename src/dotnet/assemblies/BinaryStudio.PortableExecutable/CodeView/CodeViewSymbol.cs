using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryStudio.PortableExecutable.Win32;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewSymbol : IJsonSerializable
        {
        public CodeViewSymbolsSSection Section { get; }
        public virtual DEBUG_SYMBOL_INDEX Type { get; }
        public virtual Byte[] Content { get; }
        public Int32 Offset { get; }

        private static readonly IDictionary<DEBUG_SYMBOL_INDEX,Type> types = new Dictionary<DEBUG_SYMBOL_INDEX, Type>();
        static CodeViewSymbol() {
            foreach (var type in typeof(CodeViewSymbol).Assembly.GetTypes()) {
                var key = type.GetCustomAttributes(false).OfType<CodeViewSymbolAttribute>().FirstOrDefault();
                if (key != null) {
                    types.Add(key.Key, type);
                    }
                }
            }

        private unsafe CodeViewSymbol(CodeViewSymbolsSSection section, Int32 offset, DEBUG_SYMBOL_INDEX type, Byte* content, Int32 length)
            :this(section, offset, (IntPtr)content, length)
            {
            Type = type;
            }

        protected unsafe CodeViewSymbol(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            {
            if (content == null) { throw new ArgumentNullException(nameof(content)); }
            Section = section;
            Offset = offset;
            var c = (Byte*)content;
            var r = new Byte[length];
            for (var i = 0; i < length; ++i) {
                r[i] = c[i];
                }
            Content = r;
            MaxSizeLength = Math.Max(MaxSizeLength, Content.Length.ToString("X").Length);
            }

        public static unsafe CodeViewSymbol From(CodeViewSymbolsSSection section, Int32 offset, DEBUG_SYMBOL_INDEX index, Byte* content, Int32 length) {
            if (types.TryGetValue(index,out var type)) {
                return (CodeViewSymbol)Activator.CreateInstance(type,
                    section,
                    offset,
                    (IntPtr)content,
                    length);
                }
            Console.Error.WriteLine($"{index}");
            return new CodeViewSymbol(section, offset, index, content, length);
            }

        #region M:WriteJsonOverride(JsonWriter,JsonSerializer)
        protected virtual void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer) {
            writer.WriteValue(serializer, "Length", Content.Length.ToString($"X{MaxSizeLength}"));
            writer.WriteValue(serializer, "Offset", Offset.ToString($"X4"));
            writer.WriteValue(serializer, nameof(Type), Type.ToString());
            }
        #endregion
        #region M:WriteJson(JsonWriter,JsonSerializer)
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            using (writer.ObjectScope(serializer)) {
                writer.Formatting = Formatting.None;
                WriteJsonOverride(writer, serializer);
                }
            }
        #endregion
        #region M:ToArray(Byte*,Int32):Byte[]
        protected static unsafe Byte[] ToArray(Byte* content, Int32 length) {
            var r = new Byte[length];
            for (var i = 0; i < length; i++) {
                r[i] = content[i];
                }
            return r;
            }
        #endregion
        #region M:ToString(DEBUG_TYPE_ENUM):String
        protected static String ToString(DEBUG_TYPE_ENUM value) {
            return (value >= CV_FIRST_NONPRIM)
                ? ((UInt32)value).ToString("X6")
                : value.ToString();
            }
        #endregion
        #region M:ToString(Encoding,Byte*,Boolean):String
        protected static unsafe String ToString(Encoding encoding, Byte* value, Boolean lengthprefixed) {
            if (lengthprefixed) {
                var c = (Int32)(*value);
                var r = new Byte[c];
                for (var i = 0;i < c;++i) {
                    r[i] = value[i + 1];
                    }
                return encoding.GetString(r);
                }
            else
                {
                var r = new List<Byte>();
                while (*value != 0) {
                    r.Add(*value);
                    value++;
                    }
                return encoding.GetString(r.ToArray());
                }
            }
        #endregion
        #region M:ReadString(Encoding,[Ref]Byte*,Boolean):String
        protected static unsafe String ReadString(Encoding encoding, ref Byte* value, Boolean lengthprefixed) {
            if (lengthprefixed) {
                var c = (Int32)(*value);
                var r = new Byte[c];
                for (var i = 0;i < c;++i) {
                    r[i] = value[i + 1];
                    }
                value += sizeof(Int32) + c;
                return encoding.GetString(r);
                }
            else
                {
                var r = new List<Byte>();
                while (*value != 0) {
                    r.Add(*value);
                    value++;
                    }
                return encoding.GetString(r.ToArray());
                }
            }
        #endregion

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Type.ToString();
            }

        private static Int32 MaxSizeLength;
        private const DEBUG_TYPE_ENUM CV_FIRST_NONPRIM = (DEBUG_TYPE_ENUM)0x1000;
        }
    }