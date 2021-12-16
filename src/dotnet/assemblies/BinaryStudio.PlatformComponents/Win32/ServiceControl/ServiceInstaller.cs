using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class ServiceInstaller
        {
        public Boolean InstallService(String svcPath, String svcName, String svcDispName)
            {
            try
                {
                var sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                if (sc_handle.ToInt32() != 0) {
                    var sv_handle = CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, null, null);
                    if (sv_handle.ToInt32() == 0) {
                        CloseServiceHandle(sc_handle);
                        return false;
                        }
                    else
                        {
                        //now trying to start the service
                        var i = StartService(sv_handle, 0, null);
                        // If the value i is zero, then there was an error starting the service.
                        // note: error may arise if the service is already running or some other problem.
                        if (i == 0)
                            {
                            //Console.WriteLine("Couldnt start service");
                            return false;
                            }
                        CloseServiceHandle(sc_handle);
                        return true;
                        }
                    }
                else
                    {
                    return false;
                    }
                }
            catch (Exception e)
                {
                e.Data["SvcPath"] = svcPath;
                e.Data["SvcName"] = svcName;
                e.Data["SvcDisplayName"] = svcDispName;
                throw;
                }
            }

        public Boolean UnInstallService(String svcName) {
            var sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() != 0) {
                var svc_hndl = OpenService(sc_hndl, svcName, DELETE);
                if (svc_hndl.ToInt32() != 0) {
                    var i = DeleteService(svc_hndl);
                    if (i != 0)
                        {
                        CloseServiceHandle(sc_hndl);
                        return true;
                        }
                    else
                        {
                        CloseServiceHandle(sc_hndl);
                        return false;
                        }
                    }
                else
                    return false;
                }
            else
                return false;
            }

        [DllImport("advapi32.dll")] private static extern IntPtr OpenSCManager(String lpMachineName,String lpSCDB, Int32 scParameter);
        [DllImport("Advapi32.dll")] private static extern IntPtr CreateService(IntPtr SC_HANDLE,String lpSvcName,String lpDisplayName, Int32 dwDesiredAccess,Int32 dwServiceType,Int32 dwStartType,Int32 dwErrorControl,String lpPathName, String lpLoadOrderGroup,Int32 lpdwTagId,String lpDependencies,String lpServiceStartName,String lpPassword);
        [DllImport("advapi32.dll")] private static extern void CloseServiceHandle(IntPtr SCHANDLE);
        [DllImport("advapi32.dll")] private static extern Int32 StartService(IntPtr SVHANDLE,Int32 dwNumServiceArgs,String lpServiceArgVectors);
        [DllImport("advapi32.dll",SetLastError=true)] private static extern IntPtr OpenService(IntPtr SCHANDLE,String lpSvcName,Int32 dwNumServiceArgs);
        [DllImport("advapi32.dll")] private static extern Int32 DeleteService(IntPtr SVHANDLE);
        [DllImport("kernel32.dll")] private static extern Int32 GetLastError();

        private const Int32 DELETE = 0x10000;
        private const Int32 GENERIC_WRITE = 0x40000000;
        private const Int32 SC_MANAGER_CREATE_SERVICE = 0x0002;
        private const Int32 SERVICE_WIN32_OWN_PROCESS = 0x00000010;
        private const Int32 SERVICE_DEMAND_START = 0x00000003;
        private const Int32 SERVICE_ERROR_NORMAL = 0x00000001;
        private const Int32 STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const Int32 SERVICE_QUERY_CONFIG = 0x0001;
        private const Int32 SERVICE_CHANGE_CONFIG = 0x0002;
        private const Int32 SERVICE_QUERY_STATUS = 0x0004;
        private const Int32 SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
        private const Int32 SERVICE_START =0x0010;
        private const Int32 SERVICE_STOP =0x0020;
        private const Int32 SERVICE_PAUSE_CONTINUE =0x0040;
        private const Int32 SERVICE_INTERROGATE =0x0080;
        private const Int32 SERVICE_USER_DEFINED_CONTROL =0x0100;
        private const Int32 SERVICE_ALL_ACCESS = (
                        STANDARD_RIGHTS_REQUIRED |
                        SERVICE_QUERY_CONFIG |
                        SERVICE_CHANGE_CONFIG |
                        SERVICE_QUERY_STATUS |
                        SERVICE_ENUMERATE_DEPENDENTS |
                        SERVICE_START |
                        SERVICE_STOP |
                        SERVICE_PAUSE_CONTINUE |
                        SERVICE_INTERROGATE |
                        SERVICE_USER_DEFINED_CONTROL);
        private const Int32 SERVICE_AUTO_START = 0x00000002;
        }
    }