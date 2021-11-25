using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("PropID"), ClassInterface(ClassInterfaceType.None), Guid("9E7EA907-5810-4FCA-B817-CD0BBA8496FC"), TypeLibType(2)]
    [ComImport]
    public class ExtendedPropertyClass : IExtendedProperty
        {
        [DispId(0)]
        public virtual extern CAPICOM_PROPID PropID
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(1)]
        public virtual extern String Value
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }
        }
    }
