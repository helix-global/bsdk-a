using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public partial class ServiceManager : ServiceObject
        {
        public override IntPtr Handle { get { return sc; }}

        public ServiceManager()
            {
            sc = OpenSCManagerW();
            }

        private struct LUID_AND_ATTRIBUTES
            {
            public LUID Luid;
            public UInt32 Attributes;
            }

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern IntPtr OpenSCManagerW([MarshalAs(UnmanagedType.LPWStr)] [In] String machinename = null, [MarshalAs(UnmanagedType.LPWStr)] [In] String databasename = null, UInt32 desiredaccess = SC_MANAGER_ALL_ACCESS);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern IntPtr CreateServiceW(IntPtr manager, [MarshalAs(UnmanagedType.LPWStr)] [In] String servicename, [MarshalAs(UnmanagedType.LPWStr)] [In] String displayname, UInt32 desiredaccess, UInt32 servicetype, UInt32 starttype, UInt32 errorcontrol, [MarshalAs(UnmanagedType.LPWStr)] [In] String binarypathname, [MarshalAs(UnmanagedType.LPWStr)] [In] String loadordergroup = null, [MarshalAs(UnmanagedType.LPWStr)] [In] String tagid = null, [MarshalAs(UnmanagedType.LPWStr)] [In] String dependencies = null, [MarshalAs(UnmanagedType.LPWStr)] [In] String servicestartname = null, [MarshalAs(UnmanagedType.LPWStr)] [In] String password = null);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern IntPtr OpenServiceW(IntPtr manager, [MarshalAs(UnmanagedType.LPWStr)] [In] String servicename, UInt32 desiredaccess);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean DeleteService(IntPtr service);
        [DllImport("advapi32.dll", SetLastError = true)] private static extern Boolean OpenThreadToken(IntPtr threadhandle, UInt32 desiredaccess, Boolean openasself, out IntPtr tokenhandle);
        [DllImport("advapi32.dll", SetLastError = true)] private static extern Boolean OpenProcessToken(IntPtr processhandle, UInt32 desiredaccess, out IntPtr tokenhandle);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)] private static extern Boolean LookupPrivilegeValue(String systemname, String name, out LUID luid);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean AdjustTokenPrivileges(IntPtr tokenhandle, Boolean disableallprivileges, ref TOKEN_PRIVILEGE newstate, Int32 bufferlength, ref TOKEN_PRIVILEGE previousstate, ref Int32 returnlength);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern unsafe Boolean AdjustTokenPrivileges(IntPtr tokenhandle, Boolean disableallprivileges, ref TOKEN_PRIVILEGE newstate, Int32 bufferlength, void* previousstate, Int32* returnlength);
        [DllImport("advapi32.dll", SetLastError = true)] private static extern Boolean ImpersonateSelf(UInt32 impersonationlevel);
        [DllImport("advapi32.dll", SetLastError = true)] private static extern Boolean ImpersonateLoggedOnUser(IntPtr token);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentThread();
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll")] private static extern Boolean CloseHandle(IntPtr o);
        [DllImport("advapi32.dll", SetLastError = true)] public static extern Boolean LogonUser(String lpszUserName,String lpszDomain,String lpszPassword,int dwLogonType,int dwLogonProvider,out IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean QueryServiceStatus(IntPtr service, out SERVICE_STATUS status);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean StartServiceW(IntPtr service, UInt32 numserviceargs, [MarshalAs(UnmanagedType.LPArray)] [In] String[] serviceargvectors);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean ControlService(IntPtr service, Int32 control, out SERVICE_STATUS status);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean ControlServiceExW(IntPtr service, Int32 control, Int32 level, out SERVICE_STATUS status);

        private const Int32 SERVICE_CONTROL_STATUS_REASON_INFO      = 1;
        private const Int32 SERVICE_CONTROL_STOP                    = 0x00000001;
        private const Int32 SERVICE_CONTROL_PAUSE                   = 0x00000002;
        private const Int32 SERVICE_CONTROL_CONTINUE                = 0x00000003;
        private const Int32 SERVICE_CONTROL_INTERROGATE             = 0x00000004;
        private const Int32 SERVICE_CONTROL_SHUTDOWN                = 0x00000005;
        private const Int32 SERVICE_CONTROL_PARAMCHANGE             = 0x00000006;
        private const Int32 SERVICE_CONTROL_NETBINDADD              = 0x00000007;
        private const Int32 SERVICE_CONTROL_NETBINDREMOVE           = 0x00000008;
        private const Int32 SERVICE_CONTROL_NETBINDENABLE           = 0x00000009;
        private const Int32 SERVICE_CONTROL_NETBINDDISABLE          = 0x0000000A;
        private const Int32 SERVICE_CONTROL_DEVICEEVENT             = 0x0000000B;
        private const Int32 SERVICE_CONTROL_HARDWAREPROFILECHANGE   = 0x0000000C;
        private const Int32 SERVICE_CONTROL_POWEREVENT              = 0x0000000D;
        private const Int32 SERVICE_CONTROL_SESSIONCHANGE           = 0x0000000E;
        private const Int32 SERVICE_CONTROL_PRESHUTDOWN             = 0x0000000F;
        private const Int32 SERVICE_CONTROL_TIMECHANGE              = 0x00000010;
        private const Int32 SERVICE_CONTROL_USER_LOGOFF             = 0x00000011;
        private const Int32 SERVICE_CONTROL_TRIGGEREVENT            = 0x00000020;
        private const Int32 SERVICE_CONTROL_LOWRESOURCES            = 0x00000060;
        private const Int32 SERVICE_CONTROL_SYSTEMLOWRESOURCES      = 0x00000061;

        private const UInt32 SC_MANAGER_CONNECT             = 0x0001;
        private const UInt32 SC_MANAGER_CREATE_SERVICE      = 0x0002;
        private const UInt32 SC_MANAGER_ENUMERATE_SERVICE   = 0x0004;
        private const UInt32 SC_MANAGER_LOCK                = 0x0008;
        private const UInt32 SC_MANAGER_QUERY_LOCK_STATUS   = 0x0010;
        private const UInt32 SC_MANAGER_MODIFY_BOOT_CONFIG  = 0x0020;

        private const UInt32 SERVICE_QUERY_CONFIG           = 0x0001;
        private const UInt32 SERVICE_CHANGE_CONFIG          = 0x0002;
        private const UInt32 SERVICE_QUERY_STATUS           = 0x0004;
        private const UInt32 SERVICE_ENUMERATE_DEPENDENTS   = 0x0008;
        private const UInt32 SERVICE_START                  = 0x0010;
        private const UInt32 SERVICE_STOP                   = 0x0020;
        private const UInt32 SERVICE_PAUSE_CONTINUE         = 0x0040;
        private const UInt32 SERVICE_INTERROGATE            = 0x0080;
        private const UInt32 SERVICE_USER_DEFINED_CONTROL   = 0x0100;
        private const UInt32 SERVICE_KERNEL_DRIVER          = 0x00000001;
        private const UInt32 SERVICE_DEMAND_START           = 0x00000003;
        private const UInt32 SERVICE_ERROR_NORMAL           = 0x00000001;
        private const Int32  ERROR_SERVICE_EXISTS           = 1073;

        private const UInt32 STANDARD_RIGHTS_REQUIRED       = 0x000F0000;
        private const UInt32 SC_MANAGER_ALL_ACCESS          = STANDARD_RIGHTS_REQUIRED|SC_MANAGER_CONNECT|SC_MANAGER_CREATE_SERVICE|SC_MANAGER_ENUMERATE_SERVICE|SC_MANAGER_LOCK|SC_MANAGER_QUERY_LOCK_STATUS|SC_MANAGER_MODIFY_BOOT_CONFIG;
        private const UInt32 SERVICE_ALL_ACCESS             = STANDARD_RIGHTS_REQUIRED|SERVICE_QUERY_CONFIG|SERVICE_CHANGE_CONFIG|SERVICE_QUERY_STATUS|SERVICE_ENUMERATE_DEPENDENTS|SERVICE_START|SERVICE_STOP|SERVICE_PAUSE_CONTINUE|SERVICE_INTERROGATE|SERVICE_USER_DEFINED_CONTROL;
        private const String ServiceName                    = "ntosdrv";
        private const UInt32 TOKEN_ADJUST_PRIVILEGES        = 0x0020;
        private const UInt32 TOKEN_QUERY                    = 0x0008;
        private const Int32  ERROR_NO_TOKEN                 = 1008;
        private const String SE_DEBUG_NAME                  = "SeDebugPrivilege";
        private const UInt32 SE_PRIVILEGE_ENABLED           = 0x00000002;
        private const UInt32 SecurityImpersonation          = 2;
        const int LOGON32_PROVIDER_DEFAULT  = 0;
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_SERVICE     = 5;

        private static unsafe Boolean AdjustTokenPrivilege(IntPtr tokenhandle,String privilege, Boolean value) {
            if (tokenhandle == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(tokenhandle)); }
            LUID luid;
            if(!LookupPrivilegeValue(null, privilege, out luid )) { return false; }
            var tokenprivileges = new TOKEN_PRIVILEGE();
            var previous = new TOKEN_PRIVILEGE();
            tokenprivileges.PrivilegeCount = 1;
            tokenprivileges.Privilege.Luid = luid;
            tokenprivileges.Privilege.Attributes = 0;
            var sz = Marshal.SizeOf(previous);
            if (AdjustTokenPrivileges(tokenhandle, false, ref tokenprivileges, Marshal.SizeOf(tokenprivileges), ref previous, ref sz)) {
                previous.PrivilegeCount = 1;
                previous.Privilege.Luid = luid;
                if (value)
                    {
                    previous.Privilege.Attributes |= SE_PRIVILEGE_ENABLED;
                    }
                else
                    {
                    previous.Privilege.Attributes &= ~SE_PRIVILEGE_ENABLED;
                    }
                return AdjustTokenPrivileges(tokenhandle, false, ref previous, sz, null, null);
                }
            return false;
            }

        private void EnableDebugPrivileges()
            {
            var tokenhandle = IntPtr.Zero;
            try
                {
                if (!OpenThreadToken(GetCurrentThread(),TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,false, out tokenhandle)) {
                    if (Marshal.GetLastWin32Error() == ERROR_NO_TOKEN) {
                        if (ImpersonateSelf(SecurityImpersonation)) {
                            OpenThreadToken(GetCurrentThread(),TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,false, out tokenhandle);
                            }
                        }
                    }
                if (tokenhandle != IntPtr.Zero) {
                    AdjustTokenPrivilege(tokenhandle, SE_DEBUG_NAME, true);
                    }
                }
            finally
                {
                if (tokenhandle != IntPtr.Zero) { CloseHandle(tokenhandle); }
                }
            }

        public Service OpenService(String servicename) {
            if (servicename == null) { throw new ArgumentNullException(nameof(servicename)); }
            #if NET35
            if (String.IsNullOrEmpty(servicename)) { throw new ArgumentOutOfRangeException(nameof(servicename)); }
            #else
            if (String.IsNullOrWhiteSpace(servicename)) { throw new ArgumentOutOfRangeException(nameof(servicename)); }
            #endif
            var r = OpenServiceW(sc, servicename, SC_MANAGER_ALL_ACCESS);
            return (r != IntPtr.Zero)
                ? new Service(this, r)
                : null;
            }

        public void DeleteService(ref Service service) {
            if (service != null) {
                service.Stop(Timeout.Infinite);
                DeleteService(service.Handle);
                service.Dispose();
                service = null;
                }
            }

        protected override void Dispose(Boolean disposing) {
            CloseServiceHandle(ref sc);
            }

        private IntPtr sc;
        }
    }