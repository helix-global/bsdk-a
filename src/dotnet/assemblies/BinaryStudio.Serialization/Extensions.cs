using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BinaryStudio.IO;
using Newtonsoft.Json;

namespace BinaryStudio.Serialization
    {
    public static class Extensions
        {
        #region M:WriteMultilineHexComment(JsonWriter,Byte[])
        public static void WriteMultilineHexComment(this JsonWriter writer,Byte[] value) {
            if (value != null) {
                //writer.WriteIndent();
                //writer.WriteRaw("/**");
                //foreach (var i in Base32Formatter.Format(value, value.Length, 0, Base32FormattingFlags.Offset)) {
                //    writer.WriteIndent();
                //    writer.WriteRaw($"/* {i}");
                //    }
                //writer.WriteIndent();
                //writer.WriteRaw(" */");
                }
            }
        #endregion
        #region M:WriteMultilineHexComment(JsonWriter,Stream)
        public static void WriteMultilineHexComment(this JsonWriter writer,Stream stream) {
            //if (stream != null) {
            //    writer.WriteIndent();
            //    writer.WriteRaw("/**");
            //    foreach (var i in Base32Formatter.Format(stream, Base32FormattingFlags.Offset)) {
            //        writer.WriteIndent();
            //        writer.WriteRaw($"/* {i}");
            //        }
            //    writer.WriteIndent();
            //    writer.WriteRaw(" */");
            //    }
            }
        #endregion
        #region M:WriteValue(JsonWriter,JsonSerializer,String,Object)
        public static void WriteValue(this JsonWriter writer, JsonSerializer serializer, String name, Object value) {
            if (value != null) {
                writer.WritePropertyName(name);
                if (!WriteInternal(writer, serializer, value as IJsonSerializable)) {
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
                        writer.WriteValue(value);
                        }
                    }
                }
            }
        #endregion
        #region M:WriteValue(JsonWriter,JsonSerializer,String,String)
        public static void WriteValue(this JsonWriter writer, JsonSerializer serializer, String name, String value) {
            if (value != null) {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
                }
            }
        #endregion
        #region M:WriteValue(JsonWriter,JsonSerializer,String,String,Int32)
        public static void WriteValue(this JsonWriter writer, JsonSerializer serializer, String name, String value, Int32 spaces) {
            if (value != null) {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
                //writer.WriteIndentSpace(spaces);
                }
            }
        #endregion
        #region M:WriteValue(JsonWriter,JsonSerializer,String,Boolean,Int32)
        public static void WriteValue(this JsonWriter writer, JsonSerializer serializer, String name, Boolean value, Int32 spaces) {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
            //writer.WriteIndentSpace(spaces);
            }
        #endregion
        #region M:WriteValue<T>(JsonWriter,JsonSerializer,String,IEnumerable<T>)
        public static void WriteValue<T>(this JsonWriter writer, JsonSerializer serializer, String name, IEnumerable<T> values) {
            if (values != null) {
                if (values is IJsonSerializable J)
                    {
                    writer.WritePropertyName(name);
                    J.WriteJson(writer, serializer);
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
                if (!WriteInternal(writer, serializer, value as IJsonSerializable)) {
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
        private static Boolean WriteInternal(JsonWriter writer, JsonSerializer serializer, IJsonSerializable value) {
            if (value != null) {
                value.WriteJson(writer, serializer);
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
        public static void WriteBlock(this JsonWriter writer, JsonSerializer serializer, String[] names, String[] values, String[] comments) {
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
                        //writer.WriteIndentSpace(n - (names[i].Length + values[i].Length) + 1);
                        writer.WriteComment(comments[i]);
                        }
                    }
                }
            }
        #endregion

        public static void WriteIndentSpace(this JsonWriter writer, Int32 spaces) {
            var fi = writer.GetType().GetField("_writer", BindingFlags.Instance|BindingFlags.NonPublic);
            if (fi != null) {
                var nestedwriter = (TextWriter)fi.GetValue(writer);
                if (nestedwriter != null) {
                    nestedwriter.Write(new String(' ', spaces));
                    nestedwriter.Flush();
                    }
                }
            }

        public static void WriteBase32PropertyValue(this JsonWriter writer, String propertyname, Byte[] value) {
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

        public static IDisposable ArrayScope (this JsonWriter writer, JsonSerializer serializer) { return new ArrayScopeObject(writer);  }
        public static IDisposable ObjectScope(this JsonWriter writer, JsonSerializer serializer) { return new ObjectScopeObject(writer); }
        public static IDisposable SuspendFormatingScope(this JsonWriter writer) { return new SuspendFormatingScopeObject(writer); }

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
        public static String ToString(this Byte[] source, String format) {
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