using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("OID"), ClassInterface(ClassInterfaceType.None), Guid("D2359E2C-82D6-458F-BB6F-41559155E693")]
    [ComImport]
    public class ExtensionClass : IExtension
        {
        [DispId(0)]
        public virtual extern IOID OID
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(1)]
        public virtual extern Boolean IsCritical
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        public virtual extern IEncodedData EncodedData
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }
        }
    }
