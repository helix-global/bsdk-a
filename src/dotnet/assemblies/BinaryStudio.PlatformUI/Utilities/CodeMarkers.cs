using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI
    {
    internal sealed class CodeMarkers
        {
        public static readonly CodeMarkers Instance = new CodeMarkers();
        private static readonly Byte[] CorrelationMarkBytes = new Guid("AA10EEA0-F6AD-4E21-8865-C427DAE8EDB9").ToByteArray();
        private const String AtomName = "VSCodeMarkersEnabled";
        private const String TestDllName = "Microsoft.Internal.Performance.CodeMarkers.dll";
        private const String ProductDllName = "Microsoft.VisualStudio.CodeMarkers.dll";
        private State state;
        private RegistryView registryView;
        private String regroot;
        private Boolean? shouldUseTestDll;

        public Boolean IsEnabled
            {
            get
                {
                return state == State.Enabled;
                }
            }

        public Boolean ShouldUseTestDll
            {
            get
                {
                if (!shouldUseTestDll.HasValue)
                    {
                    try
                        {
                        shouldUseTestDll = regroot != null ? new Boolean?(UsePrivateCodeMarkers(regroot, registryView)) : new Boolean?(NativeMethods.GetModuleHandle("Microsoft.VisualStudio.CodeMarkers.dll") == IntPtr.Zero);
                        }
                    catch
                        {
                        shouldUseTestDll = new Boolean?(true);
                        }
                    }
                return shouldUseTestDll.Value;
                }
            }

        private CodeMarkers()
            {
            state = (Int32)NativeMethods.FindAtom("VSCodeMarkersEnabled") != 0 ? State.Enabled : State.Disabled;
            }

        public Boolean CodeMarker(Int32 nTimerID)
            {
            if (!IsEnabled)
                return false;
            try
                {
                if (ShouldUseTestDll)
                    NativeMethods.TestDllPerfCodeMarker(nTimerID, null, 0);
                else
                    NativeMethods.ProductDllPerfCodeMarker(nTimerID, null, 0);
                }
            catch (DllNotFoundException)
                {
                state = State.DisabledDueToDllImportException;
                return false;
                }
            return true;
            }

        public Boolean CodeMarkerEx(Int32 nTimerID, Byte[] aBuff)
            {
            if (!IsEnabled)
                return false;
            if (aBuff == null)
                throw new ArgumentNullException(nameof(aBuff));
            try
                {
                if (ShouldUseTestDll)
                    NativeMethods.TestDllPerfCodeMarker(nTimerID, aBuff, aBuff.Length);
                else
                    NativeMethods.ProductDllPerfCodeMarker(nTimerID, aBuff, aBuff.Length);
                }
            catch (DllNotFoundException)
                {
                state = State.DisabledDueToDllImportException;
                return false;
                }
            return true;
            }

        public void SetStateDLLException()
            {
            state = State.DisabledDueToDllImportException;
            }

        public Boolean CodeMarkerEx(Int32 nTimerID, Guid guidData)
            {
            return CodeMarkerEx(nTimerID, guidData.ToByteArray());
            }

        public Boolean CodeMarkerEx(Int32 nTimerID, String stringData)
            {
            if (!IsEnabled)
                return false;
            if (stringData == null)
                throw new ArgumentNullException(nameof(stringData));
            try
                {
                var cbParams = Encoding.Unicode.GetByteCount(stringData) + 2;
                if (ShouldUseTestDll)
                    NativeMethods.TestDllPerfCodeMarkerString(nTimerID, stringData, cbParams);
                else
                    NativeMethods.ProductDllPerfCodeMarkerString(nTimerID, stringData, cbParams);
                }
            catch (DllNotFoundException)
                {
                state = State.DisabledDueToDllImportException;
                return false;
                }
            return true;
            }

        internal static Byte[] StringToBytesZeroTerminated(String stringData)
            {
            var unicode = Encoding.Unicode;
            var bytes = new Byte[unicode.GetByteCount(stringData) + 2];
            unicode.GetBytes(stringData, 0, stringData.Length, bytes, 0);
            return bytes;
            }

        public static Byte[] AttachCorrelationId(Byte[] buffer, Guid correlationId)
            {
            if (correlationId == Guid.Empty)
                return buffer;
            var byteArray = correlationId.ToByteArray();
            var numArray = new Byte[CorrelationMarkBytes.Length + byteArray.Length + (buffer != null ? buffer.Length : 0)];
            CorrelationMarkBytes.CopyTo(numArray, 0);
            byteArray.CopyTo(numArray, CorrelationMarkBytes.Length);
            if (buffer != null)
                buffer.CopyTo(numArray, CorrelationMarkBytes.Length + byteArray.Length);
            return numArray;
            }

        public Boolean CodeMarkerEx(Int32 nTimerID, UInt32 uintData)
            {
            return CodeMarkerEx(nTimerID, BitConverter.GetBytes(uintData));
            }

        public Boolean CodeMarkerEx(Int32 nTimerID, UInt64 ulongData)
            {
            return CodeMarkerEx(nTimerID, BitConverter.GetBytes(ulongData));
            }

        private static Boolean UsePrivateCodeMarkers(String regRoot, RegistryView registryView)
            {
            if (regRoot == null)
                throw new ArgumentNullException(nameof(regRoot));
            using (var registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                using (var registryKey2 = registryKey1.OpenSubKey(regRoot + "\\Performance"))
                    {
                    if (registryKey2 != null)
                        return !String.IsNullOrEmpty(registryKey2.GetValue(String.Empty).ToString());
                    }
                }
            return false;
            }

        private static class NativeMethods
            {
            [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
            public static extern void TestDllPerfCodeMarker(Int32 nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] Byte[] aUserParams, Int32 cbParams);

            [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
            public static extern void TestDllPerfCodeMarkerString(Int32 nTimerID, [MarshalAs(UnmanagedType.LPWStr)] String aUserParams, Int32 cbParams);

            [DllImport("Microsoft.VisualStudio.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
            public static extern void ProductDllPerfCodeMarker(Int32 nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] Byte[] aUserParams, Int32 cbParams);

            [DllImport("Microsoft.VisualStudio.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
            public static extern void ProductDllPerfCodeMarkerString(Int32 nTimerID, [MarshalAs(UnmanagedType.LPWStr)] String aUserParams, Int32 cbParams);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern UInt16 FindAtom([MarshalAs(UnmanagedType.LPWStr)] String lpString);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] String lpModuleName);
            }

        private enum State
            {
            Enabled,
            Disabled,
            DisabledDueToDllImportException,
            }
        }
    }