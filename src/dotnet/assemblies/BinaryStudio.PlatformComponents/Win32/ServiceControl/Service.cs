using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class Service : ServiceObject
        {
        public override IntPtr Handle { get { return sc; }}
        public SERVICE_TYPE Type { get; }

        public SERVICE_STATE CurrentState { get {
            return QueryServiceStatus(sc, out var service)
                ? service.CurrentState
                : (SERVICE_STATE)0;
            }}

        internal Service(ServiceManager manager, IntPtr sc)
            {
            if (sc == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(sc)); }
            if (manager == null)   { throw new ArgumentNullException(nameof(manager));  }
            this.manager = manager;
            this.sc = sc;
            if (QueryServiceStatus(sc, out var service)) {
                Type = service.ServiceType;
                }
            }

        public void Start() {
            Start(Timeout.Infinite);
            }

        public void Start(Int64 timeout) {
            if ((timeout < -1L) || (timeout > (Int64)Int32.MaxValue)) { throw new ArgumentOutOfRangeException(nameof(timeout)); }
            Validate(QueryServiceStatus(sc, out var status));
            if (status.CurrentState != SERVICE_STATE.SERVICE_RUNNING) {
                Validate(StartServiceW(Handle, 0, null));
                var r = new Thread(()=>{
                    while (CurrentState != SERVICE_STATE.SERVICE_RUNNING)
                        {
                        #if NET35
                        Thread.Sleep(0);
                        #else
                        Thread.Yield();
                        #endif
                        }
                    });
                r.Start();
                r.Join((Int32)timeout);
                }
            }

        public void Start(TimeSpan timeout) {
            Start((Int64)timeout.TotalMilliseconds);
            }

        public void Stop() {
            Stop(Timeout.Infinite);
            }

        private void Validate(Boolean status)
            {
            }

        public void Stop(Int64 timeout) {
            if ((timeout < -1L) || (timeout > (Int64)Int32.MaxValue)) { throw new ArgumentOutOfRangeException(nameof(timeout)); }
            Validate(QueryServiceStatus(sc, out var status));
            if (status.CurrentState != SERVICE_STATE.SERVICE_STOPPED) {
                Validate(ControlService(Handle, SERVICE_CONTROL_STOP, out status));
                var r = new Thread(()=>{
                    while (CurrentState != SERVICE_STATE.SERVICE_STOPPED)
                        {
                        #if NET35
                        Thread.Sleep(0);
                        #else
                        Thread.Yield();
                        #endif
                        }
                    });
                r.Start();
                r.Join((Int32)timeout);
                }
            }

        public void Stop(TimeSpan timeout) {
            Start((Int64)timeout.TotalMilliseconds);
            }

        protected override void Dispose(Boolean disposing) {
            CloseServiceHandle(ref sc);
            }

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
        private ServiceManager manager;
        private IntPtr sc;
        }
    }