using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public static class ProcessHelper
        {
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQueryInformationProcess(IntPtr process, Int32 @class, out ProcessBasicInformation pi, Int32 pisz, out Int32 r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQueryInformationProcess(IntPtr process, Int32 @class, void* pi, Int32 pisz, out Int32 r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQuerySystemInformation(SystemInformationType query, void* dataPtr, Int32 size, IntPtr r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtOpenProcess(out IntPtr r, Int32 desiredaccess, ref OBJECT_ATTRIBUTES objectattributes, ref CLIENT_ID client);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtOpenProcess(out ProcessHandle r, ProcessSpecificAccessRights desiredaccess, ref OBJECT_ATTRIBUTES objectattributes, ref CLIENT_ID client);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentThread();
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)] private static extern unsafe void* LocalAlloc(Int32 flags, IntPtr size);
        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* data);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Auto, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr lpSource_mustBeNull,  UInt32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, int nSize, IntPtr[] arguments);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi)] private static extern IntPtr GetProcAddress(IntPtr module, String procedure);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr LoadLibrary(String filename);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern unsafe Boolean QueryFullProcessImageName(ProcessHandle process, UInt32 flags, char* name, ref Int32 sz);
        [DllImport("advapi32.dll", SetLastError = true)] private static extern Boolean OpenThreadToken(IntPtr threadhandle, UInt32 desiredaccess, Boolean openasself, out IntPtr tokenhandle);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)] private static extern Boolean LookupPrivilegeValue(String systemname, String name, out LUID luid);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean AdjustTokenPrivileges(IntPtr tokenhandle, Boolean disableallprivileges, ref TOKEN_PRIVILEGE newstate, Int32 bufferlength, ref TOKEN_PRIVILEGE previousstate, ref Int32 returnlength);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern unsafe Boolean AdjustTokenPrivileges(IntPtr tokenhandle, Boolean disableallprivileges, ref TOKEN_PRIVILEGE newstate, Int32 bufferlength, void* previousstate, Int32* returnlength);
        [DllImport("kernel32.dll")] private static extern Boolean CloseHandle(IntPtr o);
        [DllImport("advapi32.dll")] private static extern Boolean ImpersonateSelf(SECURITY_IMPERSONATION_LEVEL ImpersonationLevel);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] public static extern IntPtr CreateToolhelp32Snapshot(TH32CS_FLAGS flags, Int32 processId);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Process32FirstW")] private static extern unsafe Boolean Process32First(IntPtr handle, [In][Out] void* entry);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Process32NextW")] private static extern unsafe Boolean Process32Next(IntPtr handle, [In][Out] void* entry);

        private const String SE_DEBUG_NAME          = "SeDebugPrivilege";
        private const UInt32 SE_PRIVILEGE_ENABLED   = 0x00000002;

        public static unsafe String GetProcessName(Int32 identifer)
            {
            if (identifer <= 0) { throw new ArgumentOutOfRangeException(nameof(identifer)); }
            var snapshot = CreateToolhelp32Snapshot(TH32CS_FLAGS.TH32CS_SNAPPROCESS, 0);
            try
                {
                var e = new PROCESSENTRY32 { Size = sizeof(PROCESSENTRY32) };
                var r = &e;
                if (Process32First(snapshot, r)) {
                    if (r->ProcessId == identifer) { return new String(r->ExeFile); }
                    while (Process32Next(snapshot, r)) {
                        if (r->ProcessId == identifer) {
                            return new String(r->ExeFile);
                            }
                        }
                    }
                }
            finally
                {
                CloseHandle(snapshot);
                }
            return null;
            }

        private static unsafe Boolean AdjustTokenPrivilege(IntPtr tokenhandle, String privilege, Boolean value) {
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

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private unsafe delegate UInt32 NtWow64QueryInformationProcess64(IntPtr process, UInt32 flags, void* buffer, UInt32 count, UInt32* r);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private unsafe delegate UInt32 NtWow64ReadVirtualMemory64(IntPtr process, UInt64 baseaddress, void* buffer, UInt64 count, UInt64* r);

        #region M:NtQueryInformationProcess(IntPtr,[out]PROCESS_BASIC_INFORMATION):NTSTATUS
        private static unsafe NTSTATUS NtQueryInformationProcess(IntPtr process, out ProcessBasicInformation r) {
            return NtQueryInformationProcess(process, ProcessBasicInformation,
                out r, sizeof(ProcessBasicInformation), out var rsz);
            }
        #endregion
        #region M:QueryFullProcessImageName(IntPtr):String
        public static unsafe String QueryFullProcessImageName(ProcessHandle source) {
            if (source == null) { return null; }
            Char* pspi;
            for (var size = 0x800; (pspi = (Char*)LocalAlloc(LMEM_FIXED, (IntPtr)size)) != null; size += 0x800) {
                try
                    {
                    var rsz = size;
                    if (QueryFullProcessImageName(source, 0, pspi, ref rsz)) {
                        if (rsz < size) {
                            return new String(pspi);
                            }
                        }
                    else
                        {
                        break;
                        }
                    }
                finally
                    {
                    LocalFree(pspi);
                    }
                }
            return null;
            }
        #endregion

        public static unsafe NTSTATUS NtQueryInformationProcess(IntPtr process, out LocalMemory o) {
            o = null;
            void* pspi;
            var size = 0x8000;
            for (;(pspi = LocalAlloc(LMEM_FIXED, (IntPtr)size)) != null;size += 0x8000) {
                Int32 rsz;
                var ns = NtQueryInformationProcess(process, ProcessBasicInformation, pspi, size, out rsz);
                if (ns == NTSTATUS.STATUS_SUCCESS) {
                    o = new LocalMemory((IntPtr)pspi,size);
                    return ns;
                    }
                LocalFree(pspi);
                if (ns != NTSTATUS.STATUS_INFO_LENGTH_MISMATCH) {
                    return ns;
                    }
                }
            return NTSTATUS.RPC_NT_CALL_FAILED;
            }

        #region M:GetProcesses64:IList<ProcessInfo>
        private static unsafe IList<ProcessInfo> GetProcesses64() {
            var r = new SortedDictionary<Int64, ProcessInfo>();
            LocalMemory pspi;
            for (var size = 0x8000; (void*)(pspi = new LocalMemory(size, LMEM_FIXED)) != null; size += 0x800) {
                var ns = NtQuerySystemInformation(SystemInformationType.SystemProcessInformation, pspi, size, IntPtr.Zero);
                if (ns == NTSTATUS.STATUS_SUCCESS) {
                    var i = (SYSTEM_PROCESS_INFORMATION64*)pspi;
                    while (i->NextEntryOffset != 0) {
                        IntPtr processhandle;
                        var client = new CLIENT_ID{ UniqueProcess = (IntPtr)i->UniqueProcessId};
                        var attributes = new OBJECT_ATTRIBUTES
                            {
                            Length = sizeof(OBJECT_ATTRIBUTES)
                            };
                        ns = NtOpenProcess(out processhandle, GENERIC_READ, ref attributes, ref client);
                        Debug.Print("IntPtr:{0}:{1}", processhandle, FormatMessage(Marshal.GetLastWin32Error()));
                        r.Add(i->UniqueProcessId, new ProcessInfo(ref *i));
                        i = (SYSTEM_PROCESS_INFORMATION64*)((Byte*)i + i->NextEntryOffset);
                        }
                    break;
                    }
                if (ns != NTSTATUS.STATUS_INFO_LENGTH_MISMATCH) {
                    if (ns != NTSTATUS.STATUS_SUCCESS) { Marshal.ThrowExceptionForHR((Int32)ns); }
                    break;
                    }
                }
            return r.Select(i => i.Value).ToArray();
            }
        #endregion
        #region M:GetProcesses32:IList<ProcessInfo>
        private static unsafe IList<ProcessInfo> GetProcesses32() {
            var r = new SortedDictionary<Int64, ProcessInfo>();
            LocalMemory pspi;
            for (var size = 0x8000; (void*)(pspi = new LocalMemory(size, LMEM_FIXED)) != null; size += 0x800) {
                var ns = NtQuerySystemInformation(SystemInformationType.SystemProcessInformation, pspi, size, IntPtr.Zero);
                if (ns == NTSTATUS.STATUS_SUCCESS) {
                    var i = (SYSTEM_PROCESS_INFORMATION32*)pspi;
                    while (i->NextEntryOffset != 0) {
                        IntPtr processhandle;
                        var client = new CLIENT_ID{ UniqueProcess = (IntPtr)i->UniqueProcessId};
                        var attributes = new OBJECT_ATTRIBUTES {Length = sizeof(OBJECT_ATTRIBUTES)};
                        ns = NtOpenProcess(out processhandle, GENERIC_READ, ref attributes, ref client);
                        Debug.Print("IntPtr:{0}:{1}", processhandle, FormatMessage(Marshal.GetLastWin32Error()));
                        r.Add(i->UniqueProcessId, new ProcessInfo(ref *i));
                        i = (SYSTEM_PROCESS_INFORMATION32*)((Byte*)i + i->NextEntryOffset);
                        }
                    break;
                    }
                if (ns != NTSTATUS.STATUS_INFO_LENGTH_MISMATCH) {
                    if (ns != NTSTATUS.STATUS_SUCCESS) { Marshal.ThrowExceptionForHR((Int32)ns); }
                    break;
                    }
                }
            return r.Select(i => i.Value).ToArray();
            }
        #endregion

        public static IList<ProcessInfo> GetProcesses()
            {
            return Environment.Is64BitProcess
                ? GetProcesses64()
                : GetProcesses32();
            }

        public const Int32 ProcessBasicInformation   =  0;
        public const Int32 ProcessDebugPort          =  7;
        public const Int32 ProcessWow64Information   = 26;
        public const Int32 ProcessImageFileName      = 27;
        public const Int32 ProcessBreakOnTermination = 29;

        private const Int32 LMEM_FIXED          = 0x0000;
        private const Int32 LMEM_MOVEABLE       = 0x0002;
        private const Int32 LMEM_NOCOMPACT      = 0x0010;
        private const Int32 LMEM_NODISCARD      = 0x0020;
        private const Int32 LMEM_ZEROINIT       = 0x0040;
        private const Int32 LMEM_MODIFY         = 0x0080;
        private const Int32 LMEM_DISCARDABLE    = 0x0F00;
        private const Int32 LMEM_VALID_FLAGS    = 0x0F72;
        private const Int32 LMEM_INVALID_HANDLE = 0x8000;
        private const Int32 GENERIC_READ        = unchecked((Int32)0x80000000);
        private const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private const UInt32 LANG_NEUTRAL                   = 0x00;
        private const UInt32 SUBLANG_DEFAULT                = 0x01;
        private const UInt32 TOKEN_ADJUST_PRIVILEGES        = 0x0020;
        private const UInt32 TOKEN_QUERY                    = 0x0008;
        private const Int32  ERROR_NO_TOKEN                 = 1008;

        #region M:FormatMessage(Int32):String
        private static unsafe String FormatMessage(Int32 source) {
            void* r;
            if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, (UInt32)source,
                ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL)),
                &r, 0, new IntPtr[0])) {
                try
                    {
                    return new String((char*)r);
                    }
                finally
                    {
                    LocalFree(r);
                    }
                }
            return null;
            }
        #endregion
        #region M:GetParentProcessHandle():ProcessHandle
        public static ProcessHandle GetParentProcessHandle() {
            return GetProcessHandle(
                GetParentProcessIdentifier((Int32)GetCurrentProcess()),
                ProcessSpecificAccessRights.ProcessQueryLimitedInformation|ProcessSpecificAccessRights.ProcessVirtualMemoryRead);
            }
        #endregion
        #region M:GetParentProcessIdentifier(Int32):Int32
        private static Int32 GetParentProcessIdentifier(Int32 identifer) {
            if (identifer == 0) { throw new ArgumentOutOfRangeException(nameof(identifer)); }
            Validate(NtQueryInformationProcess((IntPtr)identifer, out ProcessBasicInformation r));
            return (Int32)r.InheritedFromUniqueProcessId;
            }
        #endregion
        #region M:GetParentProcessIdentifier():Int32
        public static Int32 GetParentProcessIdentifier() {
            return GetParentProcessIdentifier((Int32)GetCurrentProcess());
            }
        #endregion
        #region M:GetProcessHandle(Int32,Int32):ProcessHandle
        private static unsafe ProcessHandle GetProcessHandle(Int32 identifier, ProcessSpecificAccessRights desiredaccess) {
            var client = new CLIENT_ID{
                UniqueProcess = (IntPtr)identifier
                };
            var attributes = new OBJECT_ATTRIBUTES {
                Length = sizeof(OBJECT_ATTRIBUTES)
                };
            try
                {
                Validate(NtOpenProcess(out ProcessHandle r, desiredaccess, ref attributes, ref client));
                return r;
                }
            catch (NTStatusException e) {
                if (e.Status == NTSTATUS.STATUS_ACCESS_DENIED) {
                    var tokenhandle = IntPtr.Zero;
                    try
                        {
                        if (!OpenThreadToken(GetCurrentThread(),TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,false, out tokenhandle)) {
                            if (Marshal.GetLastWin32Error() == ERROR_NO_TOKEN) {
                                if (ImpersonateSelf(SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation)) {
                                    OpenThreadToken(GetCurrentThread(),TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,false, out tokenhandle);
                                    }
                                }
                            }
                        if (tokenhandle != IntPtr.Zero) {
                            AdjustTokenPrivilege(tokenhandle, SE_DEBUG_NAME, true);
                            try
                                {
                                Validate(NtOpenProcess(out ProcessHandle r, desiredaccess, ref attributes, ref client));
                                return r;
                                }
                            catch (Exception exception)
                                {
                                exception.Data["DesiredAccess"] = desiredaccess;
                                exception.Data["ProcessIdentifier"] = identifier;
                                exception.Data["TokenHandle"] = tokenhandle;
                                throw;
                                }
                            }
                        }
                    finally
                        {
                        if (tokenhandle != IntPtr.Zero) { CloseHandle(tokenhandle); }
                        }
                    }
                e.Data["DesiredAccess"] = desiredaccess;
                e.Data["ProcessIdentifier"] = identifier;
                throw;
                }
            catch (Exception e)
                {
                e.Data["DesiredAccess"] = desiredaccess;
                e.Data["ProcessIdentifier"] = identifier;
                throw;
                }
            }
        #endregion

        #region M:Validate([Out]Exception,NTSTATUS):Boolean
        private static Boolean Validate(out Exception e, NTSTATUS status)
            {
            e = null;
            if (status == NTSTATUS.STATUS_SUCCESS) { return true; }
            e = new NTStatusException(status);
            #if DEBUG
            Debug.Print($"NTStatusException:{e.Message}");
            #endif
            return false;
            }
        #endregion
        #region M:Validate([Out]Exception,Boolean):Boolean
        private static Boolean Validate(out Exception e, Boolean status) {
            e = null;
            if (!status) {
                var i = Marshal.GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]", i);
                    #if DEBUG
                    Debug.Print($"COMException:{e.Message}");
                    #endif
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]", i);
                    #if DEBUG
                    Debug.Print($"COMException:{e.Message}");
                    #endif
                    }
                return false;
                }
            return true;
            }
        #endregion
        #region M:Validate(NTSTATUS)
        private static void Validate(NTSTATUS status)
            {
            if (status == NTSTATUS.STATUS_SUCCESS) { return; }
            var e = new NTStatusException(status);
            #if DEBUG
            Debug.Print($"NTStatusException:{e.Message}");
            #endif
            throw e;
            }
        #endregion

        private struct NtWow64Functions
            {
            public NtWow64QueryInformationProcess64 NtWow64QueryInformationProcess64;
            public NtWow64ReadVirtualMemory64 NtWow64ReadVirtualMemory64;
            }

        private static NtWow64Functions NtWow64;
        static ProcessHelper()
            {
            var ntdll = LoadLibrary("ntdll.dll");
            NtWow64.NtWow64QueryInformationProcess64 = (NtWow64QueryInformationProcess64)GetDelegateForFunctionPointer(GetProcAddress(ntdll, nameof(NtWow64QueryInformationProcess64)), typeof(NtWow64QueryInformationProcess64));
            NtWow64.NtWow64ReadVirtualMemory64       = (NtWow64ReadVirtualMemory64)GetDelegateForFunctionPointer(GetProcAddress(ntdll, nameof(NtWow64ReadVirtualMemory64)), typeof(NtWow64ReadVirtualMemory64));
            }

        private static Delegate GetDelegateForFunctionPointer(IntPtr ptr, Type type) {
            return (ptr != IntPtr.Zero)
                ? Marshal.GetDelegateForFunctionPointer(ptr, type)
                : null;
            }
        }
    }