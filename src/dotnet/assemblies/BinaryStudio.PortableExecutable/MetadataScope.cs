using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using BinaryStudio.IO;
using BinaryStudio.PortableExecutable.ProgramDatabase;
using BinaryStudio.PortableExecutable.TypeLibrary;
using BinaryStudio.PortableExecutable.Win32;
using Microsoft.Win32;
using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;

namespace BinaryStudio.PortableExecutable
    {
    public class MetadataScope : IDisposable
        {
        private const UInt16 IMAGE_DOS_SIGNATURE = 0x5A4D;
        private const UInt32 MSFT_SIGNATURE = 0x5446534D;
        private const UInt32 SLTG_SIGNATURE = 0x47544c53;

        private readonly IDictionary<String, ITypeLibraryTypeDescriptor> types = new ConcurrentDictionary<String, ITypeLibraryTypeDescriptor>();
        private readonly IDictionary<String, ITypeLibraryDescriptor> nlibs = new ConcurrentDictionary<String, ITypeLibraryDescriptor>(StringComparer.OrdinalIgnoreCase);
        private readonly IDictionary<TypeLibraryIdentifier, ITypeLibraryDescriptor> ilibs = new ConcurrentDictionary<TypeLibraryIdentifier, ITypeLibraryDescriptor>();
        private readonly IDictionary<String, ISet<Tuple<String,Version,Guid,CultureInfo,ITypeLibraryTypeDescriptor>>> indexes = new ConcurrentDictionary<String, ISet<Tuple<String, Version, Guid,CultureInfo,ITypeLibraryTypeDescriptor>>>();

        private HashSet<String> SearchPath { get; }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (disposing) {
                }
            }
        #endregion-
        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~MetadataScope() {
            Dispose(false);
            }
        #endregion

        private class RegistryPair
            {
            public RegistryKey Key { get; }
            public String Name { get; }
            public RegistryPair(RegistryKey key, String name)
                {
                Key = key;
                Name = name;
                }
            }

        private static IEnumerable<String> EnumerateFiles(String path, String searchpattern) {
            IEnumerable<String> files;
            IEnumerable<String> directories;
            try
                {
                files = Directory.EnumerateFiles(path, searchpattern);
                }
            catch
                {
                files = EmptyArray<String>.Value;
                }
            foreach (var file in files) {
                yield return file;
                yield break;
                }
            try
                {
                directories = Directory.EnumerateDirectories(path);
                }
            catch
                {
                directories = EmptyArray<String>.Value;
                }
            foreach (var directory in directories) {
                foreach (var file in EnumerateFiles(directory, searchpattern)) {
                    yield return file;
                    yield break;
                    }
                }
            }

        public ITypeLibraryDescriptor LoadTypeLibrary(Guid g)
            {
            return null;
            }

        public ITypeLibraryDescriptor LoadTypeLibrary(Guid g, Version v, SYSKIND o)
            {
            var key = new TypeLibraryIdentifier(g,v,o);
            if (ilibs.TryGetValue(key, out var r)) { return r; }
            var itarget = (o == SYSKIND.SYS_WIN64)
                ? "win64"
                : "win32";
            var typelib = Registry.ClassesRoot.OpenSubKey($@"TypeLib\{g:B}");
            foreach (var version in typelib.GetSubKeyNames()) {
                if ((v == null) || (v.ToString() == version)) {
                    var versionkey = typelib.OpenSubKey(version);
                    if (versionkey != null) {
                        foreach (var e in versionkey.GetSubKeyNames()) {
                            if (e == "0") {
                                var entrykey = versionkey.OpenSubKey(e);
                                if (entrykey != null) {
                                    foreach (var target in entrykey.GetSubKeyNames()) {
                                        if (String.Equals(itarget, target, StringComparison.OrdinalIgnoreCase)) {
                                            var targetkey = entrykey.OpenSubKey(target);
                                            if (targetkey != null) {
                                                var libpath = targetkey.GetValue(null).ToString();
                                                if (!String.IsNullOrWhiteSpace(libpath)) {
                                                    return LoadTypeLibrary(libpath, o);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            throw new ArgumentOutOfRangeException(nameof(g));
            }

        #region M:LoadTypeLibrary(String):ITypeLibraryDescriptor
        public ITypeLibraryDescriptor LoadTypeLibrary(String key) {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            if (String.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key)); }
            return LoadTypeLibrary(key, (IntPtr.Size == 8)
                ? SYSKIND.SYS_WIN64
                : SYSKIND.SYS_WIN32);
            }
        #endregion

        internal ITypeLibraryDescriptor LoadTypeLibrary(String key, SYSKIND o) {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            try
                {
                if (!nlibs.TryGetValue(key, out var r)) {
                    if (key.StartsWith("*\\G")) {
                        var items = key.Split(new []{"#"}, StringSplitOptions.RemoveEmptyEntries);
                        var n = Path.GetFileName(items[3]);
                        r = LoadTypeLibrary(
                            Guid.Parse(items[0].Substring(3)),
                            Version.Parse(items[1]), o);
                        if (r != null) {
                            try
                                {
                                nlibs.Add(key, r);
                                ilibs.Add(r.Identifier, r);
                                return r;
                                }
                            finally
                                {
                                }
                            }
                        else
                            {
                            throw new NotImplementedException();
                            }
                        }
                    else
                        {
                        using (new DisableWow64FileSystemRedirection()) {
                            var n = key;
                            if (!File.Exists(key)) {
                                n = null;
                                key = Path.GetFileName(key);
                                var mscore = Path.GetDirectoryName(typeof(Type).Assembly.Location);
                                foreach (var dir in (Environment.GetFolderPath(Environment.SpecialFolder.System) + ";" + mscore + ";" + Environment.GetEnvironmentVariable("PATH")).Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries)) {
                                    if (Directory.Exists(dir)) {
                                        var files = Directory.EnumerateFiles(dir, key, SearchOption.TopDirectoryOnly).ToArray();
                                        if (files.Length > 0) {
                                            n = files[0];
                                            break;
                                            }
                                        }
                                    }
                                foreach (var dir in SearchPath) {
                                    if (Directory.Exists(dir)) {
                                        var files = EnumerateFiles(dir, key).ToArray();
                                        if (files.Length > 0) {
                                            n = files[0];
                                            break;
                                            }
                                        }
                                    }
                                }
                            if (n == null) { throw new ArgumentOutOfRangeException(nameof(key)); }
                            r = (ITypeLibraryDescriptor)LoadObject(n).GetService(typeof(ITypeLibraryDescriptor));
                            return r;
                            }
                        }
                    }
                else
                    {
                    return r;
                    }
                }
            finally
                {
                }
            }

        #region M:ParseSysKind(String):SYSKIND
        private static SYSKIND ParseSysKind(String source) {
            switch (source.ToLower()) {
                case "win16" : { return SYSKIND.SYS_WIN16; }
                case "win32" : { return SYSKIND.SYS_WIN32; }
                case "win64" : { return SYSKIND.SYS_WIN64; }
                case "mac"   : { return SYSKIND.SYS_MAC;   }
                }
            return (SYSKIND)(-1);
            }
        #endregion
        #region M:ParseVersion(String):Version
        private static Version ParseVersion(String source) {
            if (String.IsNullOrEmpty(source)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            var match = Regex.Match(source, "^([0-9A-Fa-f]+)[.]([0-9A-Fa-f]+)$");
            if (match.Success) {
                return new Version(
                    Int32.Parse(match.Groups[1].Value,NumberStyles.HexNumber),
                    Int32.Parse(match.Groups[2].Value,NumberStyles.HexNumber));
                }
            throw new ArgumentOutOfRangeException(nameof(source));
            }
        #endregion
        #region M:EnumerateRegistryKeys(RegistryKey):IEnumerable<RegistryPair>
        private static IEnumerable<RegistryPair> EnumerateRegistryKeys(RegistryKey source) {
            if (source != null) {
                foreach (var i in source.GetSubKeyNames()) {
                    var r = source.OpenSubKey(i);
                    if (r != null) {
                        yield return new RegistryPair(r, i);
                        }
                    }
                }
            }
        #endregion
        #region M:EnumerateRegistryKeys(RegistryKey,String):IEnumerable<RegistryPair>
        private static IEnumerable<RegistryPair> EnumerateRegistryKeys(RegistryKey source, String suffix) {
            if (source != null) {
                foreach (var i in source.GetSubKeyNames()) {
                    var r = source.OpenSubKey($"{i}{suffix}");
                    if (r != null) {
                        yield return new RegistryPair(r, i);
                        }
                    }
                }
            }
        #endregion
        #region M:EnumerateTypeLibraries(RegistryKey):IEnumerable<Tuple<TypeLibraryDescriptiorKey,String>>
        private static IEnumerable<Tuple<TypeLibraryDescriptiorKey,String>> EnumerateTypeLibraries(RegistryKey registry) {
            if (registry != null) {
                foreach (var typelib in EnumerateRegistryKeys(registry)) {
                    foreach (var version in EnumerateRegistryKeys(typelib.Key, "\\0")) {
                        foreach (var syskindname in version.Key.GetSubKeyNames()) {
                            var syskindcode = ParseSysKind(syskindname);
                            if (syskindcode != (SYSKIND)(-1)) {
                                using (var syskind = version.Key.OpenSubKey(syskindname)) {
                                    if (syskind != null) {
                                        var key = new TypeLibraryDescriptiorKey(new Guid(typelib.Name), ParseVersion(version.Name), syskindcode);
                                        var filename = syskind.GetValue(null) as String;
                                        if (!String.IsNullOrEmpty(filename)) {
                                            yield return Tuple.Create(key, filename);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        #endregion

        public IEnumerable<String> EnumerateTypeLibraries() {
            var r = new HashSet<TypeLibraryDescriptiorKey>();
            foreach (var i in EnumerateTypeLibraries(Registry.ClassesRoot.OpenSubKey("TypeLib"))) {
                if (r.Add(i.Item1)) {
                    yield return i.Item2;
                    }
                }
            foreach (var i in EnumerateTypeLibraries(Registry.ClassesRoot.OpenSubKey(@"WOW6432Node\TypeLib"))) {
                if (r.Add(i.Item1)) {
                    yield return i.Item2;
                    }
                }
            }

        public unsafe MetadataObject LoadObject(Byte* source, Int64 size) {
            MetadataObject r = null;
                 if (*((UInt16*)source) == IMAGE_DOS_SIGNATURE) { r = new PortableExecutableSource(this); }
            else if (*((UInt32*)source) == MSFT_SIGNATURE)      { r = new MSFTMetadataTypeLibrary(this, Encoding.GetEncoding(GetACP())); }
            else if (*((UInt32*)source) == SLTG_SIGNATURE)      { r = new SLTGTypeLibrary(this, Encoding.GetEncoding(GetACP())); }
            else
                {
                r = new CommonObjectFileSource(this);
                var a = new List<Exception>();
                if (r.Load(out var e, source, size)) {
                    return r;
                    }
                //r = new MultiStreamFile(this);
                return null;
                }
            if (r != null) {
                if (r.Load(out var e, source, size)) {
                    return r;
                    }
                }
            return r;
            }

        public unsafe MetadataObject LoadObject(String filename) {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            if (String.IsNullOrWhiteSpace(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            using (new DisableWow64FileSystemRedirection()) {
                if (!File.Exists(filename)) { throw new FileNotFoundException(nameof(filename)); }
                using (var mapping = new FileMapping(filename)) {
                    using (var memory  = new FileMappingMemory(mapping)) {
                        var r = LoadObject((Byte*)(void*)memory, mapping.Size);
                        if (r == null) { return null; }
                        var typelib = (ITypeLibraryDescriptor)r.GetService(typeof(ITypeLibraryDescriptor));
                        if (typelib != null)
                            {
                            nlibs.Add(filename, typelib);
                            ilibs.Add(typelib.Identifier, typelib);
                            }
                        return r;
                        }
                    }
                }
            }

        #region M:TypeOf(VARTYPE):ITypeLibraryTypeDescriptor
        public ITypeLibraryTypeDescriptor TypeOf(VARTYPE source) {
            switch (source) {
                case VARTYPE.VT_I2:       { return TypeOf("Int16");    }
                case VARTYPE.VT_I4:       { return TypeOf("Int32");    }
                case VARTYPE.VT_R4:       { return TypeOf("Single");   }
                case VARTYPE.VT_R8:       { return TypeOf("Double");   }
                case VARTYPE.VT_BOOL:     { return TypeOf("Boolean");  }
                case VARTYPE.VT_DECIMAL:  { return TypeOf("Decimal");  }
                case VARTYPE.VT_I1:       { return TypeOf("SByte");    }
                case (VARTYPE)32:
                case VARTYPE.VT_UI1:      { return TypeOf("Byte");     }
                case VARTYPE.VT_UI2:      { return TypeOf("UInt16");   }
                case VARTYPE.VT_UI4:      { return TypeOf("UInt32");   }
                case VARTYPE.VT_I8:       { return TypeOf("Int64");    }
                case VARTYPE.VT_UI8:      { return TypeOf("UInt64");   }
                case VARTYPE.VT_BSTR:     { return TypeOf("String");   }
                case VARTYPE.VT_VOID:     { return TypeOf("Void");     }
                case VARTYPE.VT_INT_PTR:  { return TypeOf("IntPtr");   }
                case VARTYPE.VT_UINT_PTR: { return TypeOf("UIntPtr");  }
                case VARTYPE.VT_INT:      { return TypeOf("Int");      }
                case VARTYPE.VT_UINT:     { return TypeOf("UInt");     }
                case VARTYPE.VT_HRESULT:  { return TypeOf("HRESULT");  }
                case VARTYPE.VT_DATE:     { return TypeOf("DateTime"); }
                case VARTYPE.VT_NULL:     { return TypeOf("Null");     }
                case VARTYPE.VT_CY:       { return TypeOf("Currency"); }
                case VARTYPE.VT_ERROR:    { return TypeOf("SCODE");    }
                case VARTYPE.VT_FILETIME: { return TypeOf("FileTime"); }
                case VARTYPE.VT_VARIANT:  { return TypeOf("VARIANT");  }
                case VARTYPE.VT_LPSTR:    { return new TypeLibraryTypePointer(TypeOf("Char"));  }
                case VARTYPE.VT_LPWSTR:   { return new TypeLibraryTypePointer(TypeOf("WChar")); }
                case VARTYPE.VT_DISPATCH: { return new TypeLibraryTypePointer(TypeOf("IDispatch, stdole, UniqueIdentifier=00020430-0000-0000-c000-000000000046")); }
                case VARTYPE.VT_UNKNOWN:  { return new TypeLibraryTypePointer(TypeOf("IUnknown, stdole, UniqueIdentifier=00020430-0000-0000-c000-000000000046"));  }
                case VARTYPE.VT_EMPTY:    break;
                case VARTYPE.VT_PTR: break;
                case VARTYPE.VT_SAFEARRAY: break;
                case VARTYPE.VT_CARRAY: break;
                case VARTYPE.VT_USERDEFINED: break;
                case VARTYPE.VT_RECORD: break;
                case VARTYPE.VT_BLOB: break;
                case VARTYPE.VT_STREAM: break;
                case VARTYPE.VT_STORAGE: break;
                case VARTYPE.VT_STREAMED_OBJECT: break;
                case VARTYPE.VT_STORED_OBJECT: break;
                case VARTYPE.VT_BLOB_OBJECT: break;
                case VARTYPE.VT_CF: break;
                case VARTYPE.VT_CLSID: break;
                case VARTYPE.VT_VERSIONED_STREAM: break;
                case VARTYPE.VT_BSTR_BLOB: break;
                case VARTYPE.VT_VECTOR: break;
                case VARTYPE.VT_ARRAY: break;
                case VARTYPE.VT_BYREF: break;
                case VARTYPE.VT_RESERVED: break;
                case VARTYPE.VT_ILLEGAL: break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
                }
            throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        #endregion
        #region M:TypeOf(String):ITypeLibraryTypeDescriptor
        public ITypeLibraryTypeDescriptor TypeOf(String source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (String.IsNullOrWhiteSpace(source)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            ITypeLibraryTypeDescriptor r = null;
            source = source.Trim();
            if (!types.TryGetValue(source, out r)) {
                var items = source.Split(new []{", "}, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length > 0) {
                    if (items.Length == 1) {
                        switch (items[0]) {
                            case "Int16"    : { r = new TypeLibraryPrimitiveType(typeof(Int16),   "short");              } break;
                            case "Int32"    : { r = new TypeLibraryPrimitiveType(typeof(Int32),   "long");               } break;
                            case "Int64"    : { r = new TypeLibraryPrimitiveType(typeof(Int64),   "long long");          } break;
                            case "UInt16"   : { r = new TypeLibraryPrimitiveType(typeof(UInt16),  "unsigned short");     } break;
                            case "UInt32"   : { r = new TypeLibraryPrimitiveType(typeof(UInt32),  "unsigned long");      } break;
                            case "UInt64"   : { r = new TypeLibraryPrimitiveType(typeof(UInt64),  "unsigned long long"); } break;
                            case "IntPtr"   : { r = new TypeLibraryPrimitiveType(typeof(IntPtr),  "intptr");             } break;
                            case "UIntPtr"  : { r = new TypeLibraryPrimitiveType(typeof(UIntPtr), "unsigned intptr");    } break;
                            case "SByte"    : { r = new TypeLibraryPrimitiveType(typeof(SByte),   "char");               } break;
                            case "Byte"     : { r = new TypeLibraryPrimitiveType(typeof(Byte),    "byte");               } break;
                            case "Single"   : { r = new TypeLibraryPrimitiveType(typeof(Single),  "float");              } break;
                            case "Double"   : { r = new TypeLibraryPrimitiveType(typeof(Double),  "double");             } break;
                            case "Decimal"  : { r = new TypeLibraryPrimitiveType(typeof(Decimal), "decimal");            } break;
                            case "Boolean"  : { r = new TypeLibraryPrimitiveType(typeof(Boolean), "bool");               } break;
                            case "String"   : { r = new TypeLibraryPrimitiveType(typeof(String),  "string");             } break;
                            case "Null"     : { r = new TypeLibraryPrimitiveType(typeof(DBNull),  "{nil}");              } break;
                            case "Void"     : { r = new TypeLibraryPrimitiveType(typeof(void),    "void");               } break;
                            case "WChar"    : { r = new TypeLibraryPrimitiveType(typeof(Char),    "wchar");              } break;
                            case "Char"     : { r = new TypeLibraryPrimitiveType(typeof(Char),    "char");               } break;
                            case "VARIANT"  : { r = new TypeLibraryPrimitiveType(typeof(VARIANT), "VARIANT");            } break;
                            case "SCODE"    : { r = new TypeLibraryAliasType("scode", TypeOf("Int32"));   } break;
                            case "DateTime" : { r = new TypeLibraryPrimitiveType(typeof(DateTime));     } break;
                            case "Int"      : { r = new TypeLibraryAliasType("int",          TypeOf("Int32"));   } break;
                            case "UInt"     : { r = new TypeLibraryAliasType("unsigned int", TypeOf("UInt32"));  } break;
                            case "HRESULT"  : { r = new TypeLibraryAliasType("HRESULT",  TypeOf("UInt32"));  } break;
                            case "Currency" : { r = new TypeLibraryAliasType("currency", TypeOf("Decimal")); } break;
                            }
                        }
                    else
                        {
                        var typename = items[0];
                        String      n = null;
                        Guid?       g = null;
                        CultureInfo c = null;
                        Version     v = null;
                        for (var i = 1; i < items.Length; ++i) {
                                 if (items[i].StartsWith("Version="))          { v = Version.Parse(items[i].Substring(8)); }
                            else if (items[i].StartsWith("Culture="))          { c = CultureInfo.GetCultureInfo(items[i].Substring(8)); }
                            else if (items[i].StartsWith("UniqueIdentifier=")) { g = Guid.Parse(items[i].Substring(17)); }
                            else { n = items[i]; }
                            }
                        ISet<Tuple<String,Version,Guid,CultureInfo,ITypeLibraryTypeDescriptor>> index;
                        if (indexes.TryGetValue(typename, out index)) {
                            var values = index.Where(i => i.Item3 == g).OrderBy(i => i.Item2).ToArray();
                            if (v != null) { values = values.Where(i => i.Item2 == v).ToArray(); }
                            r = values.Select(i => i.Item5).FirstOrDefault();
                            }
                        if (r == null) {
                            var library = LoadTypeLibrary(g.GetValueOrDefault(), v, SYSKIND.SYS_WIN32);
                            if (library != null) {
                                r = library.DefinedTypes.FirstOrDefault(i => i.Name == typename);
                                }
                            }
                        }
                    }
                    if (r == null) { throw new ArgumentOutOfRangeException(nameof(source)); }
                    types.Add(source, r);
                }
            return r;
            }
        #endregion
        public ITypeLibraryTypeDescriptor PointerOf(VARTYPE source) {
            return new TypeLibraryTypePointer(TypeOf(source));
            }

        public void RegisterType(ITypeLibraryTypeDescriptor type) {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            types.Add(type.LibraryQualifiedName, type);
            ISet<Tuple<String,Version,Guid,CultureInfo,ITypeLibraryTypeDescriptor>> r;
            var n = type.Library.Name;
            var v = type.Library.Version;
            var u = type.Library.UniqueIdentifier;
            var c = type.Library.Culture;
            var key = Tuple.Create(n,v,u,c,type);
            if (!indexes.TryGetValue(type.Name, out r)) { indexes[type.Name] = r = new HashSet<Tuple<String, Version, Guid, CultureInfo, ITypeLibraryTypeDescriptor>>(); }
            r.Add(key);
            }

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.None)] private static extern UInt16 GetACP();
        }
    }