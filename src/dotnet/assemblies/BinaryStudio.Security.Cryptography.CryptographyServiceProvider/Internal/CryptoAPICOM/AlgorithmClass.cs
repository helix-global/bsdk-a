using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Name"), Guid("A1EEF42F-5026-4A32-BC5C-2E552B70FD96")]
    [ComImport]
    public class AlgorithmClass : IAlgorithm
        {
        [DispId(0)]
        public virtual extern CAPICOM_ENCRYPTION_ALGORITHM Name
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(1)]
        public virtual extern CAPICOM_ENCRYPTION_KEY_LENGTH KeyLength
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }
        }
    }
