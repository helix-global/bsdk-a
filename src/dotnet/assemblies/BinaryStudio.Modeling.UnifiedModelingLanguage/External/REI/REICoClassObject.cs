using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid("97B38389-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    internal class REICoClassObject : REICOMObject
        {
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public extern RoseObjectClass();

        [DispId(12668)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String IdentifyClass();

        [DispId(12669)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Boolean IsClass([MarshalAs(UnmanagedType.BStr)] String theClassName);
        }
    }
