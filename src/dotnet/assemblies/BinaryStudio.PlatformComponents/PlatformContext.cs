using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.PlatformComponents
    {
    public class PlatformContext
        {
        private static CultureInfo culture = CultureInfo.InstalledUICulture;
        private static Boolean? sc;

        #region P:DefaultCulture:CultureInfo
        public static CultureInfo DefaultCulture {
            get { return culture; }
            set
                {
                value = value ?? CultureInfo.InstalledUICulture;
                culture = value;
                DefaultCultureChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        #endregion
        #region P:IsRunningUnderServiceControl:Boolean
        public static Boolean IsRunningUnderServiceControl { get {
            if (sc == null) {
                sc = false;
                var processes = new Dictionary<Int64, Process>();
                foreach (var process in Process.Processes) { processes[process.UniqueProcessId] = process; }
                var i = processes[GetCurrentProcessId()];   if ((i.SessionId == 0)) {
                    i = processes[i.UniqueParentProcessId]; if ((i.SessionId == 0) && (String.Equals(i.ImageName, "services.exe", StringComparison.OrdinalIgnoreCase))) {
                    i = processes[i.UniqueParentProcessId]; if ((i.SessionId == 0) && (String.Equals(i.ImageName, "wininit.exe", StringComparison.OrdinalIgnoreCase)))
                        {
                        sc = true;
                        }}
                    }
                }
            return sc.GetValueOrDefault();
            }}
        #endregion
        #region P:IsRemoteSession:Boolean
        public static Boolean IsRemoteSession { get {
            if (GetSystemMetrics(SM_REMOTECONTROL) > 0) { return true; }
            if (GetSystemMetrics(SM_REMOTESESSION) > 0) { return true; }
            if (RegOpenKeyEx(HKEY_LOCAL_MACHINE,TERMINAL_SERVER_KEY,0,RegistrySpecificAccessRights.Read, out var key) == 0) {
                try
                    {
                    var size = sizeof(Int32);
                    if (RegQueryValueEx(key, GLASS_SESSION_ID, IntPtr.Zero, IntPtr.Zero, out var value, ref size) == 0) {
                        if (ProcessIdToSessionId(GetCurrentProcessId(), out var session)) {
                            if (session != value) { return true; }
                            }
                        }
                    }
                finally
                    {
                    RegCloseKey(key);
                    }
                }
            return false;
            }}
        #endregion

        public static event EventHandler DefaultCultureChanged;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern Int32 GetCurrentProcessId();
        [DllImport("user32.dll")] private static extern Int32 GetSystemMetrics(Int32 index);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Auto)] private static extern Int32 RegOpenKeyEx(IntPtr key, String lpSubKey, Int32 ulOptions, RegistrySpecificAccessRights samDesired, out IntPtr hkResult);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Auto)] private static extern Int32 RegQueryValueEx(IntPtr key, String valuename, IntPtr reserved, IntPtr type, out Int32 data, ref Int32 size);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)] private static extern Boolean ProcessIdToSessionId([In] Int32 process, out Int32 session);
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)] [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] internal static extern Int32 RegCloseKey(IntPtr key);

        private const Int32 SM_REMOTESESSION = 0x1000;
        private const Int32 SM_REMOTECONTROL = 0x2001;
        private static readonly IntPtr HKEY_LOCAL_MACHINE = (IntPtr)(unchecked((Int32)0x80000002));
        private const String TERMINAL_SERVER_KEY = "SYSTEM\\CurrentControlSet\\Control\\Terminal Server\\";
        private const String GLASS_SESSION_ID    = "GlassSessionId";
        }
    }