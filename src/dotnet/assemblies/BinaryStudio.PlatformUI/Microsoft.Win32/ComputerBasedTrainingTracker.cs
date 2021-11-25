using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BinaryStudio.DataProcessing;

namespace Microsoft.Win32
    {
    public abstract class ComputerBasedTrainingTracker : DisposableObject
        {
        private WindowsHookHandle Handle {get; set; }
        private readonly WindowsHookProc procedure;

        protected ComputerBasedTrainingTracker() {
            procedure = HookProc;
            }

        #region M:DisposeNativeResources
        protected override void DisposeNativeResources() {
            base.DisposeNativeResources();
            if (Handle != null) {
                Handle.Close();
                Handle = null;
                }
            }
        #endregion
        #region M:HookProc(Int32,IntPtr,IntPtr):IntPtr
        private IntPtr HookProc(Int32 code, IntPtr W, IntPtr L) {
            if (code >= 0) {
                switch (code) {
                    case HCBT_SETFOCUS:
                        {
                        OnKeyboardFocusReceived(W,L);
                        }
                        break;
                    case HCBT_CREATEWND:
                        {

                        }
                        break;
                    }
                }
            return (Handle != null)
                ? Handle.CallNext(code, W, L)
                : IntPtr.Zero;
            }
        #endregion
        #region M:OnKeyboardFocusReceived(IntPtr,IntPtr)
        protected virtual void OnKeyboardFocusReceived(IntPtr gainFocusHandle, IntPtr loseFocusHandle) {
            Debug.Print("OnKeyboardFocusReceived:NewFocusHandle={0:X},OldFocusHandle={1:X}", (Int64)gainFocusHandle, (Int64)loseFocusHandle);
            }
        #endregion
        #region M:EnsureHandle
        protected void EnsureHandle() {
            if (Handle == null) {
                Handle = new WindowsHookHandle(procedure);
                }
            }
        #endregion
        #region M:EnsureHandle(UInt32)
        protected void EnsureHandle(UInt32 thread) {
            if (Handle == null) {
                Handle = new WindowsHookHandle(procedure, thread);
                }
            }
        #endregion
        #region M:DestroyHandle
        private void DestroyHandle() {
            if (Handle != null) {
                Handle.Close();
                Handle = null;
                }
            }
        #endregion
        #region M:TryDestroyHandle
        protected virtual void TryDestroyHandle() {
            DestroyHandle();
            }
        #endregion

        private delegate IntPtr WindowsHookProc(Int32 code, IntPtr W, IntPtr L);

        private const Int32 HCBT_MOVESIZE = 0;
        private const Int32 HCBT_MINMAX = HCBT_MOVESIZE + 1;
        private const Int32 HCBT_QS = HCBT_MINMAX + 1;
        private const Int32 HCBT_CREATEWND = HCBT_QS + 1;
        private const Int32 HCBT_DESTROYWND = HCBT_CREATEWND + 1;
        private const Int32 HCBT_ACTIVATE = HCBT_DESTROYWND + 1;
        private const Int32 HCBT_CLICKSKIPPED = HCBT_ACTIVATE + 1;
        private const Int32 HCBT_KEYSKIPPED = HCBT_CLICKSKIPPED + 1;
        private const Int32 HCBT_SYSCOMMAND = HCBT_KEYSKIPPED + 1;
        private const Int32 HCBT_SETFOCUS = HCBT_SYSCOMMAND + 1;

        private sealed class WindowsHookHandle : CriticalHandle {
            #region P:IsInvalid:Boolean
            public override Boolean IsInvalid { get {
                return handle == IntPtr.Zero;
                }}
            #endregion
            #region M:WindowsHookHandle(WindowsHookProc)
            public WindowsHookHandle(WindowsHookProc procedure)
                :base(IntPtr.Zero) {
                SetHandle(SetWindowsHookEx(WH_CBT, procedure, IntPtr.Zero, GetCurrentThreadId()));
                }
            #endregion
            #region M:WindowsHookHandle(WindowsHookProc)
            public WindowsHookHandle(WindowsHookProc procedure, UInt32 thread)
                :base(IntPtr.Zero) {
                SetHandle(SetWindowsHookEx(WH_CBT, procedure, IntPtr.Zero, thread));
                }
            #endregion
            #region M:ReleaseHandle:Boolean
            protected override Boolean ReleaseHandle() {
                return UnhookWindowsHookEx(handle);
                }
            #endregion
            #region M:CallNext(Int32,IntPtr,IntPtr):IntPtr
            public IntPtr CallNext(Int32 code, IntPtr W, IntPtr L) {
                return CallNextHookEx(handle, code, W, L);
                }
            #endregion

            [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr SetWindowsHookEx(Int32 type, WindowsHookProc procedure, IntPtr module, UInt32 thread);
            [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, Int32 code, IntPtr wParam, IntPtr lParam);
            [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] private static extern Boolean UnhookWindowsHookEx(IntPtr hhk);
            [DllImport("kernel32.dll")] private static extern UInt32 GetCurrentThreadId();
            private const Int32 WH_CBT = 5;
            }
        }
    }