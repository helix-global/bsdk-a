using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    public class DebugClient : IDisposable
        {
        /// <summary>
        /// The DebugCreate function creates a new client object and returns an interface pointer to it.
        /// </summary>
        /// <param name="InterfaceId">Specifies the interface identifier (IID) of the desired debugger engine client interface. This is the type of the interface that will be returned in Interface.</param>
        /// <param name="client">Receives an interface pointer for the new client. The type of this interface is specified by InterfaceId.</param>
        [DllImport("dbgeng.dll", EntryPoint = "DebugCreate", SetLastError = false, CallingConvention = CallingConvention.StdCall, PreserveSig = false)]
        private static extern void DebugCreate([In][MarshalAs(UnmanagedType.LPStruct)]Guid InterfaceId, out IDebugClient client);

        /// <summary>
        /// The DebugCreateEx function creates a new client object and returns an interface pointer to it.
        /// </summary>
        /// <param name="InterfaceId">Specifies the interface identifier (IID) of the desired debugger engine client interface. This is the type of the interface that will be returned in Interface.</param>
        /// <param name="DbgEngOptions">Supplies debugger option flags.</param>
        /// <param name="client">Receives an interface pointer for the new client. The type of this interface is specified by InterfaceId.</param>
        [DllImport("dbgeng.dll", EntryPoint = "DebugCreateEx", SetLastError = false, CallingConvention = CallingConvention.StdCall, PreserveSig = false)]
        private static extern void DebugCreateEx([In][MarshalAs(UnmanagedType.LPStruct)]Guid InterfaceId, UInt32 DbgEngOptions, out IDebugClient client);
        [DllImport("kernel32.dll")] private static extern Int32 GetProcessId(IntPtr handle);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();

        public static IDebugClient DebugCreate() {
            DebugCreate(typeof(IDebugClient).GUID, out var client);
            return client;
            }

        public DebugClient(Int32 process) {
            client = DebugCreate();
            control = client as IDebugControl7;
            if (client == null) { throw new InvalidOperationException(); }
            client.AttachProcess(0,
                (process == -1)
                    ? GetProcessId(GetCurrentProcess())
                    : process, DEBUG_ATTACH.DEBUG_ATTACH_NONINVASIVE | DEBUG_ATTACH.DEBUG_ATTACH_NONINVASIVE_NO_SUSPEND);
            }

        /// <summary>
        /// Disassembles a processor instruction in the target's memory.
        /// </summary>
        /// <param name="BeginOffset">Specifies the location in the target's memory of the instruction to disassemble.</param>
        /// <param name="DisassemblySize">Receives the size, in characters, of the disassembled instruction.</param>
        /// <param name="EndOffset">Receives the location in the target's memory of the instruction following the disassembled instruction.</param>
        /// <returns></returns>
        public String Disassemble(IntPtr BeginOffset, out Int32 DisassemblySize, out IntPtr EndOffset) {
            if (first) {
                control.WaitForEvent(0, -1);
                first = false;
                }
            var buffer = new StringBuilder(256);
            control.Disassemble((Int64)BeginOffset, 0, buffer, buffer.Capacity, out DisassemblySize, out var EndOffset64);
            EndOffset = (IntPtr)EndOffset64;
            var r = buffer.ToString().TrimEnd('\n');
            Debug.Print($"{r}");
            Match m;
            if ((m = Regex.Match(r, @"(^.+)[[](.+\p{Z})[(]([A-Fa-f0-9]{8}[`][A-Fa-f0-9]{8})[)][]]$")).Success)      { return $"{m.Groups[1].Value}[{m.Groups[3].Value}]";                    }
            if ((m = Regex.Match(r, @"(^.+)\p{Z}(\P{Z}+\p{Z})[(]([A-Fa-f0-9]{8}[`][A-Fa-f0-9]{8})[)]$")).Success)   { return $"{m.Groups[1].Value} {m.Groups[3].Value}";                     }
            if ((m = Regex.Match(r, @"(^.+)[[](.+\p{Z})[(]([A-Fa-f0-9]{8}[`][A-Fa-f0-9]{8})[)][]](,.+$)")).Success) { return $"{m.Groups[1].Value}[{m.Groups[3].Value}]{m.Groups[4].Value}"; }
            return r;
            }

        /// <summary>
        /// Disassembles a processor instruction in the target's memory.
        /// </summary>
        /// <param name="BeginOffset">Specifies the location in the target's memory of the instruction to disassemble.</param>
        /// <param name="EndOffset">Receives the location in the target's memory of the instruction following the disassembled instruction.</param>
        /// <returns></returns>
        public String Disassemble(IntPtr BeginOffset, out IntPtr EndOffset) {
            return Disassemble(BeginOffset, out var DisassemblySize, out EndOffset);
            }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            client = null;
            control = null;
            }
        #endregion
        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~DebugClient()
            {
            Dispose(false);
            }
        #endregion

        private IDebugClient client;
        private IDebugControl7 control;
        private Boolean first = true;
        }
    }