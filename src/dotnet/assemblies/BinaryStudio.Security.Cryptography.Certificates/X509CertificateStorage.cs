using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
{
    public class X509CertificateStorage : IX509CertificateStorage
        {
        static X509CertificateStorage()
            {
            }

        #region P:Location:X509StoreLocation
        public X509StoreLocation Location { get {
            return (storage1 != null)
                ? storage1.Location
                : location;
            }}
        #endregion
        #region P:Name:String
        public String Name { get {
            return (storage1 != null)
                ? storage1.Name
                : name;
            }}
        #endregion
        #region P:Certificates:IEnumerable<X509Certificate>
        public IEnumerable<X509Certificate> Certificates { get {
            EnsureCore();
            if (storage1 != null) {
                foreach (var i in storage1.Certificates) {
                    yield return i;
                    }
                }
            else
                {
                if (store != IntPtr.Zero) {
                    var i = CertEnumCertificatesInStore(store, IntPtr.Zero);
                    while (i != IntPtr.Zero) {
                        yield return new X509Certificate(i);
                        i = CertEnumCertificatesInStore(store, i);
                        }
                    }
                }
            }}
        #endregion

        public IntPtr Handle {get {
            return (storage1 != null)
                ? storage1.Handle
                : store;
            }}

        IEnumerable<IX509Certificate> IX509CertificateStorage.Certificates { get {
            foreach (var certificate in Certificates) {
                yield return certificate;
                }
            }}

        #region P:SystemStoreLocations:IList<String>
        public static X509StoreLocation[] SystemStoreLocations { get {
            var r = new List<X509StoreLocation>();
            Validate(CertEnumSystemStoreLocation(0, IntPtr.Zero, delegate(String name,CERT_SYSTEM_STORE_FLAGS flags,IntPtr reserved,IntPtr arg) {
                r.Add((X509StoreLocation)flags);
                return true;
                }));
            return r.ToArray();
            }}
        #endregion

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing)
            {
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
        ~X509CertificateStorage()
            {
            Dispose(false);
            }
        #endregion
        #region M:EnsureCore
        protected virtual void EnsureCore() {
            if (storage1 == null) {
                if (store == IntPtr.Zero) {
                    if (name.Contains("\\")) {
                        store = CertOpenStore(
                            CERT_STORE_PROV_PHYSICAL,
                            PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                            IntPtr.Zero,
                            MapX509StoreFlags(Location,X509OpenFlags.MaxAllowed|X509OpenFlags.OpenExistingOnly),
                            Name);
                        }
                    #region standard storage
                    else if ((Location == X509StoreLocation.CurrentUser) || (Location == X509StoreLocation.LocalMachine)) {
                        store = CertOpenStore(
                            CERT_STORE_PROV_SYSTEM,
                            PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                            IntPtr.Zero,
                            MapX509StoreFlags(Location,X509OpenFlags.MaxAllowed|X509OpenFlags.OpenExistingOnly),
                            Name);
                        return;
                        }
                    #endregion
                    }
                }
            }
        #endregion
        #region M:MapX509StoreFlags(X509StoreLocation,X509OpenFlags):UInt32
        protected internal static UInt32 MapX509StoreFlags(X509StoreLocation storeLocation, X509OpenFlags flags) {
            var r = 0U;
            var mode = ((UInt32)flags) & 0x3;
            switch (mode)
                {
                case (UInt32)X509OpenFlags.ReadOnly:    { r |= CERT_STORE_READONLY_FLAG; }          break;
                case (UInt32)X509OpenFlags.MaxAllowed:  { r |= CERT_STORE_MAXIMUM_ALLOWED_FLAG; }   break;
                }
            if ((flags & X509OpenFlags.OpenExistingOnly) == X509OpenFlags.OpenExistingOnly) { r |= CERT_STORE_OPEN_EXISTING_FLAG; }
            if ((flags & X509OpenFlags.IncludeArchived)  == X509OpenFlags.IncludeArchived)  { r |= CERT_STORE_ENUM_ARCHIVED_FLAG; }
                 if (storeLocation == X509StoreLocation.LocalMachine)            { r |= CERT_SYSTEM_STORE_LOCAL_MACHINE;               }
            else if (storeLocation == X509StoreLocation.CurrentUser)             { r |= CERT_SYSTEM_STORE_CURRENT_USER;                }
            else if (storeLocation == X509StoreLocation.CurrentService)          { r |= CERT_SYSTEM_STORE_CURRENT_SERVICE;             }
            else if (storeLocation == X509StoreLocation.CurrentUserGroupPolicy)  { r |= CERT_SYSTEM_STORE_CURRENT_USER_GROUP_POLICY;   }
            else if (storeLocation == X509StoreLocation.LocalMachineEnterprise)  { r |= CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE;    }
            else if (storeLocation == X509StoreLocation.LocalMachineGroupPolicy) { r |= CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY;  }
            else if (storeLocation == X509StoreLocation.Services)                { r |= CERT_SYSTEM_STORE_SERVICES;                    }
            else if (storeLocation == X509StoreLocation.Users)                   { r |= CERT_SYSTEM_STORE_USERS;                       }
            return r;
            }
        #endregion
        #region M:IX509CertificateStorage.Add(IX509Certificate)
        public virtual void Add(IX509Certificate o)
            {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
                 if (storage1 != null) { storage1.Add(o); }
            else if (store != IntPtr.Zero) {
                if (!CertAddCertificateContextToStore(store, o.Handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, IntPtr.Zero)) {
                    var hr = (HRESULT)(Marshal.GetHRForLastWin32Error());
                    throw new COMException($"{FormatMessage((Int32)hr)} [HRESULT:{hr}]");
                    }
                }
            }
        #endregion
        #region M:IX509CertificateStorage.Add(IX509CertificateRevocationList)
        public void Add(IX509CertificateRevocationList o)
            {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
                 if (storage1 != null) { storage1.Add(o); }
            else if (store != IntPtr.Zero) {
                if (!CertAddCRLContextToStore(store, o.Handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, IntPtr.Zero)) {
                    var hr = (HRESULT)(Marshal.GetHRForLastWin32Error());
                    throw new COMException($"{FormatMessage((Int32)hr)} [HRESULT:{hr}]");
                    }
                }
            }
        #endregion
        #region M:IX509CertificateStorage.Remove(IX509Certificate)
        public void Remove(IX509Certificate o) {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
                 if (storage1 != null) { storage1.Remove(o); }
            else if (store != IntPtr.Zero) {
                throw new NotImplementedException();
                }
            }

        #endregion
        #region M:IX509CertificateStorage.Remove(IX509CertificateRevocationList)
        public void Remove(IX509CertificateRevocationList o)
            {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
                 if (storage1 != null) { storage1.Remove(o); }
            else if (store != IntPtr.Zero) {
                throw new NotImplementedException();
                }
            }
        #endregion

        public void Commit()
            {
            EnsureCore();
            if (store != IntPtr.Zero) {
                Validate(CertControlStore(store, 0, CERT_STORE_CTRL_AUTO_RESYNC, IntPtr.Zero));
                }
            else if (storage1 != null) {
                storage1.Commit();
                }
            }

        public static String[] GetSystemStores(X509StoreLocation flags) {
            var r = new List<String>();
            Validate(CertEnumSystemStore((CERT_SYSTEM_STORE_FLAGS)flags, IntPtr.Zero, IntPtr.Zero, delegate(IntPtr systemstore, CERT_SYSTEM_STORE_FLAGS storeflags, ref CERT_SYSTEM_STORE_INFO pStoreInfo, IntPtr pvReserved, IntPtr pvArg) {
                if (storeflags.HasFlag(CERT_SYSTEM_STORE_FLAGS.CERT_SYSTEM_STORE_RELOCATE_FLAG))
                    {
                    throw new NotSupportedException();
                    }
                else
                    {
                    r.Add(Marshal.PtrToStringUni(systemstore));
                    }
                return true;
                }));
            return r.ToArray();
            }

        public static String[] GetPhysicalStores(X509StoreLocation flags) {
            var r = new List<String>();
            Validate(CertEnumSystemStore((CERT_SYSTEM_STORE_FLAGS)flags, IntPtr.Zero, IntPtr.Zero, delegate(IntPtr systemstore, CERT_SYSTEM_STORE_FLAGS storeflags, ref CERT_SYSTEM_STORE_INFO pStoreInfo, IntPtr pvReserved, IntPtr pvArg) {
                Validate(CertEnumPhysicalStore(systemstore, storeflags, IntPtr.Zero, delegate(IntPtr pvSystemStore, CERT_SYSTEM_STORE_FLAGS localflags, String name, ref CERT_PHYSICAL_STORE_INFO pStoreInfo2, IntPtr pvReserved2, IntPtr pvArg2) {
                    r.Add($"{Marshal.PtrToStringUni(systemstore)}/{name}");
                    return true;
                    }));
                return true;
                }));
            return r.ToArray();
            }

        public X509CertificateStorage(X509StoreName name, X509StoreLocation location)
            {
            switch (name)
                {
                case X509StoreName.AddressBook:          { storage1 = new CertificateStorageContext(name, location); } break;
                case X509StoreName.AuthRoot:             { storage1 = new CertificateStorageContext(name, location); } break;
                case X509StoreName.CertificateAuthority: { storage1 = new CertificateStorageContext(name, location); } break;
                case X509StoreName.Disallowed:           { storage1 = new CertificateStorageContext(name, location); } break;
                case X509StoreName.My:                   { storage1 = new CertificateStorageContext(name, location); } break;
                case X509StoreName.Root:                 { storage1 = new CertificateStorageContext(name, location); } break;
                case X509StoreName.TrustedPeople:        { storage1 = new CertificateStorageContext(name, location); } break;
                case X509StoreName.TrustedPublisher:     { storage1 = new CertificateStorageContext(name, location); } break;
                case X509StoreName.TrustedDevices:       { storage1 = new CertificateStorageContext(name, location); } break;
                default: throw new ArgumentOutOfRangeException(nameof(name), name, null);
                }
            }

        public X509CertificateStorage(String name, X509StoreLocation location)
            {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            if (String.IsNullOrWhiteSpace(name)) { throw new ArgumentOutOfRangeException(nameof(name)); }
            this.name = name;
            this.location = location;
            }

        #region M:X509CertificateStorage(Uri)
        public X509CertificateStorage(Uri uri)
            {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }
            if (uri.Scheme != "file") { throw new ArgumentOutOfRangeException(nameof(uri)); }
            using (new TraceScope())
                {
                var certificates = new HashSet<String>();
                store = CertOpenStore(CERT_STORE_PROV_MEMORY, PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                    IntPtr.Zero,
                    MapX509StoreFlags(Location,X509OpenFlags.MaxAllowed|X509OpenFlags.OpenExistingOnly),
                    IntPtr.Zero);
                if (Directory.Exists(uri.AbsolutePath))
                    {
                    Directory.EnumerateFiles(uri.AbsolutePath, "*.*", SearchOption.AllDirectories).AsParallel().ForAll(i =>{
                        var e = Path.GetExtension(i);
                        if (e != null) {
                            switch (e.ToLower())
                                {
                                case ".cer":
                                    {
                                    var body = File.ReadAllBytes(i);
                                    if (body.Length > 0) {
                                        var handle = CertCreateCertificateContext(X509_ASN_ENCODING|PKCS_7_ASN_ENCODING, body, body.Length);
                                        if (handle != IntPtr.Zero) {
                                            if (!CertAddCertificateContextToStore(store, handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, IntPtr.Zero)) {
                                                var hr = (HRESULT)(Marshal.GetHRForLastWin32Error());
                                                throw new COMException($"{FormatMessage((Int32)hr)} [HRESULT:{hr}]");
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case ".crl":
                                    {
                                    var body = File.ReadAllBytes(i);
                                    if (body.Length > 0) {
                                        var handle = CertCreateCRLContext(X509_ASN_ENCODING|PKCS_7_ASN_ENCODING, body, body.Length);
                                        if (handle != IntPtr.Zero) {
                                            if (!CertAddCRLContextToStore(store, handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, IntPtr.Zero)) {
                                                var hr = (HRESULT)(Marshal.GetHRForLastWin32Error());
                                                throw new COMException($"{FormatMessage((Int32)hr)} [HRESULT:{hr}]");
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        });
                    }
                }
            }
        #endregion

        public X509CertificateStorage(ICryptographicMessage source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            storage1 = new MessageCertificateStorage(source.Handle);
            }

        public X509CertificateStorage(IntPtr source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            storage1 = new CertificateStorageContext(source);
            }

        #region M:X509CertificateStorage(IEnumerable<IX509Certificate>)
        public X509CertificateStorage(IEnumerable<IX509Certificate> certificates)
            {
            store = CertOpenStore(CERT_STORE_PROV_MEMORY, PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                IntPtr.Zero,
                MapX509StoreFlags(Location,X509OpenFlags.MaxAllowed|X509OpenFlags.OpenExistingOnly),
                IntPtr.Zero);
            foreach (var certificate in certificates)
                {
                Add(certificate);
                }
            }
        #endregion
        #region M:X509CertificateStorage()
        public X509CertificateStorage()
            {
            store = CertOpenStore(CERT_STORE_PROV_MEMORY, PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                IntPtr.Zero,
                MapX509StoreFlags(Location,X509OpenFlags.MaxAllowed|X509OpenFlags.OpenExistingOnly),
                IntPtr.Zero);
            }
        #endregion

        #region M:Find(CERT_INFO*):X509Certificate
        public unsafe IX509Certificate Find(CERT_INFO* value, out Exception e)
            {
            return storage1.Find(value, out e);
            }
        #endregion
        #region M:Validate(Boolean)
        private static void Validate(Boolean status) {
            if (!status) {
                Exception e;
                var i = Marshal.GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]");
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]");
                    }
                throw e;
                }
            }
        #endregion
        #region M:FormatMessage(Int32):String
        protected internal static unsafe String FormatMessage(Int32 source) {
            void* r;
            if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, source,
                ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL)),
                &r, 0, new IntPtr[0])) {
                try
                    {
                    return (new String((Char*)r)).Trim();
                    }
                finally
                    {
                    LocalFree(r);
                    }
                }
            return null;
            }
        #endregion

        private CertificateStorage storage1;
        private IntPtr store;
        private String name;
        private X509StoreLocation location;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] public delegate Boolean PFN_CERT_ENUM_SYSTEM_STORE_LOCATION([MarshalAs(UnmanagedType.LPWStr)] String name,CERT_SYSTEM_STORE_FLAGS flags,IntPtr reserved,IntPtr arg);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] public delegate Boolean PFN_CERT_ENUM_SYSTEM_STORE(IntPtr pvSystemStore, CERT_SYSTEM_STORE_FLAGS flags, ref CERT_SYSTEM_STORE_INFO pStoreInfo, IntPtr pvReserved, IntPtr pvArg);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] public delegate Boolean PFN_CERT_ENUM_PHYSICAL_STORE(IntPtr pvSystemStore, CERT_SYSTEM_STORE_FLAGS flags, [MarshalAs(UnmanagedType.LPWStr)] String name, ref CERT_PHYSICAL_STORE_INFO pStoreInfo, IntPtr pvReserved, IntPtr pvArg);

        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern bool CertControlStore([In] IntPtr hCertStore, [In] uint dwFlags, [In] uint dwCtrlType, [In] IntPtr pvCtrlPara);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr CertEnumCertificatesInStore(IntPtr hCertStore, IntPtr pPrevCertContext);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean CertEnumSystemStoreLocation(Int32 flags, IntPtr args, PFN_CERT_ENUM_SYSTEM_STORE_LOCATION pfn);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern IntPtr CertOpenStore(IntPtr lpszStoreProvider, UInt32 dwMsgAndCertEncodingType, IntPtr hCryptProv, UInt32 dwFlags, [In] String pvPara);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern IntPtr CertOpenStore(IntPtr lpszStoreProvider, UInt32 dwMsgAndCertEncodingType, IntPtr hCryptProv, UInt32 dwFlags, [In] IntPtr pvPara);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean CertAddCertificateContextToStore(IntPtr store, IntPtr context, CERT_STORE_ADD disposition, IntPtr r);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean CertAddCRLContextToStore(IntPtr store, IntPtr context, CERT_STORE_ADD disposition, IntPtr r);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean CertEnumSystemStore(CERT_SYSTEM_STORE_FLAGS flags, IntPtr pvSystemStoreLocationPara, IntPtr pvArg, PFN_CERT_ENUM_SYSTEM_STORE pfnEnum);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean CertEnumPhysicalStore(IntPtr pvSystemStore, CERT_SYSTEM_STORE_FLAGS flags, IntPtr pvArg, PFN_CERT_ENUM_PHYSICAL_STORE pfnEnum);
        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern IntPtr CertCreateCertificateContext(UInt32 dwCertEncodingType, [MarshalAs(UnmanagedType.LPArray)] Byte[] blob, Int32 size);
        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern IntPtr CertCreateCRLContext(UInt32 dwCertEncodingType, [MarshalAs(UnmanagedType.LPArray)] Byte[] blob, Int32 size);

        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;

        protected const UInt32 PKCS_7_ASN_ENCODING         = 0x00010000;
        protected const UInt32 X509_ASN_ENCODING           = 0x00000001;
        protected const UInt32 CERT_STORE_NO_CRYPT_RELEASE_FLAG                 = 0x00000001;
        protected const UInt32 CERT_STORE_SET_LOCALIZED_NAME_FLAG               = 0x00000002;
        protected const UInt32 CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG      = 0x00000004;
        protected const UInt32 CERT_STORE_DELETE_FLAG                           = 0x00000010;
        protected const UInt32 CERT_STORE_UNSAFE_PHYSICAL_FLAG                  = 0x00000020;
        protected const UInt32 CERT_STORE_SHARE_STORE_FLAG                      = 0x00000040;
        protected const UInt32 CERT_STORE_SHARE_CONTEXT_FLAG                    = 0x00000080;
        protected const UInt32 CERT_STORE_MANIFOLD_FLAG                         = 0x00000100;
        protected const UInt32 CERT_STORE_ENUM_ARCHIVED_FLAG                    = 0x00000200;
        protected const UInt32 CERT_STORE_UPDATE_KEYID_FLAG                     = 0x00000400;
        protected const UInt32 CERT_STORE_BACKUP_RESTORE_FLAG                   = 0x00000800;
        protected const UInt32 CERT_STORE_READONLY_FLAG                         = 0x00008000;
        protected const UInt32 CERT_STORE_OPEN_EXISTING_FLAG                    = 0x00004000;
        protected const UInt32 CERT_STORE_CREATE_NEW_FLAG                       = 0x00002000;
        protected const UInt32 CERT_STORE_MAXIMUM_ALLOWED_FLAG                  = 0x00001000;
        protected const UInt32 CERT_SYSTEM_STORE_CURRENT_USER                   = 0x00010000;
        protected const UInt32 CERT_SYSTEM_STORE_LOCAL_MACHINE                  = 0x00020000;
        protected const UInt32 CERT_SYSTEM_STORE_CURRENT_SERVICE                = 0x00040000;
        protected const UInt32 CERT_SYSTEM_STORE_SERVICES                       = 0x00050000;
        protected const UInt32 CERT_SYSTEM_STORE_USERS                          = 0x00060000;
        protected const UInt32 CERT_SYSTEM_STORE_CURRENT_USER_GROUP_POLICY      = 0x00070000;
        protected const UInt32 CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY     = 0x00080000;
        protected const UInt32 CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE       = 0x00090000;

        protected static readonly IntPtr CERT_STORE_PROV_MSG                 = new IntPtr(1);
        protected static readonly IntPtr CERT_STORE_PROV_MEMORY              = new IntPtr(2);
        protected static readonly IntPtr CERT_STORE_PROV_FILE                = new IntPtr(3);
        protected static readonly IntPtr CERT_STORE_PROV_REG                 = new IntPtr(4);
        protected static readonly IntPtr CERT_STORE_PROV_PKCS7               = new IntPtr(5);
        protected static readonly IntPtr CERT_STORE_PROV_SERIALIZED          = new IntPtr(6);
        protected static readonly IntPtr CERT_STORE_PROV_FILENAME_A          = new IntPtr(7);
        protected static readonly IntPtr CERT_STORE_PROV_FILENAME_W          = new IntPtr(8);
        protected static readonly IntPtr CERT_STORE_PROV_FILENAME            = CERT_STORE_PROV_FILENAME_W;
        protected static readonly IntPtr CERT_STORE_PROV_SYSTEM_A            = new IntPtr(9);
        protected static readonly IntPtr CERT_STORE_PROV_SYSTEM_W            = new IntPtr(10);
        protected static readonly IntPtr CERT_STORE_PROV_SYSTEM              = CERT_STORE_PROV_SYSTEM_W;
        protected static readonly IntPtr CERT_STORE_PROV_COLLECTION          = new IntPtr(11);
        protected static readonly IntPtr CERT_STORE_PROV_SYSTEM_REGISTRY_A   = new IntPtr(12);
        protected static readonly IntPtr CERT_STORE_PROV_SYSTEM_REGISTRY_W   = new IntPtr(13);
        protected static readonly IntPtr CERT_STORE_PROV_SYSTEM_REGISTRY     = CERT_STORE_PROV_SYSTEM_REGISTRY_W;
        protected static readonly IntPtr CERT_STORE_PROV_PHYSICAL_W          = new IntPtr(14);
        protected static readonly IntPtr CERT_STORE_PROV_PHYSICAL            = CERT_STORE_PROV_PHYSICAL_W;
        protected static readonly IntPtr CERT_STORE_PROV_SMART_CARD_W        = new IntPtr(15);
        protected static readonly IntPtr CERT_STORE_PROV_SMART_CARD          = CERT_STORE_PROV_SMART_CARD_W;
        protected static readonly IntPtr CERT_STORE_PROV_LDAP_W              = new IntPtr(16);
        protected static readonly IntPtr CERT_STORE_PROV_LDAP                = CERT_STORE_PROV_LDAP_W;
        protected static readonly IntPtr CERT_STORE_PROV_PKCS12              = new IntPtr(17);
        protected static readonly String sz_CERT_STORE_PROV_MEMORY           = "Memory";
        protected static readonly String sz_CERT_STORE_PROV_FILENAME_W       = "File";
        protected static readonly String sz_CERT_STORE_PROV_FILENAME         = sz_CERT_STORE_PROV_FILENAME_W;
        protected static readonly String sz_CERT_STORE_PROV_SYSTEM_W         = "System";
        protected static readonly String sz_CERT_STORE_PROV_SYSTEM           = sz_CERT_STORE_PROV_SYSTEM_W;
        protected static readonly String sz_CERT_STORE_PROV_PKCS7            = "PKCS7";
        protected static readonly String sz_CERT_STORE_PROV_PKCS12           = "PKCS12";
        protected static readonly String sz_CERT_STORE_PROV_SERIALIZED       = "Serialized";
        protected static readonly String sz_CERT_STORE_PROV_COLLECTION       = "Collection";
        protected static readonly String sz_CERT_STORE_PROV_SYSTEM_REGISTRY_W = "SystemRegistry";
        protected static readonly String sz_CERT_STORE_PROV_SYSTEM_REGISTRY  = sz_CERT_STORE_PROV_SYSTEM_REGISTRY_W;
        protected static readonly String sz_CERT_STORE_PROV_PHYSICAL_W       = "Physical";
        protected static readonly String sz_CERT_STORE_PROV_PHYSICAL         = sz_CERT_STORE_PROV_PHYSICAL_W;
        protected static readonly String sz_CERT_STORE_PROV_SMART_CARD_W     = "SmartCard";
        protected static readonly String sz_CERT_STORE_PROV_SMART_CARD       = sz_CERT_STORE_PROV_SMART_CARD_W;
        protected static readonly String sz_CERT_STORE_PROV_LDAP_W           = "Ldap";
        protected static readonly String sz_CERT_STORE_PROV_LDAP             = sz_CERT_STORE_PROV_LDAP_W;

        private const UInt32 CERT_STORE_CTRL_RESYNC              = 1;
        private const UInt32 CERT_STORE_CTRL_NOTIFY_CHANGE       = 2;
        private const UInt32 CERT_STORE_CTRL_COMMIT              = 3;
        private const UInt32 CERT_STORE_CTRL_AUTO_RESYNC         = 4;
        private const UInt32 CERT_STORE_CTRL_CANCEL_NOTIFY       = 5;
        }
    }
