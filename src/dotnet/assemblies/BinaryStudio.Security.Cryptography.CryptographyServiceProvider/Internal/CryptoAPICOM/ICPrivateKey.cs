using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ComConversionLoss, Guid("50F241B7-A8F2-4E0A-B982-4BD7DF0CCF3C"), InterfaceType(1)]
    [TypeLibType(TypeLibTypeFlags.FRestricted)]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public interface ICPrivateKey
        {
        [TypeLibFunc(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        IntPtr _GetKeyProvInfo();

        [TypeLibFunc(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        IntPtr _GetKeyContext();
        }
    }
