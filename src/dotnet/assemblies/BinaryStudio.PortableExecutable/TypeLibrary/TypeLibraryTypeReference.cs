using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    internal class TypeLibraryTypeReference : TypeLibraryTypeDescriptor, ITypeLibraryTypeReferenceSerializer
        {
        public override String Name { get { return (UnderlyingType != null) ? UnderlyingType.Name : null; }}
        public override String LibraryQualifiedName { get { return (UnderlyingType != null) ? UnderlyingType.LibraryQualifiedName : null; }}
        public override ITypeLibraryTypeDescriptor UnderlyingType { get; }
        public override ITypeLibraryDescriptor Library { get { return (UnderlyingType != null) ? UnderlyingType.Library : null; }}
        public override ITypeLibraryTypeDescriptor BaseType { get { return (UnderlyingType != null) ? UnderlyingType.BaseType : null; }}
        public override Boolean IsAlias     { get { return (UnderlyingType != null) && UnderlyingType.IsAlias;     }}
        public override Boolean IsPrimitive { get { return (UnderlyingType != null) && UnderlyingType.IsPrimitive; }}
        public override Boolean IsPointer   { get { return (UnderlyingType != null) && UnderlyingType.IsPointer;   }}
        public override Boolean IsArray     { get { return (UnderlyingType != null) && UnderlyingType.IsArray;     }}
        public override Boolean IsEnum      { get { return (UnderlyingType != null) && UnderlyingType.IsEnum;      }}
        public override Boolean IsClass     { get { return (UnderlyingType != null) && UnderlyingType.IsClass;     }}
        public override Boolean IsInterface { get { return (UnderlyingType != null) && UnderlyingType.IsInterface; }}
        public override Boolean IsModule    { get { return (UnderlyingType != null) && UnderlyingType.IsModule;    }}
        public override Boolean IsUnion     { get { return (UnderlyingType != null) && UnderlyingType.IsUnion;     }}
        public override Boolean IsStructure { get { return (UnderlyingType != null) && UnderlyingType.IsStructure; }}
        public override Boolean IsDispatch  { get { return (UnderlyingType != null) && UnderlyingType.IsDispatch;  }}
        public override Version Version     { get { return (UnderlyingType != null) ? UnderlyingType.Version : null; }}
        public override TypeLibTypeFlags Flags     { get { return (UnderlyingType != null) ? UnderlyingType.Flags : default(TypeLibTypeFlags); }}
        public override Guid? UniqueIdentifier { get { return (UnderlyingType != null) ? UnderlyingType.UniqueIdentifier : null; }}

        public TypeLibraryTypeReference(ITypeLibraryTypeDescriptor typeref)
            :base(null)
            {
            UnderlyingType = typeref;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"{Name}&";
            }

        public virtual void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteValue(LibraryQualifiedName);
            }
        }
    }