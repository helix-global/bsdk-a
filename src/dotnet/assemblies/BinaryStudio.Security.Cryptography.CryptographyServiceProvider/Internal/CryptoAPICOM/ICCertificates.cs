using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("EBDC6DC2-684D-4425-BBB7-CB4D15A088A7"), InterfaceType(1)]
    [TypeLibType(TypeLibTypeFlags.FRestricted)]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public interface ICCertificates
        {
        [TypeLibFunc(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void _ExportToStore([In] IntPtr hCertStore);
        }
    }
