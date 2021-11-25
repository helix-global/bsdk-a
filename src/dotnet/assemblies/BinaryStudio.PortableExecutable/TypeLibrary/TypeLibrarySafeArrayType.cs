using System;
using System.Diagnostics;

namespace BinaryStudio.PortableExecutable
    {
    internal class TypeLibrarySafeArrayType : TypeLibraryTypeReference
        {
        public override Boolean IsArray     { get { return true;  }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsPrimitive { get { return false; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsPointer   { get { return false; }}
        public override String Name { get { return $"SAFEARRAY({UnderlyingType.Name})"; }}

        public TypeLibrarySafeArrayType(ITypeLibraryTypeDescriptor typeref)
            : base(typeref)
            {
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"SAFEARRAY({UnderlyingType.Name})";
            }
        }
    }