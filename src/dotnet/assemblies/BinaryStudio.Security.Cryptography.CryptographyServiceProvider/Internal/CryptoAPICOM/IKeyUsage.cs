using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("41DD35A8-9FF9-45A6-9A7C-F65B2F085D1F")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IKeyUsage
        {
        [DispId(1)]
        Boolean IsPresent
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        Boolean IsCritical
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(3)]
        Boolean IsDigitalSignatureEnabled
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(4)]
        Boolean IsNonRepudiationEnabled
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(5)]
        Boolean IsKeyEnciphermentEnabled
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(6)]
        Boolean IsDataEnciphermentEnabled
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(7)]
        Boolean IsKeyAgreementEnabled
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(8)]
        Boolean IsKeyCertSignEnabled
            {
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(9)]
        Boolean IsCRLSignEnabled
            {
            [DispId(9)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(10)]
        Boolean IsEncipherOnlyEnabled
            {
            [DispId(10)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(11)]
        Boolean IsDecipherOnlyEnabled
            {
            [DispId(11)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }
        }
    }
