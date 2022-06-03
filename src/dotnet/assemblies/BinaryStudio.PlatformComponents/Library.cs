using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BinaryStudio.PlatformComponents
    {
    public class Library : IDisposable
        {
        protected IDictionary<String, IntPtr> Entries { get; }
        protected SharedObject SharedObject { get;private set; }

        protected virtual String FilePath { get; }
        public  String FileName { get; }

        #region M:Dispose<T>([Ref]T)
        protected void Dispose<T>(ref T o)
            where T: class, IDisposable
            {
            if (o != null) {
                o.Dispose();
                o = null;
                }
            }
        #endregion
        #region M:Dispose(Boolean)
        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(Boolean disposing) {
            if (disposing) {
                }
            }
        #endregion
        #region M:Dispose
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~Library()
            {
            Dispose(false);
            }
        #endregion

        protected Library()
            {
            Entries = new Dictionary<String, IntPtr>();
            }

        protected Library(String filepath)
            :this()
            {
            if (filepath == null) { throw new ArgumentNullException(nameof(filepath)); }
            #if NET35
            if (String.IsNullOrEmpty(filepath)) { throw new ArgumentOutOfRangeException(nameof(filepath)); }
            #else
            if (String.IsNullOrWhiteSpace(filepath)) { throw new ArgumentOutOfRangeException(nameof(filepath)); }
            #endif
            FilePath = filepath;
            FileName = Path.GetFileName(FilePath);
            }

        [DllImport("version.dll",  CharSet = CharSet.Auto,    SetLastError = true)] private static extern Int32 GetFileVersionInfoSize(String filename, out IntPtr handle);
        [DllImport("version.dll",  CharSet = CharSet.Auto,    SetLastError = true)] private static extern Boolean GetFileVersionInfo(String filename, IntPtr handle, Int32 length, [MarshalAs(UnmanagedType.LPArray)] Byte[] data);
        [DllImport("version.dll",  CharSet = CharSet.Auto,    SetLastError = true)] private static extern unsafe Boolean VerQueryValue([MarshalAs(UnmanagedType.LPArray)] Byte[] block, String query, out VS_FIXEDFILEINFO* fileinfo, out Int32 size);

        #region M:EnsureCore
        protected virtual void EnsureCore() {
            if (SharedObject == null) {
                SharedObject = SharedObject.Create(FilePath);
                }
            }
        #endregion
        #region M:EnsureProcedure<T>(String,[Ref]T):T
        protected T EnsureProcedure<T>(String name, ref T value) {
            if (value == null) {
                lock(this) {
                    EnsureCore();
                    if (!Entries.TryGetValue(name, out var r)) {
                        r = SharedObject.Get(name);
                        Entries.Add(name, r);
                        }
                    value = (T)(Object)Marshal.GetDelegateForFunctionPointer(r, typeof(T));
                    }
                }
            return value;
            }
        #endregion
        #region M:DefineDynamicAssembly(AssemblyName,AssemblyBuilderAccess):AssemblyBuilder
        private static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access) {
            var r = typeof(AssemblyBuilder).GetMethod(
                "DefineDynamicAssembly", BindingFlags.Public|BindingFlags.Static, null, CallingConventions.Any,
                new Type[] { typeof(AssemblyName), typeof(AssemblyBuilderAccess) }, null);
            if (r == null) { throw new MissingMethodException(); }
            return (AssemblyBuilder)r.Invoke(null, new Object[]
                {
                name,
                access
                });
            }
        #endregion
        #region M:BuildField(TypeBuilder,Type,String):FieldBuilder
        private static FieldBuilder BuildField(TypeBuilder target, Type @delegate,String index)
            {
            var r = target.DefineField(
                $"{{{index}}}",
                @delegate,
                FieldAttributes.Private);
            var attribute = new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute).GetConstructor(new Type[]{ typeof(DebuggerBrowsableState)}), new Object[]{ DebuggerBrowsableState.Never });
            r.SetCustomAttribute(attribute);
            return r;
            }
        #endregion
        #region M:DefineMethod(TypeBuilder,String,MethodAttributes,Type,Type[],MethodImplAttributes):MethodBuilder
        private static MethodBuilder DefineMethod(TypeBuilder target, String name, MethodAttributes attributes, Type returnType, IEnumerable<Type> parameterTypes,MethodImplAttributes implAttributes)
            {
            var r = target.DefineMethod(name, attributes, returnType, parameterTypes.ToArray());
            r.SetImplementationFlags(implAttributes);
            return r;
            }
        #endregion
        #region M:BuildConstructor(TypeBuilder)
        private static void BuildConstructor(TypeBuilder type)
            {
            var ctor = type.DefineConstructor(
                MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.HasThis, new Type[]{ typeof(String) });
            ctor.DefineParameter(1, ParameterAttributes.None, "filepath");
            ctor.SetImplementationFlags(MethodImplAttributes.Managed);
            ctor.InitLocals = true;
            var gtor = ctor.GetILGenerator();
            gtor.BeginScope();
            //gtor.DeclareLocal(null);
            gtor.Emit(OpCodes.Ldarg_0);
            gtor.Emit(OpCodes.Ldarg_1);
            gtor.Emit(OpCodes.Call,typeof(Library).GetConstructor(BindingFlags.NonPublic|BindingFlags.Instance, null, CallingConventions.Any, new Type[]{ typeof(String) }, null));
            gtor.EndScope();
            gtor.Emit(OpCodes.Ret);
            }
        #endregion
        #region M:BuildMethod(TypeBuilder,MethodInfo,FieldBuilder)
        private static void BuildMethod(TypeBuilder type, MethodInfo source, FieldBuilder field, MethodBuilder invoke) {
            var parameters = source.GetParameters();
            var r = type.DefineMethod(
                $"{source.DeclaringType.FullName}.{source.Name}",
                MethodAttributes.HideBySig|MethodAttributes.Private|MethodAttributes.Final|MethodAttributes.Virtual|MethodAttributes.NewSlot);
            r.SetSignature(source.ReturnType, null, null, parameters.Select(i => i.ParameterType).ToArray(), null, null);
            r.DefineParameter(0, ParameterAttributes.Retval, String.Empty);
            for (var i = 0; i < parameters.Length; i++) {
                r.DefineParameter(i + 1,
                    parameters[i].Attributes,
                    parameters[i].Name);
                }
            var gtor = r.GetILGenerator();
            var mi = typeof(Library).GetMethod(nameof(EnsureProcedure), BindingFlags.Instance|BindingFlags.NonPublic);
            mi = mi.MakeGenericMethod(field.FieldType);
            gtor.DeclareLocal(source.ReturnType);
            gtor.Emit(OpCodes.Ldarg_0);
            gtor.Emit(OpCodes.Ldstr, source.Name);
            gtor.Emit(OpCodes.Ldarg_0);
            gtor.Emit(OpCodes.Ldflda, field);
            gtor.Emit(OpCodes.Call, mi);
            gtor.Emit(OpCodes.Ldarg_0);
            gtor.Emit(OpCodes.Ldfld, field);
            for (var i = 0; i < parameters.Length; i++) {
                switch (i)
                    {
                    case 0: { gtor.Emit(OpCodes.Ldarg_1); break; }
                    case 1: { gtor.Emit(OpCodes.Ldarg_2); break; }
                    case 2: { gtor.Emit(OpCodes.Ldarg_3); break; }
                    default:
                        {
                        gtor.Emit(OpCodes.Ldarg_S, i + 1);
                        }
                        break;
                    }
                }
            gtor.Emit(OpCodes.Callvirt, invoke);
            gtor.Emit(OpCodes.Stloc_0);
            var L = gtor.DefineLabel();
            gtor.Emit(OpCodes.Br_S, L);
            gtor.MarkLabel(L);
            gtor.Emit(OpCodes.Ldloc_0);
            gtor.Emit(OpCodes.Ret);
            type.DefineMethodOverride(r, source);
            }
        #endregion
        #region M:BuildDelegate(TypeBuilder,String,MethodInfo,MethodBuilder):TypeBuilder
        private static TypeBuilder BuildDelegate(TypeBuilder type, String index, MethodInfo source, out MethodBuilder invoke)
            {
            var parameters = source.GetParameters();
            var types = parameters.Select(i => i.ParameterType).ToArray();
            var r = type.DefineNestedType($"{{{index}}}",
                TypeAttributes.NestedPrivate|TypeAttributes.Sealed|TypeAttributes.AnsiClass,
                typeof(MulticastDelegate), null);
            #region Constructor
            r.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.UnmanagedExport, CallingConventions.Standard,
                new Type[]
                    {
                    typeof(Object),
                    typeof(IntPtr)
                    }).
            SetImplementationFlags(MethodImplAttributes.Runtime|MethodImplAttributes.Managed);
            #endregion
            #region BeginInvoke
            var mi = DefineMethod(r, "BeginInvoke",
                MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot |
                MethodAttributes.Virtual,
                typeof(IAsyncResult),
                Merge(types, typeof(AsyncCallback), typeof(Object)),
                MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
            var j = 1;
            for (var i = 0; i < parameters.Length; i++) {
                mi.DefineParameter(j,
                    parameters[i].Attributes,
                    parameters[i].Name);
                j++;
                }
            mi.DefineParameter(j, ParameterAttributes.In, "callback");
            mi.DefineParameter(j + 1, ParameterAttributes.In, "object");
            #endregion
            #region EndInvoke
            mi = DefineMethod(r, "EndInvoke",
                MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot |
                MethodAttributes.Virtual,
                source.ReturnType, Merge(parameters.Where(i => i.IsOut).Select(i => i.ParameterType), typeof(IAsyncResult)),
                MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
            j = 1;
            for (var i = 0; i < parameters.Length; i++) {
                if (parameters[i].IsOut) {
                    mi.DefineParameter(j,
                        parameters[i].Attributes,
                        parameters[i].Name);
                    j++;
                    }
                }
            mi.DefineParameter(j, ParameterAttributes.In, "result");
            #endregion
            #region Invoke
            invoke = mi = DefineMethod(r, "Invoke",
                MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot |
                MethodAttributes.Virtual,
                source.ReturnType, types,
                MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
            for (var i = 0; i < parameters.Length; i++) {
                mi.DefineParameter(i + 1,
                    parameters[i].Attributes,
                    parameters[i].Name);
                }
            #endregion
            r.CreateType();
            return r;
            }
        #endregion
        #region M:Merge(IEnumerable<Type>,Type[]):IEnumerable<Type>
        private static IEnumerable<Type> Merge(IEnumerable<Type> x, params Type[] y)
            {
            foreach (var i in x) { yield return i; }
            foreach (var i in y) { yield return i; }
            }
        #endregion
        #region M:LoadLibrary<T>(String):T
        public static T LoadLibrary<T>(String filepath)
            {
            if (filepath == null) { throw new ArgumentNullException(nameof(filepath)); }
            #if NET35
            if (String.IsNullOrEmpty(filepath)) { throw new ArgumentOutOfRangeException(nameof(filepath)); }
            #else
            if (String.IsNullOrWhiteSpace(filepath)) { throw new ArgumentOutOfRangeException(nameof(filepath)); }
            #endif
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var type = typeof(T);
            var assemblyname = new AssemblyName($"{{{filename}}}"){ Version = GetVersion(filepath) };
            var assembly = DefineDynamicAssembly(assemblyname, AssemblyBuilderAccess.Run);
            //var module = assembly.DefineDynamicModule($"{assemblyname.Name}", $"{assemblyname.Name}.dll");
            var module = assembly.DefineDynamicModule($"{assemblyname.Name}");
            var target = module.DefineType("{Library}", TypeAttributes.Public|TypeAttributes.BeforeFieldInit, typeof(Library), new Type[]{ type });
            BuildConstructor(target);
            var methods = type.GetMethods();
            var format = "{0:D" + ((Int32)Math.Ceiling(Math.Log10(methods.Length)) + 1) + "}";
            for (var i = 0; i < methods.Length; i++)
                {
                var n = String.Format(format, i);
                var d =
                BuildDelegate(target, n , methods[i], out var invoke);
                BuildMethod(target, methods[i],
                BuildField(target, d, n), invoke);
                }
            var r = target.CreateType();
            var ctor = r.GetConstructor(new Type[]{ typeof(String)});
            if (ctor == null) { throw new InvalidOperationException(); }
            return (T)ctor.Invoke(new Object[]{ filepath });
            }
        #endregion
        #region M:GetVersion(String)
        public static unsafe Version GetVersion(String filepath) {
            switch (Environment.OSVersion.Platform)
                {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    {
                    var sz = GetFileVersionInfoSize(filepath, out var handle);
                    if (sz > 0) {
                        var block = new Byte[sz];
                        if (GetFileVersionInfo(filepath, handle, sz, block)) {
                            if (VerQueryValue(block, "\\", out var r, out sz)) {
                                return new Version(
                                    (Int32)((r->FileVersionMS & 0xFFFF0000) >> 16),
                                    (Int32)((r->FileVersionMS & 0x0000FFFF)),
                                    (Int32)((r->FileVersionLS & 0xFFFF0000) >> 16),
                                    (Int32)((r->FileVersionLS & 0x0000FFFF)));
                                }
                            }
                        }
                    }
                    break;
                }
            return new Version(0,0,0,0);
            }
        #endregion
        }

    public interface ICryptoAPIProvider2
        {
        [Import] Boolean CryptAcquireContextA([Out] out IntPtr hCryptProv, [MarshalAs(UnmanagedType.LPStr)] [In] String pszContainer, [MarshalAs(UnmanagedType.LPStr)] [In] String pszProvider, [In] Int32 dwProvType, out Int32 dwFlags);
        }

    public class X : Library, ICloneable, ICancelAddNew, ICryptoAPIProvider2
    {
        public X(String filename)
            : base(filename)
        {
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        Object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>Discards a pending new item from the collection.</summary>
        /// <param name="itemIndex">The index of the item that was previously added to the collection. </param>
        void ICancelAddNew.CancelNew(Int32 itemIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>Commits a pending new item to the collection.</summary>
        /// <param name="itemIndex">The index of the item that was previously added to the collection. </param>
        void ICancelAddNew.EndNew(Int32 itemIndex)
        {
            throw new NotImplementedException();
        }

        delegate Boolean CryptAcquireContextA_1(out IntPtr hCryptProv, String pszContainer, String pszProvider, Int32 dwProvType,out Int32 dwFlags);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] CryptAcquireContextA_1 f1;
        Boolean ICryptoAPIProvider2.CryptAcquireContextA(out IntPtr hCryptProv, String pszContainer, String pszProvider, Int32 dwProvType,
            out Int32 dwFlags)
        {
            EnsureProcedure("!!!!", ref f1);
            return f1(out hCryptProv, pszContainer, pszProvider, dwProvType, out dwFlags);
        }
    }
    }
