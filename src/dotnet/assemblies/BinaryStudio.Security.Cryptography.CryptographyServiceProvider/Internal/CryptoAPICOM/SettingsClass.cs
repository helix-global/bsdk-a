using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("A996E48C-D3DC-4244-89F7-AFA33EC60679"), TypeLibType(2)]
    [ComImport]
    public class SettingsClass : ISettings
        {
        [DispId(1)]
        public virtual extern Boolean EnablePromptForCertificateUI
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(2)]
        public virtual extern CAPICOM_ACTIVE_DIRECTORY_SEARCH_LOCATION ActiveDirectorySearchLocation
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }
        }
    }
