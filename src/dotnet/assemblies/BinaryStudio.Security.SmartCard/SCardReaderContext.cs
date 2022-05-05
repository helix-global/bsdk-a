using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.Security.SmartCard
    {
    public sealed class SCardReaderContext : SCardObject
        {
        [DllImport("winscard.dll", CharSet = CharSet.Unicode, ExactSpelling = true)] private static extern HRESULT SCardConnectW(SCardContextHandle context, String reader, SCardShareMode sharemode, SCardProtocol preferredprotocols, out IntPtr handle, out SCardProtocol activeprotocol);
        [DllImport("winscard.dll")] private static extern HRESULT SCardDisconnect(IntPtr handle, SCardDisposition flags);
        [DllImport("winscard.dll")] private static extern HRESULT SCardTransmit(IntPtr handle, ref SCARD_IO_REQUEST sendpci, [MarshalAs(UnmanagedType.LPArray)] Byte[] sendbuffer, Int32 sendlength, ref SCARD_IO_REQUEST rcvpci, IntPtr rcvbuffer, ref Int32 rcvsize);
        [DllImport("winscard.dll")] private static extern unsafe HRESULT SCardTransmit(IntPtr handle, ref SCARD_IO_REQUEST sendpci, Byte* sendbuffer, Int32 sendlength, IntPtr rcvpci, Byte* rcvbuffer, ref Int32 rcvsize);
        
        private const Int32 SCARD_S_SUCCESS = 0;
        private const Int32 SCARD_E_INSUFFICIENT_BUFFER = unchecked((Int32)0x80100008);
        private IntPtr Context;

        public SCardReader Reader { get; }
        public SCardProtocol Protocol { get; }

        internal SCardReaderContext(SCardReader reader, SCardShareMode sharemode) {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            Reader = reader;
            Validate(SCardConnectW(reader.Context.Context, reader.Name, sharemode, SCardProtocol.T0|SCardProtocol.T1, out Context, out var r));
            Protocol = r;
            }

        public void Close(SCardDisposition flags) {
            if (Context != IntPtr.Zero) {
                SCardDisconnect(Context, flags);
                Context = IntPtr.Zero;
                }
            }

        #region M:Dispose(Boolean)
        protected override void Dispose(Boolean disposing) {
            Close(SCardDisposition.Leave);
            if (disposing)
                {
                }
            }
        #endregion

        private Boolean SCardTransmit(out HResultException e, ref SCARD_IO_REQUEST sndpci, Byte[] sndbuffer, ref SCARD_IO_REQUEST rcvpci, IntPtr rcvbuffer, ref Int32 rcvsize) {
            if (sndbuffer == null) { throw new ArgumentNullException(nameof(sndbuffer)); }
            if (rcvbuffer == null) { throw new ArgumentNullException(nameof(rcvbuffer)); }
            return Validate(out e, SCardTransmit(Context, ref sndpci, sndbuffer, sndbuffer.Length, ref rcvpci, rcvbuffer, ref rcvsize));
            }

        private unsafe Boolean Transmit(out Exception e, Byte[] sndbuffer, out Byte[] rcvbuffer)
            {
            e = null;
            rcvbuffer  = new Byte[0];
            var sndpci = new SCARD_IO_REQUEST(Protocol);
            var rcvpci = new SCARD_IO_REQUEST(Protocol);
            for (var isize = 0x800;;isize <<= 1) {
                var mprcv  = new LocalMemory(isize);
                var osize = isize;
                if (SCardTransmit(out var E, ref sndpci, sndbuffer, ref rcvpci, mprcv, ref osize)) {
                    rcvbuffer = new Byte[osize];
                    var uprcv = (Byte*)mprcv;
                    for (var i = 0; i < osize; i++) {
                        rcvbuffer[i] = uprcv[i];
                        }
                    return Validate(out e, rcvbuffer);
                    }
                if (E.ErrorCode != (Int32)HRESULT.SCARD_E_INSUFFICIENT_BUFFER)
                    {
                    e = E;
                    return false;
                    }
                }
            }

        public void Transmit(Byte[] inputbuffer, out Byte[] outputbuffer) {
            if (!Transmit(out var e, inputbuffer, out outputbuffer)) {
                throw e;
                }
            }

        internal static Byte FromHex(Char source)
            {
            if (((source >= '0') && (source <= '9')) ||
                ((source >= 'A') && (source <= 'F')) ||
                ((source >= 'a') && (source <= 'f')))
                {
                return ((source >= '0') && (source <= '9'))
                    ? (Byte)(source - '0')
                    : ((source >= 'a') && (source <= 'f'))
                        ? (Byte)(source - 'a' + 10)
                        : ((source >= 'A') && (source <= 'F'))
                            ? (Byte)(source - 'A' + 10)
                            : (Byte)0;
                }
            throw new ArgumentOutOfRangeException(nameof(source));
            }

        private static IDictionary<Char,Byte> DecodingTable = new Dictionary<Char, Byte>{
            { '0',  0 },
            { '1',  1 },
            { '2',  2 },
            { '3',  3 },
            { '4',  4 },
            { '5',  5 },
            { '6',  6 },
            { '7',  7 },
            { '8',  8 },
            { '9',  9 },
            { 'a', 10 },
            { 'b', 11 },
            { 'c', 12 },
            { 'd', 13 },
            { 'e', 14 },
            { 'f', 15 },
            { 'A', 10 },
            { 'B', 11 },
            { 'C', 12 },
            { 'D', 13 },
            { 'E', 14 },
            { 'F', 15 },
            };

        public void Transmit(String sequence, out Byte[] outputbuffer) {
            if (sequence == null) { throw new ArgumentNullException(nameof(sequence)); }
            sequence = sequence.Replace(" ",String.Empty);
            if ((sequence.Length % 2) != 0) { throw new ArgumentOutOfRangeException(nameof(sequence)); }
            var inputbuffer = new Byte[sequence.Length / 2];
            Int32 i,j;
            for (i = 0,j = 0; i < inputbuffer.Length; i++, j+=2)
                {
                inputbuffer[i] = (Byte)((DecodingTable[sequence[j]] << 4) | DecodingTable[sequence[j + 1]]);
                }
            if (!Transmit(out var e, inputbuffer, out outputbuffer)) {
                throw e;
                }
            }

        private static Boolean Validate(out Exception e, Byte[] r)
            {
            e = SCardInterchangeException.GetExceptionForCode((((Int32)r[0]) << 8) | (Int32)r[1]);
            if ((r[0] == 0x90) && (r[1] == 0))    { return true; }
            if ((r[0] >= 0x61) && (r[0] <= 0x63)) { return true; }
            return false;
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SCARD_IO_REQUEST
            {
            private readonly SCardProtocol Protocol;
            private readonly Int32 Length;

            private SCARD_IO_REQUEST(SCardProtocol protocol, Int32 length)
                {
                Protocol = protocol;
                Length = length;
                }

            public unsafe SCARD_IO_REQUEST(SCardProtocol protocol):
                this(protocol, sizeof(SCARD_IO_REQUEST))
                {
                }
            }
        }
    }