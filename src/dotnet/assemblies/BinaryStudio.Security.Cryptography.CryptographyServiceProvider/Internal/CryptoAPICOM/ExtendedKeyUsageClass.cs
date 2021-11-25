using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("42C18607-1B4B-4126-8F1B-76E2DC7F631A")]
    [ComImport]
    public class ExtendedKeyUsageClass : IExtendedKeyUsage
        {
        [DispId(1)]
        public virtual extern Boolean IsPresent
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        public virtual extern Boolean IsCritical
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(3)]
        public virtual extern IEKUs EKUs
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }
        }
    }
