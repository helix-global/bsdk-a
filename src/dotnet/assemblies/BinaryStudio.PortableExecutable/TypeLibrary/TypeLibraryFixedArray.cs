using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;
    //[JsonConverter(typeof(Converter))]
    internal class TypeLibraryFixedArray : TypeLibraryTypeReference, ITypeLibraryFixedArrayTypeDescriptor
        {
        public override Boolean IsArray     { get { return true;  }}
        public override Boolean IsPrimitive { get { return false; }}
        public override Boolean IsPointer   { get { return false; }}
        [JsonIgnore] public override Guid? UniqueIdentifier { get { return null; }}
        [JsonIgnore] public override ITypeLibraryFixedArrayTypeDescriptor FixedArrayTypeDescriptor { get { return this; }}

        public TypeLibraryFixedArray(ITypeLibraryTypeDescriptor typeref, IList<TypeLibraryFixedArrayBound> bounds)
            : base(typeref)
            {
            Dimension = new TypeLibraryFixedArrayBoundCollection(bounds);
            }

        //private class Converter : TypeLibraryJsonConverter
        //    {
        //    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        //        {
        //        base.WriteJson(writer, value, serializer);
        //        }
        //    }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            if (UnderlyingType != null) {
                writer.WritePropertyName(nameof(UnderlyingType));
                WriteJsonValue(writer, UnderlyingType, serializer);
                }
            writer.WritePropertyName(nameof(IsArray));
            writer.WriteValue(IsArray);
            writer.WriteEndObject();
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            var r = new StringBuilder();
            r.Append(UnderlyingType.Name);
            r.Append(Dimension);
            return r.ToString();
            }

        #region P:LibraryQualifiedName:String
        public override String LibraryQualifiedName { get {
            var r = new StringBuilder();
            r.Append(UnderlyingType.Name);
            r.Append(Dimension);
            return r.ToString();
            }}
        #endregion

        [JsonIgnore] public ITypeLibraryTypeDescriptor ElementType { get { return UnderlyingType; }}
        public TypeLibraryFixedArrayBoundCollection Dimension { get; }

        private static unsafe TypeLibraryFixedArray FromSafeArray(SAFEARRAY32* source, ITypeLibraryTypeDescriptor typeref) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var bounds = (SAFEARRAYBOUND*)(source + 1);
            var r = new List<TypeLibraryFixedArrayBound>();
            for (var i = 0; i < source->cDims; ++i) {
                r.Add(new TypeLibraryFixedArrayBound(
                    bounds[i].Elements,
                    bounds[i].LowerBound));
                }
            return new TypeLibraryFixedArray(typeref, r);
            }

        private static unsafe TypeLibraryFixedArray FromSafeArray(SAFEARRAY64* source, ITypeLibraryTypeDescriptor typeref)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var bounds = (SAFEARRAYBOUND*)(source + 1);
            var r = new List<TypeLibraryFixedArrayBound>();
            for (var i = 0; i < source->cDims; ++i) {
                r.Add(new TypeLibraryFixedArrayBound(
                    bounds[i].Elements,
                    bounds[i].LowerBound));
                }
            return new TypeLibraryFixedArray(typeref, r);
            }

        public static unsafe TypeLibraryFixedArray FromSafeArray(SYSKIND kind, void* source, ITypeLibraryTypeDescriptor typeref) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (kind == SYSKIND.SYS_WIN64)
                ? FromSafeArray((SAFEARRAY64*)source, typeref)
                : FromSafeArray((SAFEARRAY32*)source, typeref);
            }

        [JsonIgnore] public override String Name { get {
            var r = new StringBuilder();
            r.Append(UnderlyingType.Name);
            r.Append(Dimension);
            return r.ToString();
            }}
        }
    }