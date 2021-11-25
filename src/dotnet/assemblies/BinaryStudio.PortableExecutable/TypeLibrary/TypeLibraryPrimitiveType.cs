using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    //[JsonConverter(typeof(Converter))]
    internal class TypeLibraryPrimitiveType : TypeLibraryTypeDescriptor
        {
        public override String Name { get; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override String LibraryQualifiedName { get { return Name; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override ITypeLibraryTypeDescriptor UnderlyingType { get { return null; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override ITypeLibraryTypeDescriptor BaseType { get { return null; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override ITypeLibraryDescriptor Library { get { return null; }}
        public override TypeLibTypeFlags Flags { get; }
        public override Version Version { get; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsPrimitive { get { return true;  }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsAlias     { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsPointer   { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsArray     { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsEnum      { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsClass     { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsUnion     { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsInterface { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsDispatch  { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsModule    { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsStructure { get { return false; }}
        public override Guid? UniqueIdentifier { get; }

        //private class Converter : TypeLibraryJsonConverter {
        //    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer) {
        //        var source = value as TypeLibraryPrimitiveType;
        //        if (source != null) {
        //            writer.WriteValue(source.Name);
        //            }
        //        }
        //    }

        public TypeLibraryPrimitiveType(Type source)
            :base(null)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Name = source.Name;
            UniqueIdentifier = source.GUID;
            }

        public TypeLibraryPrimitiveType(Type source, String name)
            :base(null)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Name = name;
            UniqueIdentifier = source.GUID;
            }

        public TypeLibraryPrimitiveType(Guid g, String name)
            :base(null)
            {
            Name = name;
            UniqueIdentifier = g;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return Name;
            }
        }
    }