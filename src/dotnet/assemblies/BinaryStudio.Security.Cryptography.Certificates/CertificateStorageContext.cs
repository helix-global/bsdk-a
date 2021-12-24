using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [Serializable]
    internal class CertificateStorageContext : CertificateStorage
        {
        public override X509StoreLocation Location { get; }
        public override String Name { get; }
        public override IntPtr Handle { get { return store; }}

        #region M:CertificateStorageContext(IntPtr)
        public CertificateStorageContext(IntPtr store)
            {
            if (store == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(store)); }
            this.store = store;
            }
        #endregion
        #region M:CertificateStorageContext()
        protected CertificateStorageContext()
            {
            }
        #endregion
        #region M:CertificateStorageContext(X509StoreLocation)
        public CertificateStorageContext(X509StoreLocation location)
            :this(X509StoreName.My, location)
            {
            }
        #endregion
        #region M:CertificateStorageContext(X509StoreName)
        public CertificateStorageContext(X509StoreName name)
            :this(name, X509StoreLocation.CurrentUser)
            {
            }
        #endregion
        #region M:CertificateStorageContext(X509StoreName,X509StoreLocation)
        public CertificateStorageContext(X509StoreName name, X509StoreLocation location) {
            Location = location;
            switch (name)
                {
                case X509StoreName.AddressBook:          { Name = "AddressBook";      } break;
                case X509StoreName.AuthRoot:             { Name = "AuthRoot";         } break;
                case X509StoreName.CertificateAuthority: { Name = "CA";               } break;
                case X509StoreName.Disallowed:           { Name = "Disallowed";       } break;
                case X509StoreName.My:                   { Name = "My";               } break;
                case X509StoreName.Root:                 { Name = "Root";             } break;
                case X509StoreName.TrustedPeople:        { Name = "TrustedPeople";    } break;
                case X509StoreName.TrustedPublisher:     { Name = "TrustedPublisher"; } break;
                case X509StoreName.TrustedDevices:       { Name = "TrustedDevices";   } break;
                default: throw new ArgumentOutOfRangeException(nameof(name), name, null);
                }
            }
        #endregion
        #region M:CertificateStorageContext(String,X509StoreLocation)
        public CertificateStorageContext(String name, X509StoreLocation location) {
            Location = location;
            Name = name;
            }
        #endregion

        protected CertificateStorageContext(SerializationInfo info, StreamingContext context)
            :base(info, context)
            {
            var r = (IntPtr)info.GetInt64(nameof(Handle));
            if (r != IntPtr.Zero)
                {
                store = CertDuplicateStore(r);
                }
            }

        #region M:EnsureCore
        protected override void EnsureCore() {
            if (store == IntPtr.Zero) {
                #region standard storage
                if ((Location == X509StoreLocation.CurrentUser) || (Location == X509StoreLocation.LocalMachine)) {
                    store = CertOpenStore(
                        CERT_STORE_PROV_SYSTEM_W,
                        PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                        IntPtr.Zero,
                        MapX509StoreFlags(Location,X509OpenFlags.MaxAllowed|X509OpenFlags.OpenExistingOnly),
                        Name);
                    return;
                    }
                #endregion
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
                 if (storeLocation == X509StoreLocation.LocalMachine) { r |= CERT_SYSTEM_STORE_LOCAL_MACHINE; }
            else if (storeLocation == X509StoreLocation.CurrentUser)  { r |= CERT_SYSTEM_STORE_CURRENT_USER;  }
            return r;
            }
        #endregion
        #region M:Find(CERT_INFO*):X509Certificate
        public override unsafe X509Certificate Find(CERT_INFO* value, out Exception e)
            {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            e = null;
            EnsureCore();
            var i = CertGetSubjectCertificateFromStore(store,
                PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                value);
            if (i == IntPtr.Zero)
                {
                e = new CryptographicException(Marshal.GetLastWin32Error());
                return null;
                }
            return new X509Certificate(i);
            }
        #endregion
        #region M:IX509CertificateStorage.Add(IX509Certificate)
        public override void Add(IX509Certificate o)
            {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
            if (store != IntPtr.Zero) {
                if (!CertAddCertificateContextToStore(store, o.Handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, IntPtr.Zero)) {
                    var e = Marshal.GetLastWin32Error();
                    if ((e >= 0) && (e <= 0xffff)) {
                        if ((Win32ErrorCode)e == Win32ErrorCode.ERROR_ACCESS_DENIED) {
                            AddAdm(o);
                            }
                        }
                    }
                }
            }
        #endregion
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        private void AddX(IX509Certificate o)
            {
            Validate(CertAddCertificateContextToStore(store, o.Handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, IntPtr.Zero));
            }
        #region M:IX509CertificateStorage.Add(IX509Certificate)
        //[PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void AddAdm(IX509Certificate o)
            {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
            if (store != IntPtr.Zero) {
                var permissions = new PermissionSet(PermissionState.Unrestricted);
                permissions.AddPermission(new PrincipalPermission(null, "Administrators"));
                PlatformContext.Invoke(null,permissions, AddX,o);
                }
            }
        #endregion
        #region M:IX509CertificateStorage.Add(IX509CertificateRevocationList)
        public override void Add(IX509CertificateRevocationList o)
            {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
            if (store != IntPtr.Zero) {
                Validate(CertAddCRLContextToStore(store, o.Handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, IntPtr.Zero));
                }
            }
        #endregion
        #region M:IX509CertificateStorage.Remove(IX509Certificate)
        public override void Remove(IX509Certificate o) {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
            if (store != IntPtr.Zero) {
                CertAddCertificateContextToStore(store, o.Handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, out var r);
                Validate(CertDeleteCertificateFromStore(r));
                }
            }
        #endregion
        #region M:IX509CertificateStorage.Remove(IX509Certificate)
        public override void Remove(IX509CertificateRevocationList o)
            {
            if (o == null) {throw new ArgumentNullException(nameof(o)); }
            EnsureCore();
            if (store != IntPtr.Zero) {
                CertAddCRLContextToStore(store, o.Handle, CERT_STORE_ADD.CERT_STORE_ADD_ALWAYS, out var r);
                Validate(CertDeleteCRLFromStore(r));
                }
            }
        #endregion
        public override void Commit()
            {
            EnsureCore();
            if (store != IntPtr.Zero) {
                Validate(CertControlStore(store, 0, CERT_STORE_CTRL_AUTO_RESYNC, IntPtr.Zero));
                }
            }

        [DllImport("crypt32.dll", CharSet = CharSet.Auto,    SetLastError = true)] protected static extern bool CertControlStore([In] IntPtr hCertStore, [In] uint dwFlags, [In] uint dwCtrlType, [In] IntPtr pvCtrlPara);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto,    SetLastError = true)] protected static extern IntPtr CertDuplicateStore([In] IntPtr store);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern Boolean CertAddCertificateContextToStore(IntPtr store, IntPtr context, CERT_STORE_ADD disposition, out IntPtr r);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern Boolean CertAddCertificateContextToStore(IntPtr store, IntPtr context, CERT_STORE_ADD disposition, IntPtr r);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern Boolean CertAddCRLContextToStore(IntPtr store, IntPtr context, CERT_STORE_ADD disposition, IntPtr r);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern Boolean CertAddCRLContextToStore(IntPtr store, IntPtr context, CERT_STORE_ADD disposition, out IntPtr r);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern Boolean CertDeleteCertificateFromStore(IntPtr context);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern Boolean CertDeleteCRLFromStore(IntPtr context);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern IntPtr CertOpenStore(IntPtr lpszStoreProvider, UInt32 dwMsgAndCertEncodingType, IntPtr hCryptProv, UInt32 dwFlags, [In] String pvPara);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern IntPtr CertOpenStore(IntPtr lpszStoreProvider, UInt32 dwMsgAndCertEncodingType, IntPtr hCryptProv, UInt32 dwFlags, [In] IntPtr pvPara);
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)] protected static extern unsafe IntPtr CertGetSubjectCertificateFromStore(IntPtr hCertStore, UInt32 dwMsgAndCertEncodingType, CERT_INFO* CertId);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto,    SetLastError = true)] protected static extern IntPtr CertEnumCertificatesInStore(IntPtr hCertStore, IntPtr pPrevCertContext);

        protected IntPtr store;

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

        protected const UInt32 PKCS_7_ASN_ENCODING         = 0x00010000;
        protected const UInt32 X509_ASN_ENCODING           = 0x00000001;

        private const UInt32 CERT_STORE_CTRL_RESYNC              = 1;
        private const UInt32 CERT_STORE_CTRL_NOTIFY_CHANGE       = 2;
        private const UInt32 CERT_STORE_CTRL_COMMIT              = 3;
        private const UInt32 CERT_STORE_CTRL_AUTO_RESYNC         = 4;
        private const UInt32 CERT_STORE_CTRL_CANCEL_NOTIFY       = 5;

        public override IEnumerable<X509Certificate> Certificates { get {
            EnsureCore();
            var i = CertEnumCertificatesInStore(store, IntPtr.Zero);
            while (i != IntPtr.Zero) {
                yield return new X509Certificate(i);
                i = CertEnumCertificatesInStore(store, i);
                }
            }}

        /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }
            info.AddValue(nameof(Handle), (Int64)Handle);
            }
        }
    }
