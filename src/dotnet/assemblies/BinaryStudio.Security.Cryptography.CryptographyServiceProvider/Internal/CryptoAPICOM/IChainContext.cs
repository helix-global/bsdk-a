using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("B27FFB30-432E-4585-A3FD-72530108CBFD"), InterfaceType(1)]
    [ComImport]
    public interface IChainContext
        {
        [DispId(1610678272)]
        Int32 ChainContext
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [MethodImpl(MethodImplOptions.InternalCall)]
        void FreeContext([In] Int32 pChainContext);
        }
    }
