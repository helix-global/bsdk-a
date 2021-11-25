using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.PortableExecutable
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("66F5C4DC-3CB2-4C6C-B10F-BAE1D309C87A")]
    internal interface IOpcodeDecoder
        {
        void Disassemble(IntPtr Offset, out IOpcode Opcode);
        }

    public class OpcodeDecoder
        {
        private IntPtr library;
        private IOpcodeDecoder decoder;
        private DynamicMethod<DCreateOpcodeDecoder> FCreateOpcodeDecoder;

        public OpcodeDecoder()
            {
            var filename = (IntPtr.Size == 4)
                ? "intrcp32.dll"
                : "intrcp64.dll";
            var executingassembly = Assembly.GetExecutingAssembly();
            var location = executingassembly.Location;
            if (String.IsNullOrEmpty(location))
                {
                location = AppDomain.CurrentDomain.BaseDirectory;
                }

            var module = ReplaceFileName(location, filename);
            if (!File.Exists(module))
                {
                location = (new Uri(GetCodeBase(executingassembly), UriKind.RelativeOrAbsolute)).AbsolutePath;
                module = ReplaceFileName(Uri.UnescapeDataString(location), filename);
                if (!File.Exists(module))
                    {
                    throw new FileNotFoundException($"Library \"{filename}\" not found.");
                    }
                }

            library = LoadLibraryExW(module, IntPtr.Zero, LOAD_IGNORE_CODE_AUTHZ_LEVEL | LOAD_WITH_ALTERED_SEARCH_PATH);
            if (library == IntPtr.Zero)
                {
                throw new Win32Exception();
                }

            FCreateOpcodeDecoder = new DynamicMethod<DCreateOpcodeDecoder>(library, "CreateOpcodeDecoder");
            FCreateOpcodeDecoder.Method.Invoke(out decoder);
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

        /// <summary>
        /// Disassembles a processor instruction in the target's memory.
        /// </summary>
        /// <param name="BeginOffset">Specifies the location in the target's memory of the instruction to disassemble.</param>
        /// <param name="DisassemblySize">Receives the size, in characters, of the disassembled instruction.</param>
        /// <param name="EndOffset">Receives the location in the target's memory of the instruction following the disassembled instruction.</param>
        /// <returns></returns>
        public unsafe String Disassemble(IntPtr BeginOffset, out Int32 DisassemblySize, out IntPtr EndOffset) {
            return Disassemble((Byte*)BeginOffset, out DisassemblySize, out EndOffset);
            }

        private unsafe String Disassemble(Byte* BeginOffset, out Int32 DisassemblySize, out IntPtr EndOffset) {
            decoder.Disassemble((IntPtr)BeginOffset, out var opcode);
            EndOffset = (IntPtr)(BeginOffset + opcode.Size);
            DisassemblySize = opcode.Size;
            if (Environment.Is64BitProcess) {
                var HighOffset = ((UInt64)BeginOffset) >> 32;
                var LowOffset  = ((UInt64)BeginOffset) & 0xFFFFFFFF;
                var r = new StringBuilder();
                r.AppendFormat("{0:x8}`{1:x8} ", HighOffset, LowOffset);
                for (var i = 0; i < opcode.Size; i++) {
                    r.AppendFormat("{0:x2}", BeginOffset[i]);
                    }
                r.Append(new String(' ', Math.Max(1, 16-opcode.Size*2)));
                r.AppendFormat("{0,-7}", opcode.Instruction);
                r.Append(' ');
                var operands = new List<String>();
                for (var i = 0; i < opcode.Operands.Count; i++) {
                    operands.Add(opcode.Operands[i].ToString());
                    }
                r.Append(String.Join(",", operands));
                if (!String.IsNullOrWhiteSpace(opcode.Postfix))
                    {
                    r.AppendFormat(" {0}", opcode.Postfix);
                    }
                return r.ToString().Trim();
                }
            return "";
            }

        private abstract class DynamicMethod
            {
            protected String MethodName { get; }
            protected IntPtr Module { get; }
            protected DynamicMethod(IntPtr module, String methodname)
                {
                if (methodname == null) { throw new ArgumentNullException(nameof(methodname)); }
                MethodName = methodname;
                Module = module;
                }
            }

        private class DynamicMethod<T> : DynamicMethod
            {
            private Delegate mi;
            public DynamicMethod(IntPtr module, String methodname)
                : base(module, methodname)
                {
                }

            public T Method
                {
                get
                    {
                    if (mi == null)
                        {
                        mi = Marshal.GetDelegateForFunctionPointer(GetProcAddress(Module, MethodName), typeof(T));
                        }
                    return (T)(Object)mi;
                    }
                }
            }

        private static String ReplaceFileName(String source, String filename)
            {
            if (String.IsNullOrEmpty(source)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (String.IsNullOrEmpty(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            source = source.Replace('/', '\\');
            for (var i = source.Length - 1; i >= 0; i--)
                {
                if (source[i] == '\\')
                    {
                    return source.Substring(0, i + 1) + filename;
                    }
                }
            return filename;
            }

        private static String GetCodeBase(Assembly assembly)
            {
            return assembly.CodeBase;
            }


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)] private static extern IntPtr LoadLibraryExW([In] string lpwLibFileName, [In] IntPtr hFile, [In] uint dwFlags);
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi)] private static extern IntPtr GetProcAddress(IntPtr module, String procedure);

        const UInt32 LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;
        const UInt32 LOAD_IGNORE_CODE_AUTHZ_LEVEL  = 0x00000010;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DCreateOpcodeDecoder([Out] out IOpcodeDecoder Decoder);
        }
    }