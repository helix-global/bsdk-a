using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("8F83F792-014C-4E22-BD57-5C381E622F34"), InterfaceType(1)]
    [TypeLibType(TypeLibTypeFlags.FRestricted)]
    [ComImport]
    public interface ICSigner
        {
        [DispId(1610678272)]
        Int32 AdditionalStore
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }
        }
    }
