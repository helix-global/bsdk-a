using System;
using System.Linq;
using System.Numerics;
using BinaryStudio.IO;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="INTEGER"/> type.
    /// </summary>
    public sealed class Asn1Integer : Asn1UniversalObject, IConvertible
        {
        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.Integer"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.Integer; }}
        public BigInteger Value { get;private set; }

        internal Asn1Integer(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        #region M:IConvertible.GetTypeCode:TypeCode
        /// <summary>Returns the <see cref="T:System.TypeCode" /> for this instance.</summary>
        /// <returns>The enumerated constant that is the <see cref="T:System.TypeCode" /> of the class or value type that implements this interface.</returns>
        /// <filterpriority>2</filterpriority>
        TypeCode IConvertible.GetTypeCode()
            {
            return TypeCode.Object;
            }
        #endregion
        #region M:IConvertible.ToBoolean(IFormatProvider):Boolean
        /// <summary>Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.</summary>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Boolean IConvertible.ToBoolean(IFormatProvider provider)
            {
            return ToUInt64(provider) != 0;
            }
        #endregion
        #region M:IConvertible.ToChar(IFormatProvider):Char
        /// <summary>Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.</summary>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Char IConvertible.ToChar(IFormatProvider provider)
            {
            return ((IConvertible)(UInt16)Value).ToChar(provider);
            }
        #endregion
        #region M:IConvertible.ToSByte(IFormatProvider):SByte
        /// <summary>Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        SByte IConvertible.ToSByte(IFormatProvider provider)
            {
            return (SByte)Value;
            }
        #endregion
        #region M:IConvertible.ToByte(IFormatProvider):Byte
        /// <summary>Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Byte IConvertible.ToByte(IFormatProvider provider)
            {
            return (Byte)Value;
            }
        #endregion
        #region M:IConvertible.ToInt16(IFormatProvider):Int16
        /// <summary>Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Int16 IConvertible.ToInt16(IFormatProvider provider)
            {
            return (Int16)Value;
            }
        #endregion
        #region M:IConvertible.ToUInt16(IFormatProvider):UInt16
        /// <summary>Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        UInt16 IConvertible.ToUInt16(IFormatProvider provider)
            {
            return (UInt16)Value;
            }
        #endregion
        #region M:IConvertible.ToInt32(IFormatProvider):Int32
        /// <summary>Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Int32 IConvertible.ToInt32(IFormatProvider provider)
            {
            return (Int32)Value;
            }
        public Int32 ToInt32()
            {
            return (Int32)Value;
            }
        #endregion
        #region M:IConvertible.ToUInt32(IFormatProvider):UInt32
        /// <summary>Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        public UInt32 ToUInt32(IFormatProvider provider)
            {
            return (UInt32)Value;
            }
        #endregion
        #region M:IConvertible.ToInt64(IFormatProvider)
        /// <summary>Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Int64 IConvertible.ToInt64(IFormatProvider provider)
            {
            return (Int64)Value;
            }
        #endregion
        #region M:IConvertible.ToUInt64(IFormatProvider):UInt64
        /// <summary>Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        public UInt64 ToUInt64(IFormatProvider provider) {
            return (UInt64)Value;
            }
        #endregion
        #region M:IConvertible.ToSingle(IFormatProvider):Single
        /// <summary>Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.</summary>
        /// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Single IConvertible.ToSingle(IFormatProvider provider)
            {
            return (Single)Value;
            }
        #endregion
        #region M:IConvertible.ToDouble(IFormatProvider):Double
        /// <summary>Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.</summary>
        /// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Double IConvertible.ToDouble(IFormatProvider provider)
            {
            return (Double)Value;
            }
        #endregion
        #region M:IConvertible.ToDecimal(IFormatProvider):Decimal
        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.Decimal" /> number using the specified culture-specific formatting information.</summary>
        /// <returns>A <see cref="T:System.Decimal" /> number equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Decimal IConvertible.ToDecimal(IFormatProvider provider)
            {
            return (Decimal)Value;
            }
        #endregion
        #region M:IConvertible.ToDateTime(IFormatProvider):DateTime
        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.DateTime" /> using the specified culture-specific formatting information.</summary>
        /// <returns>A <see cref="T:System.DateTime" /> instance equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
            {
            return new DateTime((Int64)Value);
            }
        #endregion
        #region M:IConvertible.ToString(IFormatProvider):String
        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.String" /> using the specified culture-specific formatting information.</summary>
        /// <returns>A <see cref="T:System.String" /> instance equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        String IConvertible.ToString(IFormatProvider provider)
            {
            return ToString();
            }
        #endregion
        #region M:IConvertible.ToType(Type,IFormatProvider):Object
        /// <summary>Converts the value of this instance to an <see cref="T:System.Object" /> of the specified <see cref="T:System.Type" /> that has an equivalent value, using the specified culture-specific formatting information.</summary>
        /// <returns>An <see cref="T:System.Object" /> instance of type <paramref name="type" /> whose value is equivalent to the value of this instance.</returns>
        /// <param name="type">The <see cref="T:System.Type" /> to which the value of this instance is converted. </param>
        /// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information. </param>
        /// <filterpriority>2</filterpriority>
        Object IConvertible.ToType(Type type, IFormatProvider provider)
            {
             return DefaultToType(this, type, provider);
            }
        #endregion

        internal static Object DefaultToType(IConvertible value, Type type, IFormatProvider provider)
            {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (value.GetType() == type) { return value; }
            if (type == typeof(Boolean))  { return value.ToBoolean(provider);  }
            if (type == typeof(Char))     { return value.ToChar(provider);     }
            if (type == typeof(SByte))    { return value.ToSByte(provider);    }
            if (type == typeof(Byte))     { return value.ToByte(provider);     }
            if (type == typeof(Int16))    { return value.ToInt16(provider);    }
            if (type == typeof(UInt16))   { return value.ToUInt16(provider);   }
            if (type == typeof(Int32))    { return value.ToInt32(provider);    }
            if (type == typeof(UInt32))   { return value.ToUInt32(provider);   }
            if (type == typeof(Int64))    { return value.ToInt64(provider);    }
            if (type == typeof(UInt64))   { return value.ToUInt64(provider);   }
            if (type == typeof(Single))   { return value.ToSingle(provider);   }
            if (type == typeof(Double))   { return value.ToDouble(provider);   }
            if (type == typeof(Decimal))  { return value.ToDecimal(provider);  }
            if (type == typeof(DateTime)) { return value.ToDateTime(provider); }
            if (type == typeof(String))   { return value.ToString(provider);   }
            if (type == typeof(Object))   { return value;                      }
            if (type == typeof(Enum))     { return (Enum)value;                }
            throw new InvalidCastException();
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return Value.ToString();
            }

        protected internal override Boolean Decode()
            {
            if (IsDecoded) { return true; }
            if (IsIndefiniteLength) { return false; }
            var r = new Byte[Length];
            Content.Read(r, 0, r.Length);
            Value = new BigInteger(r.Reverse().ToArray());
            State |= ObjectState.Decoded;
            return true;
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer)
            {
            base.WriteJsonOverride(writer, serializer);
            WriteValue(writer, serializer, "Value", Value.ToString());
            }

        ///// <summary>Converts an object into its XML representation.</summary>
        ///// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        //public override void WriteXml(XmlWriter writer)
        //    {
        //    writer.WriteStartElement("Integer");
        //    if (Offset >= 0) { writer.WriteAttributeString(nameof(Offset), Offset.ToString()); }
        //    writer.WriteEndElement();
        //    }

        public static implicit operator Int32(Asn1Integer source) { return (Int32)source.Value; }
        public static implicit operator BigInteger(Asn1Integer source) { return source.Value; }
        }
    }
