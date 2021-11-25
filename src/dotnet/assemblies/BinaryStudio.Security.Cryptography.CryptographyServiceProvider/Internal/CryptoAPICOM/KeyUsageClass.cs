using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("9226C95C-38BE-4CC4-B3A2-A867F5199C13")]
    [ComImport]
    public class KeyUsageClass : IKeyUsage
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
        public virtual extern Boolean IsDigitalSignatureEnabled
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(4)]
        public virtual extern Boolean IsNonRepudiationEnabled
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(5)]
        public virtual extern Boolean IsKeyEnciphermentEnabled
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(6)]
        public virtual extern Boolean IsDataEnciphermentEnabled
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(7)]
        public virtual extern Boolean IsKeyAgreementEnabled
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(8)]
        public virtual extern Boolean IsKeyCertSignEnabled
            {
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(9)]
        public virtual extern Boolean IsCRLSignEnabled
            {
            [DispId(9)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(10)]
        public virtual extern Boolean IsEncipherOnlyEnabled
            {
            [DispId(10)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(11)]
        public virtual extern Boolean IsDecipherOnlyEnabled
            {
            [DispId(11)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }
        }
    }
