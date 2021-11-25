using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("9E7D3477-4F63-423E-8A45-E13B2BB851A2"), InterfaceType(1)]
    [ComImport]
    public interface ICertContext
        {
        [DispId(1610678272)]
        Int32 CertContext
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [MethodImpl(MethodImplOptions.InternalCall)]
        void FreeContext([In] Int32 pCertContext);
        }
    }
