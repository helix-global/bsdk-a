﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryStudio.IO;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1ObjectIdentifier : Asn1UniversalObject, IEquatable<String>
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.ObjectIdentifier; }}
        public Int64[] Sequence { get;private set; }

        public String FriendlyName { get {
            var value = ToString();
            var r = OID.ResourceManager.GetString(value, PlatformSettings.DefaultCulture);
            return (!String.IsNullOrWhiteSpace(r))
                    ? r
                    : value;
            }}

        public Asn1ObjectIdentifier(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public Asn1ObjectIdentifier(String source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            #if NET35
            if (String.IsNullOrEmpty(source)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            #else
            if (String.IsNullOrWhiteSpace(source)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            #endif
            State |= ObjectState.Decoded;
            Sequence = source.Split('.').Select(Int64.Parse).ToArray();
            }

        #region M:CreateSequence(Byte[]):Int64[]
        private static Int64[] CreateSequence(Byte[] source) {
            var j = 0L;
            var values = new List<Int64>();
            for (var i = 0; i < source.LongLength; ++i) {
                var c = source[i];
                if (i == 0) {
                    if (c < 40) {
                        values.Add(0);
                        values.Add(c);
                        }
                    else if (c < 80)
                        {
                        values.Add(1);
                        values.Add(c - 40);
                        }
                    else
                        {
                        values.Add(2);
                        values.Add(c - 80);
                        }
                    }
                else
                    {
                    if (c < 128) {
                        if (j == 0) { values.Add(c); }
                        else
                            {
                            values.Add(c + j);
                            j  = 0;
                            }
                        }
                    else
                        {
                        var K = c & 0x7F;
                        j = (j == 0)
                            ? (K) * 128
                            : (K + j) * 128;
                        }
                    }
                }
            return values.ToArray();
            }
        #endregion
        #region M:ToString:String
        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            #if NET35
            return IsDecoded
                ? String.Join(".", Sequence.Select(i => i.ToString()).ToArray())
                : base.ToString();
            #else
            return IsDecoded
                ? String.Join(".", Sequence)
                : base.ToString();
            #endif
            }
        #endregion

        /**
         * <summary>Indicates whether the current object is equal to another string object.</summary>
         * <param name="key">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="key"/> parameter; otherwise, false.</returns>
         * */
        public Boolean Equals(String key) {
            return (Equals(ToString(), key));
            }

        /**
         * <summary>Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.</summary>
         * <returns>true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.</returns>
         * <param name="other">The object to compare with the current object.</param>
         * <filterpriority>2</filterpriority>
         * */
        public override Boolean Equals(Object other)
            {
            if (ReferenceEquals(this, other)) { return true; }
            if (other is String) { return Equals((String)other); }
            return base.Equals(other);
            }

        /**
         * <summary>Serves as a hash function for a particular type.</summary>
         * <returns>A hash code for the current <see cref="Object"/>.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Int32 GetHashCode()
            {
            return ToString().GetHashCode();
            }

        protected internal override Boolean Decode()
            {
            if (IsDecoded) { return true; }
            Content.Seek(0, SeekOrigin.Begin);
            var r = new Byte[Length];
            Content.Read(r, 0, r.Length);
            Sequence = CreateSequence(r);
            State |= ObjectState.Decoded;
            return true;
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartObject();
            WriteValue(writer, serializer, nameof(Class), Class.ToString());
            WriteValue(writer, serializer, nameof(Type),  Type.ToString());
            if (Offset >= 0) { WriteValue(writer, serializer, nameof(Offset), Offset); }
            WriteValue(writer, serializer, "Value", ToString());
            writer.WriteEndObject();
            }
        }
    }