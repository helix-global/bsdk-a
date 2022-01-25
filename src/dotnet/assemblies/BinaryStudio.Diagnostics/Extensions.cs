using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BinaryStudio.Diagnostics
    {
    public static class Extensions
        {
        public static void Write(this TextWriter writer, Byte[] source) {
            using (var r = new MemoryStream(source)) {
                Write(writer, r);
                }
            }


        private static void WriteInternal(TextWriter writer, Byte[] source, Int32 size, Int32 offset)
            {
            var r = new StringBuilder();
            r.AppendFormat("{0:X8}: ", offset);
            for (var i = 0; i < 16; i++) {
                if (i < 8) {
                    if (i < size) {
                        r.AppendFormat("{0:X2} ", source[i]);
                        }
                    else
                        {
                        r.Append("   ");
                        }
                    }
                else
                    {
                    if (i == 8) {
                        if (i < size) {
                            r.Append("| ");
                            }
                        else
                            {
                            r.Append("  ");
                            }
                        }
                    if (i < size) {
                        r.AppendFormat("{0:X2} ", source[i]);
                        }
                    else
                        {
                        r.Append("   ");
                        }
                    }
                }
            for (var i = 0; i < 16; i++) {
                if (i >= size) { break; }
                var c = source[i];
                if (((c >= 'A') && (c <= 'Z')) ||
                    ((c >= 'a') && (c <= 'z')) ||
                    ((c >= '0') && (c <= '9')) ||
                    ((c >= 0x20) && (c <= 0x7E)))
                    {
                    r.Append((char)c);
                    }
                else
                    {
                    r.Append('.');
                    }
                }
            writer.WriteLine(r.ToString());
            }

        public static void Write(this TextWriter writer, Stream source)
            {
            source.Seek(0, SeekOrigin.Begin);
            var buffer = new Byte[16];
            var offset = 0;
            for (;;)
                {
                var sz = source.Read(buffer, 0, buffer.Length);
                if (sz == 0) { break; }
                WriteInternal(writer, buffer, sz, offset);
                offset += sz;
                }
            }

        #region M:WriteValue(JsonWriter,JsonSerializer,String,Object)
        internal static void WriteValue(this JsonWriter writer, JsonSerializer serializer, String name, Object value) {
            if (value != null) {
                writer.WritePropertyName(name);
                if (!WriteInternal(writer, serializer, value as IExceptionSerializable))
                if (!WriteInternal(writer, serializer, value as IList<String>)) {
                        var type = value.GetType();
                        if (type.IsEnum) {
                            if (!IsNullOrEmpty(type.GetCustomAttributes(typeof(FlagsAttribute), false))) {
                                value = Enum.Format(type, value, "F");
                                }
                            else
                                {
                                var utype = Enum.GetUnderlyingType(type);
                                if ((value is IConvertible) && (value.GetType() != utype)) {
                                    value = ((IConvertible)value).ToType(utype, CultureInfo.InvariantCulture);
                                    }
                                value = Enum.Format(type, value, "G");
                                }
                            }

                        try
                            {
                            writer.WriteValue(value);
                            }
                        catch (JsonWriterException)
                            {
                            (new ExceptionObjectDecorator(value)).WriteTo(writer, serializer);
                            }
                        }
                }
            }
        #endregion
        #region M:WriteValue(JsonWriter,JsonSerializer,String,String)
        internal static void WriteValue(this JsonWriter writer, JsonSerializer serializer, String name, String value) {
            if (value != null) {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
                }
            }
        #endregion
        #region M:WriteValue(JsonWriter,JsonSerializer,String,String,Int32)
        internal static void WriteValue(this JsonWriter writer, JsonSerializer serializer, String name, String value, Int32 spaces) {
            if (value != null) {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
                }
            }
        #endregion
        #region M:WriteValue(JsonWriter,JsonSerializer,String,Boolean,Int32)
        internal static void WriteValue(this JsonWriter writer, JsonSerializer serializer, String name, Boolean value, Int32 spaces) {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
            }
        #endregion
        #region M:WriteValue<T>(JsonWriter,JsonSerializer,String,IEnumerable<T>)
        internal static void WriteValue<T>(this JsonWriter writer, JsonSerializer serializer, String name, IEnumerable<T> values) {
            if (values != null) {
                if (values is IExceptionSerializable J)
                    {
                    writer.WritePropertyName(name);
                    J.WriteTo(writer, serializer);
                    }
                else
                    {
                    var i = 0;
                    foreach (var value in values) {
                        if (i == 0) {
                            writer.WritePropertyName(name);
                            writer.WriteStartArray();
                            }
                        i++;
                        WriteInternal(writer, serializer, value);
                        }
                    if (i > 0)
                        {
                        writer.WriteEndArray();    
                        }
                    }
                }
            }
        #endregion
        #region M:WriteInternal(JsonWriter,JsonSerializer,Object):Boolean
        private static Boolean WriteInternal(JsonWriter writer, JsonSerializer serializer, Object value) {
            if (value != null) {
                if (!WriteInternal(writer, serializer, value as IExceptionSerializable)) {
                    if (!WriteInternal(writer, serializer, value as IList<String>)) {
                        writer.WriteValue(value);
                        }
                    }
                return true;
                }
            return false;
            }
        #endregion
        #region M:WriteInternal(JsonWriter,JsonSerializer,IJsonSerializable):Boolean
        private static Boolean WriteInternal(JsonWriter writer, JsonSerializer serializer, IExceptionSerializable value) {
            if (value != null) {
                value.WriteTo(writer, serializer);
                return true;
                }
            return false;
            }
        #endregion
        #region M:WriteInternal(JsonWriter,JsonSerializer,IList<String>):Boolean
        private static Boolean WriteInternal(JsonWriter writer, JsonSerializer serializer, IList<String> value) {
            if (value != null) {
                writer.WriteStartArray();
                foreach (var i in value)
                    {
                    writer.WriteValue(i);
                    }
                writer.WriteEndArray();
                return true;
                }
            return false;
            }
        #endregion
        #region M:WriteBlock(JsonWriter,JsonSerializer,String[],String[],String[])
        internal static void WriteBlock(this JsonWriter writer, JsonSerializer serializer, String[] names, String[] values, String[] comments) {
            if ((names != null) && (names.Length > 0)) {
                if (values == null)   { throw new ArgumentNullException(nameof(values));   }
                if (comments == null) { throw new ArgumentNullException(nameof(comments)); }
                if (values.Length != names.Length)   { throw new ArgumentOutOfRangeException(nameof(values));   }
                if (comments.Length != names.Length) { throw new ArgumentOutOfRangeException(nameof(comments)); }
                var n = 0;
                for (var i = 0; i < names.Length; i++) {
                    if (values[i] != null) {
                        n = Math.Max(n, names[i].Length + values[i].Length);
                        }
                    }
                for (var i = 0; i < names.Length; i++) {
                    if (values[i] != null) {
                        WriteValue(writer, serializer, names[i], values[i]);
                        writer.WriteComment(comments[i]);
                        }
                    }
                }
            }
        #endregion

        internal static void WriteBase32PropertyValue(this JsonWriter writer, String propertyname, Byte[] value) {
            if ((value != null) && (value.Length > 0)) {
                writer.WritePropertyName(propertyname);
                if (value.Length <= 16) {
                    writer.WriteValue(String.Join(String.Empty, value.Select(i => i.ToString("X2"))));
                    return;
                    }
                var buffer = new Byte[16];
                var length = value.Length;
                var offset = 0;
                writer.WriteStartArray();
                while (length > 0) {
                    var count = Math.Min(length, 16);
                    Array.Copy(value, offset, buffer, 0, count);
                    var output = new StringBuilder();
                    for (var i = 0; i < count; i++)
                        {
                        output.Append($"{buffer[i]:X2}");
                        }
                    writer.WriteValue(output.ToString());
                    length -= 16;
                    offset += 16;
                    }
                writer.WriteEndArray();
                }
            }

        internal static IDisposable ArrayScope (this JsonWriter writer, JsonSerializer serializer) { return new ArrayScopeObject(writer);  }
        internal static IDisposable ObjectScope(this JsonWriter writer, JsonSerializer serializer) { return new ObjectScopeObject(writer); }
        internal static IDisposable SuspendFormatingScope(this JsonWriter writer) { return new SuspendFormatingScopeObject(writer); }

        #region T:ArrayScopeObject
        private class ArrayScopeObject: IDisposable
            {
            private readonly JsonWriter writer;
            public ArrayScopeObject(JsonWriter writer)
                {
                this.writer = writer;
                writer.WriteStartArray();
                }

            public void Dispose()
                {
                writer.WriteEnd();
                }
            }
        #endregion
        #region T:ObjectScopeObject
        private class ObjectScopeObject: IDisposable
            {
            private readonly JsonWriter writer;
            public ObjectScopeObject(JsonWriter writer)
                {
                this.writer = writer;
                writer.WriteStartObject();
                }

            public void Dispose()
                {
                writer.WriteEndObject();
                }
            }
        #endregion
        #region T:SuspendFormatingScopeObject
        private class SuspendFormatingScopeObject: IDisposable
            {
            private readonly JsonWriter writer;
            private readonly Formatting formatting;
            public SuspendFormatingScopeObject(JsonWriter writer)
                {
                this.writer = writer;
                this.formatting = writer.Formatting;
                writer.Formatting = Formatting.None;
                }

            public void Dispose()
                {
                writer.Formatting = formatting;
                }
            }
        #endregion

        #region M:ToString(Byte[],String):String
        private static String ToString(this Byte[] source, String format) {
            if (source == null) { return null; }
            var c = source.Length;
            switch (format)
                {
                case "X" : { return String.Join(String.Empty, source.Select(i => i.ToString("X2"))); }
                case "x" : { return String.Join(String.Empty, source.Select(i => i.ToString("x2"))); }
                case "FL":
                    {
                    return (c > 1)
                        ? $"{source[0]:X2}{source[c - 1]:X2}"
                        : String.Join(String.Empty, source.Select(i => i.ToString("X2")));
                    }
                }
            return source.ToString();
            }
        #endregion
        #region M:IsNullOrEmpty(ICollection):Boolean
        private static Boolean IsNullOrEmpty(ICollection value) {
            return (value == null) || (value.Count == 0);
            }
        #endregion
        }
    }