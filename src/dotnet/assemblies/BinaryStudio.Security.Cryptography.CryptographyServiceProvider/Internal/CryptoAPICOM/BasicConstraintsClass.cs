using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("C05AAC6E-3A58-45A9-A203-56952E961E48")]
    [ComImport]
    public class BasicConstraintsClass : IBasicConstraints
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
        public virtual extern Boolean IsCertificateAuthority
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(4)]
        public virtual extern Boolean IsPathLenConstraintPresent
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(5)]
        public virtual extern Int32 PathLenConstraint
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }
        }
    }
