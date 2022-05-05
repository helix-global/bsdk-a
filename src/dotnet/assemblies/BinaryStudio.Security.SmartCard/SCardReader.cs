using System;
using System.Runtime.InteropServices;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.Security.SmartCard
    {
    public class SCardReader : SCardObject
        {
        internal readonly SCardContext Context;

        public String Name { get; }
        public String DeviceInstanceId { get; }

        internal SCardReader(SCardContext context, String name) {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            Name = name;
            Context = context;
            DeviceInstanceId = SCardGetReaderDeviceInstanceId(context.Context, name);
            }

        public override String ToString()
            {
            return Name;
            }

        public SCardReaderContext Open(SCardShareMode sharemode) { return new SCardReaderContext(this, sharemode); }
        [DllImport("winscard.dll",CharSet = CharSet.Unicode, ExactSpelling = true)] private static extern HRESULT SCardGetReaderDeviceInstanceIdW(SCardContextHandle handle, [MarshalAs(UnmanagedType.LPWStr)] String readername, Char[] buffer, ref Int32 bufferLength);

        private static String SCardGetReaderDeviceInstanceId(SCardContextHandle handle, String readername)
            {
            var c = 0;
            Char[] r;
            HRESULT hr;
            hr = SCardGetReaderDeviceInstanceIdW(handle, readername, null, ref c);
            hr = SCardGetReaderDeviceInstanceIdW(handle, readername, r = new Char[c], ref c);
            return new String(r);
            }
        }
    }