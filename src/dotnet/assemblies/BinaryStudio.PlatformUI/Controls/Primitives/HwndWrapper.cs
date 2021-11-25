using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI {
    public abstract class HwndWrapper : DisposableObject {
        private Boolean isHandleCreationAllowed = true;
        private const String DestroyWindowFailedEvent = "VS/Platform/HwndWrapper/Destroy-Window-Error";
        private const String DestroyWindowFailedErrorProperty = "VS.Platform.HwndWrapper.Destroy-Window-Error.Win32Error";
        private IntPtr handle;
        private UInt16 wndClassAtom;
        private Delegate wndProc;
        private static Int64 failedDestroyWindows;
        private static Int32 lastDestroyWindowError;

        [CLSCompliant(false)]
        protected UInt16 WindowClassAtom {
            get {
                if (wndClassAtom == 0)
                    wndClassAtom = CreateWindowClassCore();
                return wndClassAtom;
                }
            }

        public IntPtr Handle {
            get {
                EnsureHandle();
                return handle;
                }
            }

        protected virtual Boolean IsWindowSubclassed {
            get {
                return false;
                }
            }

        [CLSCompliant(false)]
        protected virtual UInt16 CreateWindowClassCore() {
            return RegisterClass(Guid.NewGuid().ToString());
            }

        protected virtual void DestroyWindowClassCore() {
            if (wndClassAtom == 0)
                return;
            NativeMethods.UnregisterClass(new IntPtr(wndClassAtom), NativeMethods.GetModuleHandle(null));
            wndClassAtom = 0;
            }

        [CLSCompliant(false)]
        protected UInt16 RegisterClass(String className) {
            var lpWndClass = new WNDCLASS
            {
                cbClsExtra = 0,
                cbWndExtra = 0,
                hbrBackground = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hIcon = IntPtr.Zero,
                lpfnWndProc = wndProc = new NativeMethods.WndProc(WndProc),
                lpszClassName = className,
                lpszMenuName = null,
                style = 0U
                };
            return NativeMethods.RegisterClass(ref lpWndClass);
            }

        private void SubclassWndProc() {
            wndProc = new NativeMethods.WndProc(WndProc);
            NativeMethods.SetWindowLong(handle, -4, Marshal.GetFunctionPointerForDelegate(wndProc));
            }

        protected abstract IntPtr CreateWindowCore();

        protected virtual void DestroyWindowCore() {
            if (!(handle != IntPtr.Zero))
                return;
            if (!NativeMethods.DestroyWindow(handle)) {
                lastDestroyWindowError = Marshal.GetLastWin32Error();
                ++failedDestroyWindows;
                /*
        // ISSUE: variable of a compiler-generated type
        IVsPostFaultEvent globalService = Package.GetGlobalService(typeof (SVsTelemetryService)) as IVsPostFaultEvent;
        if (globalService != null)
        {
          // ISSUE: reference to a compiler-generated method
          // ISSUE: variable of a compiler-generated type
          IVsFaultEvent fault = globalService.CreateFault("VS/Platform/HwndWrapper/Destroy-Window-Error", "Call to DestroyWindow failed in HwndWrapper", (string) null, (IVsFaultEventCallBack) null);
          // ISSUE: reference to a compiler-generated method
          fault.SetProperty("VS.Platform.HwndWrapper.Destroy-Window-Error.Win32Error", (object) HwndWrapper.lastDestroyWindowError);
          // ISSUE: reference to a compiler-generated method
          TelemetryHelper.DefaultTelemetrySession.PostEvent((IVsTelemetryEvent) fault);
          
        }*/
                }
            handle = IntPtr.Zero;
            }

        protected virtual IntPtr WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam) {
            try
                {
                //Debug.Print(">HwndWrapper.WndProc");
                return NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
                }
            finally
                {
                //Debug.Print("<HwndWrapper.WndProc");
                }

            }

        public void EnsureHandle() {
            if (!(handle == IntPtr.Zero))
                return;
            if (!isHandleCreationAllowed) {
                //VSDebug.Fail_Tagged("HWWR0001", "HwndWrapper.EnsureHandle should not be called when not allowed. Because of the current state, a new HWND will not be created", "F:\\dd\\src\\env\\shell\\PackageFramework\\Current\\Shell\\UI\\Common\\HwndWrapper.cs", 177U);
                }
            else {
                isHandleCreationAllowed = false;
                handle = CreateWindowCore();
                if (!IsWindowSubclassed)
                    return;
                SubclassWndProc();
                }
            }

        protected override void DisposeNativeResources() {
            isHandleCreationAllowed = false;
            DestroyWindowCore();
            DestroyWindowClassCore();
            }
        }
    }