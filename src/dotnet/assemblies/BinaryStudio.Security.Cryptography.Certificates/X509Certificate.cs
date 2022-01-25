#define VERIFY_CERTIFICATE_ALWAYS_TRUE
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BinaryStudio.IO;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Diagnostics;
using BinaryStudio.DirectoryServices;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Security.Cryptography.Certificates.Converters;
using BinaryStudio.Security.Cryptography.Certificates.Properties;
using BinaryStudio.Serialization;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.Certificates.Internal;
using Newtonsoft.Json;
using Microsoft.Win32;

// ReSharper disable LocalVariableHidesMember
// ReSharper disable ParameterHidesMember

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [Serializable]
    [TypeConverter(typeof(X509CertificateTypeConverter))]
    public class X509Certificate : X509Object, IX509Certificate, IXmlSerializable, IJsonSerializable, IFileService
        {
        IntPtr context;
        private PublicKey publickey;
        private Asn1Certificate source;
        private Boolean disposed;
        private String _fileName;

        public Asn1Certificate Source { get { return source; }}
        [Browsable(false)] public override IntPtr Handle { get { return context; }}
        [Browsable(false)] public override X509ObjectType ObjectType { get { return X509ObjectType.Certificate; }}

        #region P:[ 1]:Version:Int32
        [DispId(1)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(Version))]
        public Int32 Version { get; }
        #endregion
        #region P:[ 2]:SerialNumber:String
        [DispId(2)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(SerialNumber))]
        public String SerialNumber { get; }
        #endregion
        #region P:[ 5]:Issuer:X509RelativeDistinguishedNameSequence
        [DispId(5)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(Issuer))]
        public X509RelativeDistinguishedNameSequence Issuer { get; }
        #endregion
        [X509DisplayName("9000")]
        public String Container { get; }
        #region P:[ 6]:NotBefore:DateTime
        [DispId(6)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(NotBefore))]
        [TypeConverter(typeof(X509DataTimeConverter))]
        public DateTime NotBefore { get; }
        #endregion
        #region P:[ 7]:NotAfter:DateTime
        [DispId(7)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(NotAfter))]
        [TypeConverter(typeof(X509DataTimeConverter))]
        public DateTime NotAfter  { get; }
        #endregion
        #region P:[ 8]:Subject:X509RelativeDistinguishedNameSequence
        [DispId(8)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(Subject))]
        public X509RelativeDistinguishedNameSequence Subject { get; }
        #endregion
        #region P:[10]:Extensions:X509CertificateExtensionCollection
        [DispId(10)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(Extensions))]
        public Asn1CertificateExtensionCollection Extensions { get { return Source.Extensions; }}
        #endregion
        #region P:[13]:Thumbprint:String
        [DispId(13)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(Thumbprint))]
        public String Thumbprint { get; }
        #endregion
        [DispId(3)]
        [X509DisplayName(nameof(X509Certificate) + "." + nameof(SignatureAlgorithm))]
        public Oid SignatureAlgorithm { get; }
        public Oid HashAlgorithm { get; }
        public X509KeySpec KeySpec { get; }
        public String FullQualifiedContainerName { get; }
        public String FriendlyName { get { return Source.ToString(); }}

        IX509RelativeDistinguishedNameSequence IX509Certificate.Issuer  { get { return Issuer;  }}
        IList<IX509CertificateExtension> IX509Certificate.Extensions { get { return Source.Extensions; }}

        String IX509Certificate.Subject { get { return Subject.ToString(); }}
        [Browsable(false)] public RawSecurityDescriptor SecurityDescriptor { get; }
        [Browsable(false)]
        public unsafe Byte[] Bytes { get {
            var src = (CERT_CONTEXT*)Handle;
            var size  = src->CertEncodedSize;
            var bytes = src->CertEncoded;
            var r = new Byte[size];
            for (var i = 0U; i < size; ++i) {
                r[i] = bytes[i];
                }
            return r;
            }}

        public Byte[] SignatureValue { get {
            return Source[2].Content.ToArray();
            }}

        public String Country { get { return Source.Country; }}

        protected unsafe X509Certificate(SerializationInfo info, StreamingContext context)
            :base(info, context)
            {
            var source = (IntPtr)info.GetInt64(nameof(Handle));
            if (source != IntPtr.Zero) {
                using (new TraceScope()) {
                    this.context = CertDuplicateCertificateContext(source);
                    this.source = Load(this.context);
                    Version = Source.Version;
                    SerialNumber = Source.SerialNumber.ToString();
                    Issuer  = new X509RelativeDistinguishedNameSequence(Source.Issuer);
                    Subject = new X509RelativeDistinguishedNameSequence(Source.Subject);
                    NotAfter  = Source.NotAfter;
                    NotBefore = Source.NotBefore;
                    SignatureAlgorithm = new Oid(Source.SignatureAlgorithm.SignatureAlgorithm.ToString());
                    HashAlgorithm = (Source.SignatureAlgorithm.HashAlgorithm != null)
                        ? new Oid(Source.SignatureAlgorithm.HashAlgorithm.ToString())
                        : null;
                    Thumbprint = String.Join(String.Empty, GetProperty(CERT_HASH_PROP_ID, true).Select(i => i.ToString("X2")));
                    var r = GetProperty(CERT_KEY_PROV_INFO_PROP_ID, false);
                    if (r.Length > 0) {
                        fixed (Byte* bytes = r) {
                            var pi = (CRYPT_KEY_PROV_INFO*)bytes;
                            KeySpec = (X509KeySpec)pi->KeySpec;
                            Container = (pi->ContainerName != null)
                                ? Marshal.PtrToStringUni((IntPtr)(pi->ContainerName))
                                : null;
                            }
                        }
                    }
                }
            }

        /// <summary>Initializes a new instance of the <see cref="X509Certificate"/> class from specified source.</summary>
        /// <param name="source">Source of certificate context.</param>
        public unsafe X509Certificate(IntPtr source)
            {
            if (source == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(source)); }
            using (new TraceScope()) {
                context = CertDuplicateCertificateContext(source);
                this.source = Load(context);
                Version = Source.Version;
                SerialNumber = Source.SerialNumber.ToString();
                Issuer  = new X509RelativeDistinguishedNameSequence(Source.Issuer);
                Subject = new X509RelativeDistinguishedNameSequence(Source.Subject);
                NotAfter  = Source.NotAfter;
                NotBefore = Source.NotBefore;
                SignatureAlgorithm = new Oid(Source.SignatureAlgorithm.SignatureAlgorithm.ToString());
                HashAlgorithm = (Source.SignatureAlgorithm.HashAlgorithm != null)
                    ? new Oid(Source.SignatureAlgorithm.HashAlgorithm.ToString())
                    : null;
                Thumbprint = String.Join(String.Empty, GetProperty(CERT_HASH_PROP_ID, true).Select(i => i.ToString("X2")));
                var r = GetProperty(CERT_KEY_PROV_INFO_PROP_ID, false);
                if (r.Length > 0) {
                    fixed (Byte* bytes = r) {
                        var pi = (CRYPT_KEY_PROV_INFO*)bytes;
                        KeySpec = (X509KeySpec)pi->KeySpec;
                        Container = (pi->ContainerName != null)
                            ? Marshal.PtrToStringUni((IntPtr)(pi->ContainerName))
                            : null;
                        }
                    }
                }
            }

        #region M:StrLenA(IntPtr):int32
        private static unsafe Int32 StrLenA(IntPtr value) {
            return (value == IntPtr.Zero)
                ? 0
                : StrLenA((Byte*)value);
            }
        #endregion
        #region M:StrLenA(Byte*):Int32
        private static unsafe Int32 StrLenA(Byte* value) {
            if (value == null) { return 0; }
            for (var i = 0;; i++) {
                if (value[i] == 0) {
                    return i;
                    }
                }
            }
        #endregion

        public static unsafe void StoreSecureCode(IX509Certificate certificate, SecureString value) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            if (value != null) {
                using (var manager = new LocalMemoryManager()) {
                    //var i = Marshal.SecureStringToGlobalAllocAnsi(value);
                    var i = Marshal.StringToHGlobalAnsi("12345678");
                    try
                        {
                        var L = StrLenA(i);
                        var c = 0;
                        var H = certificate.Handle;
                        var P = new CRYPT_KEY_PROV_INFO {
                            ContainerName = (IntPtr)manager.StringToMem(certificate.Container, Encoding.ASCII),
                            };
                        //if (EntryPoint.CertGetCertificateContextProperty(H, CERT_KEY_PROV_INFO_PROP_ID, null, ref c)) {
                        //    var buffer = new Byte[c];
                        //    if (EntryPoint.CertGetCertificateContextProperty(H, CERT_KEY_PROV_INFO_PROP_ID, buffer, ref c)) {
                        //        fixed (Byte* r = buffer) {
                        //            var P = (CRYPT_KEY_PROV_INFO*)r;
                        //            var T = (CRYPT_KEY_PROV_PARAM*)manager.Alloc(sizeof(CRYPT_KEY_PROV_PARAM));
                        //            T[0].ParameterIdentifier = CRYPT_PARAM.PP_KEYEXCHANGE_PIN;
                        //            T[0].ParameterData = (Byte*)i;
                        //            T[0].ParameterDataSize = L + 1;
                        //            P->ProviderParameters = T;
                        //            P->ProviderParameterCount = 1;
                        //            P->Flags |= CRYPT_SILENT;
                        //            ValidateInternal(EntryPoint.CertSetCertificateContextProperty(H, CERT_KEY_PROV_INFO_PROP_ID, CERT_SET_PROPERTY_IGNORE_PERSIST_ERROR_FLAG, (IntPtr)P));

                        //            }
                        //        }
                        //    }
                        }
                    finally
                        {
                        Marshal.ZeroFreeGlobalAllocAnsi(i);
                        }
                    }
                }
            }

        #region M:Initialize
        private static unsafe Asn1Certificate Load(IntPtr context)
            {
            if (context == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(context)); }
            return Load((CERT_CONTEXT*)context);
            }
        private static Asn1Certificate Load(Byte[] source)
            {
            return new Asn1Certificate(Asn1Object.Load(new ReadOnlyMemoryMappingStream(source)).FirstOrDefault());
            }
        private static unsafe Asn1Certificate Load(CERT_CONTEXT* context)
            {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            var size  = context->CertEncodedSize;
            var bytes = context->CertEncoded;
            var buffer = new Byte[size];
            for (var i = 0U; i < size; ++i) {
                buffer[i] = bytes[i];
                }
            return new Asn1Certificate(Asn1Object.Load(new ReadOnlyMemoryMappingStream(buffer)).FirstOrDefault());
            }
        #endregion

        /// <summary>Initializes a new instance of the <see cref="X509Certificate"/> class from specified source.</summary>
        /// <param name="source">Source of certificate context.</param>
        public unsafe X509Certificate(CERT_CONTEXT* source)
            :this((IntPtr)source)
            {
            }

        public X509Certificate(Asn1Certificate source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            this.source = source;
            var body = source.UnderlyingObject.Body;
            var handle = CertCreateCertificateContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, body, body.Length);
            if (handle == IntPtr.Zero)
                {
                var hr = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR(hr);
                }
            context = handle;
            Version = Source.Version;
            SerialNumber = Source.SerialNumber.ToString();
            Issuer  = new X509RelativeDistinguishedNameSequence(Source.Issuer);
            Subject = new X509RelativeDistinguishedNameSequence(Source.Subject);
            NotAfter  = Source.NotAfter;
            NotBefore = Source.NotBefore;
            SignatureAlgorithm = new Oid(Source.SignatureAlgorithm.SignatureAlgorithm.ToString());
            HashAlgorithm = (Source.SignatureAlgorithm.HashAlgorithm != null)
                ? new Oid(Source.SignatureAlgorithm.HashAlgorithm.ToString())
                : null;
            #if FEATURE_CRYPT_CERTIFICATE_HANDLE
            Thumbprint = String.Join(String.Empty, GetProperty(CERT_HASH_PROP_ID, true).Select(i => i.ToString("X2")));
            #endif
            }

        public X509Certificate(Byte[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            using (new TraceScope()) {
                var handle = CertCreateCertificateContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, source, source.Length);
                if (handle == IntPtr.Zero)
                    {
                    var hr = GetHRForLastWin32Error();
                    Marshal.ThrowExceptionForHR((Int32)hr);
                    }
                context = handle;
                this.source = Load(source);
                Version = Source.Version;
                SerialNumber = Source.SerialNumber.ToString();
                Issuer  = new X509RelativeDistinguishedNameSequence(Source.Issuer);
                Subject = new X509RelativeDistinguishedNameSequence(Source.Subject);
                NotAfter  = Source.NotAfter;
                NotBefore = Source.NotBefore;
                SignatureAlgorithm = new Oid(Source.SignatureAlgorithm.SignatureAlgorithm.ToString());
                HashAlgorithm = new Oid(Source.SignatureAlgorithm.HashAlgorithm.ToString());
                Thumbprint = String.Join(String.Empty, GetProperty(CERT_HASH_PROP_ID, true).Select(i => i.ToString("X2")));
                }
            }
        #region M:X509Certificate(Byte[],String,KeySpec,ProviderType)
        internal X509Certificate(Byte[] source, String container, X509KeySpec keyspec, CRYPT_PROVIDER_TYPE provider, String fqcn, RawSecurityDescriptor secdesc, String providername)
            :this(source)
            {
            Container = container;
            KeySpec = keyspec;
            #if !CAPILITE
            var pi = new CRYPT_KEY_PROV_INFO
                {
                ContainerName = Marshal.StringToHGlobalUni(container),
                ProviderName = Marshal.StringToHGlobalUni(providername),
                KeySpec = (CRYPT_KEY_SPEC)keyspec,
                Flags = 0,
                ProviderParameterCount = 0,
                ProviderParameters = null,
                ProviderType = provider
                };
            Validate(CertSetCertificateContextProperty(Handle, CERT_KEY_PROV_INFO_PROP_ID, CERT_STORE_NO_CRYPT_RELEASE_FLAG, ref pi));
            #endif
            FullQualifiedContainerName = fqcn;
            SecurityDescriptor = secdesc;
            }
        #endregion

        #if CAPILITE
        [DllImport("capi20")] private static extern CertificateContextHandle CertDuplicateCertificateContext([In] IntPtr pCertContext);
        [DllImport("capi20")] private static extern IntPtr CertCreateCertificateContext(UInt32 dwCertEncodingType, [MarshalAs(UnmanagedType.LPArray)] Byte[] blob, Int32 size);
        [DllImport("capi20")] private static extern Boolean CertGetCertificateContextProperty([In] IntPtr context, [In] UInt32 property, [In][Out][MarshalAs(UnmanagedType.LPArray)] Byte[] data, [In][Out] ref Int32 size);
        [DllImport("capi20")] private static extern Boolean CertSetCertificateContextProperty([In] IntPtr context, [In] UInt32 property, UInt32 flags, ref CRYPT_KEY_PROV_INFO data);
        [DllImport("capi20")] private static extern unsafe Boolean CertGetCertificateChain([In] IntPtr chainEngine, [In] CertificateContextHandle context, [In] ref FILETIME time, [In] IntPtr additionalStore, [In] ref CERT_CHAIN_PARA chainPara, [In] CERT_CHAIN_FLAGS flags, [In] IntPtr reserved, [In][Out] CERT_CHAIN_CONTEXT** chainContext);
        #endif

        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CertGetCertificateContextProperty([In] IntPtr context, [In] Int32 property, [In][Out][MarshalAs(UnmanagedType.LPArray)] Byte[] data, [In][Out] ref Int32 size);
        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CertSetCertificateContextProperty([In] IntPtr context, [In] Int32 property, Int32 flags, ref CRYPT_KEY_PROV_INFO data);
        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CertSetCertificateContextProperty([In] IntPtr context, [In] Int32 property, Int32 flags, IntPtr data);
        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern IntPtr CertDuplicateCertificateContext([In] IntPtr pCertContext);
        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern IntPtr CertCreateCertificateContext(UInt32 dwCertEncodingType, [MarshalAs(UnmanagedType.LPArray)] Byte[] blob, Int32 size);
        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern IntPtr CertFindCRLInStore(IntPtr store, UInt32 CertEncodingType, Int32 FindFlags, Int32 FindType, IntPtr FindPara, IntPtr PrevCrlContext);
        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CertFreeCertificateContext(IntPtr pCertContext);

        private const Int32 CERT_KEY_PROV_INFO_PROP_ID = 2;
        private const Int32 CERT_SHA1_HASH_PROP_ID     = 3;
        private const Int32 CERT_HASH_PROP_ID          = CERT_SHA1_HASH_PROP_ID;
        private const Int32 CERT_STORE_NO_CRYPT_RELEASE_FLAG = 0x00000001;
        private const Int32 CERT_SET_PROPERTY_IGNORE_PERSIST_ERROR_FLAG = unchecked((Int32)0x80000000);
        private const Int32 CRL_FIND_ANY                        = 0;
        private const Int32 CRL_FIND_ISSUED_BY                  = 1;
        private const Int32 CRL_FIND_EXISTING                   = 2;
        private const Int32 CRL_FIND_ISSUED_FOR                 = 3;
        private const Int32 CRL_FIND_ISSUED_BY_AKI_FLAG         = 0x1;
        private const Int32 CRL_FIND_ISSUED_BY_SIGNATURE_FLAG   = 0x2;
        private const Int32 CRL_FIND_ISSUED_BY_DELTA_FLAG       = 0x4;
        private const Int32 CRL_FIND_ISSUED_BY_BASE_FLAG        = 0x8;
        private const Int32 PP_KEYEXCHANGE_PIN = 32;
        private const UInt32 CRYPT_SILENT = 0x00000040;

        #region M:GetProperty(Int32,Boolean)
        private Byte[] GetProperty(Int32 property, Boolean flags)
            {
            if (flags)
                {
                var c = 0;
                Validate(CertGetCertificateContextProperty(Handle, property, null, ref c));
                var r = new Byte[c];
                Validate(CertGetCertificateContextProperty(Handle, property, r, ref c));
                return r;
                }
            else
                {
                var c = 0;
                if (!CertGetCertificateContextProperty(Handle, property, null, ref c)) { return new Byte[0]; }
                var r = new Byte[c];
                return !CertGetCertificateContextProperty(Handle, property, r, ref c)
                    ? new Byte[0]
                    : r;
                }
            }
        #endregion
        #region M:IsNullOrEmpty(ICollection):Boolean
        private static Boolean IsNullOrEmpty(ICollection source)
            {
            return (source == null) || (source.Count == 0);
            }
        #endregion
        #region M:CopyToMemory(OidCollection):LocalMemoryHandle
        private static unsafe LocalMemoryHandle CopyToMemory(OidCollection values) {
            var items = values.OfType<Oid>().Select(i => i.Value).ToArray();
            var n = items.Length*IntPtr.Size;
            var offset = n;
            foreach (var i in items) {
                n += i.Length + 1;
                }
            var r = LocalMemoryHandle.Alloc(n);
            var p = (IntPtr)r;
            var c = p;
            for (var i = 0; i < items.Length; i++) {
                *(IntPtr*)(void*)c = p + offset;
                for (var j = 0; j < items[i].Length; j++) {
                    *(Byte*)(p + offset) = (Byte)items[i][j];
                    ++offset;
                    }
                *(Byte*)(p + offset) = 0;
                ++offset;
                c += IntPtr.Size;
                }
            return r;
            }
        #endregion

        #region M:VerifyNotAfter([Out]Exception,DateTime):Boolean
        private Boolean VerifyNotAfter(out Exception e, DateTime datetime) {
            e = null;
            try
                {
                if (NotAfter < datetime) {
                    throw new CryptographicException(Resources.ResourceManager.GetString("5000", PlatformContext.DefaultCulture));
                    }
                }
            catch (Exception exception)
                {
                exception.Data["NotAfter"] = NotAfter.ToString("s");
                e = exception;
                return false;
                }
            return true;
            }
        #endregion
        #region M:VerifyNotBefore([Out]Exception,DateTime):Boolean
        private Boolean VerifyNotBefore(out Exception e, DateTime datetime)
            {
            e = null;
            try
                {
                if (datetime < NotBefore) {
                    throw new CryptographicException(Resources.ResourceManager.GetString("5001", PlatformContext.DefaultCulture));
                    }
                }
            catch (Exception exception)
                {
                exception.Data["NotBefore"] = NotBefore.ToString("s");
                e = exception;
                return false;
                }
            return true;
            }
        #endregion
        //#region M:VerifySignature(List<Exception>,ICryptographicContext)
        private static Boolean VerifyCRL(X509Certificate certificate, HashSet<Exception> target, ICryptographicContext context, IntPtr store, ref CertificateChainErrorStatus status, DateTime datetime) {
            if ((store != IntPtr.Zero) && (status != CertificateChainErrorStatus.CERT_TRUST_NO_ERROR)) {
                var ski = ((CertificateSubjectKeyIdentifier)certificate.Source.Extensions.FirstOrDefault(i => i is CertificateSubjectKeyIdentifier))?.KeyIdentifier?.ToString("X");
                var exceptions = new List<Exception>();
                var hcrl = IntPtr.Zero;
                if (!String.IsNullOrWhiteSpace(ski)) {
                    while (true) {
                        hcrl = CertFindCRLInStore(store, X509_ASN_ENCODING, CRL_FIND_ISSUED_BY_AKI_FLAG, CRL_FIND_ISSUED_BY, certificate.Handle, hcrl);
                        if (hcrl == IntPtr.Zero)
                            {
                            break;
                            }
                        var crl = new X509CertificateRevocationList(hcrl);
                        var aki = ((CertificateAuthorityKeyIdentifier)crl.UnderlyingObject.Extensions.FirstOrDefault(i => i is CertificateAuthorityKeyIdentifier))?.KeyIdentifier?.ToString("X");
                        if (aki == ski) {
                            if (crl.EffectiveDate <= datetime) {
                                if (crl.NextUpdate != null) {
                                    if (crl.NextUpdate >= datetime) {
                                        return true;
                                        }
                                    }
                                else
                                    {
                                    return true;
                                    }
                                }
                            }
                        }
                    }
                }
            return true;
            }
        //#endregion

        #region M:VerifyPrivateKeyUsagePeriod([Out]Exception):Boolean
        public Boolean VerifyPrivateKeyUsagePeriod(out Exception e)
            {
            e = null;
            #if VERIFY_CERTIFICATE_ALWAYS_TRUE
            return true;
            #else
            var x = (Asn1CertificatePrivateKeyUsagePeriodExtension)Source.Extensions.FirstOrDefault(i => i is Asn1CertificatePrivateKeyUsagePeriodExtension);
            if (x != null) {
                if (x.NotAfter != null) {
                    if (DateTime.Now > x.NotAfter.Value) {
                        e = new Exception("Private key has expired.");
                        e.Data["PrivateKeyUsagePeriod.NotAfter"] = x.NotAfter.Value.ToString("o");
                        e.Data[nameof(Thumbprint)] = Thumbprint;
                        e.Data["(Self)"] = Source.FriendlyName;
                        return false;
                        }
                    }
                }
            return true;
            #endif
            }
        #endregion
        #region M:VerifyPrivateKeyUsagePeriod()
        public void VerifyPrivateKeyUsagePeriod()
            {
            if (!VerifyPrivateKeyUsagePeriod(out var e)) {
                throw e;
                }
            }
        #endregion

        #region M:GetSigningStreamInternal:IEnumerable<Byte>
        private IEnumerable<Byte> GetSigningStreamInternal() {
            return Source[0].Body;
            }
        #endregion
        #region M:GetSigningStream:Stream
        public Stream GetSigningStream()
            {
            return new ForwardOnlyByteStream(GetSigningStreamInternal);
            }
        #endregion

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         */
        public override String ToString()
            {
            return Source.ToString();
            }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            Source.WriteJson(writer, serializer);
            }

        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.</summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        public XmlSchema GetSchema()
            {
            throw new NotImplementedException();
            }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
        public void ReadXml(XmlReader reader)
            {
            throw new NotImplementedException();
            }

        #region M:WriteProperty(XmlWriterAdapter,Boolean,String,Object)
        private static void WriteProperty(XmlWriterAdapter writer, Boolean newline, String propertyname, Object value) {
            if (value != null) {
                writer.WriteStartAttribute(propertyname, newline);
                writer.WriteString(value.ToString());
                writer.WriteEndAttribute();
                }
            }
        #endregion
        #region M:WriteXml(XmlWriter)
        /**
         * <summary>Converts an object into its XML representation.</summary>
         * <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
         */
        public void WriteXml(XmlWriter writer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            using (new TraceScope()) {
                var flag = writer.Settings?.NewLineOnAttributes;
                using (var adapter = (flag == true)
                    ? (XmlWriterAdapter)new XmlWriterDefaultAdapter(writer)
                    : (XmlWriterAdapter)new XmlWellFormedWriterAdapter(writer))
                    {
                    using (adapter.WriteStartElement("Certificate")) {
                        WriteProperty(adapter, false, nameof(Version), Version);
                        WriteProperty(adapter, false, nameof(Thumbprint), Thumbprint);
                        WriteProperty(adapter, true,  nameof(NotBefore), NotBefore.ToString("s"));
                        WriteProperty(adapter, false, nameof(NotAfter), NotAfter.ToString("s"));
                        WriteProperty(adapter, true,  nameof(SerialNumber), SerialNumber);
                        WriteProperty(adapter, true,  nameof(Container), Container);
                        WriteProperty(adapter, true,  nameof(KeySpec), KeySpec);
                        using (adapter.WriteStartElement("Certificate." + nameof(Issuer))) {
                            var n = Source.Issuer.Select(i => (new Oid(i.Key.ToString())).FriendlyName).Max(i => i.Length);
                            foreach (var i in Source.Issuer) {
                                var key = (new Oid(i.Key.ToString())).FriendlyName;
                                using (adapter.WriteStartElement("TypeValuePair")) {
                                    WriteProperty(adapter, false,  "Type",  key);
                                    adapter.WriteWhitespace(n - key.Length);
                                    WriteProperty(adapter, false,  "Value", i.Value);
                                    }
                                }
                            using (adapter.CDataScope()) { adapter.WriteString(Issuer.ToString()); }
                            }
                        using (adapter.WriteStartElement("Certificate." + nameof(Subject))) {
                            var n = Source.Subject.Select(i => (new Oid(i.Key.ToString())).FriendlyName).Max(i => i.Length);
                            foreach (var i in Source.Subject) {
                                var key = (new Oid(i.Key.ToString())).FriendlyName;
                                using (adapter.WriteStartElement("TypeValuePair")) {
                                    WriteProperty(adapter, false,  "Type",  key);
                                    adapter.WriteWhitespace(n - key.Length);
                                    WriteProperty(adapter, false,  "Value", i.Value);
                                    }
                                }
                            using (adapter.CDataScope()) { adapter.WriteString(Subject.ToString()); }
                            }
                        using (adapter.WriteStartElement("Certificate." + nameof(SignatureAlgorithm))) {
                            }
                        }
                    }
                }
            }
        #endregion

        #region M:VerifyInternal(ICollection<Exception>,Action)
        private static void VerifyInternal(ICollection<Exception> target, Action action)
            {
            try
                {
                action();
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion
        #region M:VerifyInternal(ICollection<Exception>,Action<T1>)
        private static void VerifyInternal<T1>(ICollection<Exception> target, Action<T1> action, T1 arg1)
            {
            try
                {
                action(arg1);
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion
        #region M:VerifyInternal(ICollection<Exception>,Action<T1,T2>)
        private static void VerifyInternal<T1,T2>(ICollection<Exception> target, Action<T1,T2> action, T1 arg1, T2 arg2)
            {
            try
                {
                action(arg1, arg2);
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion
        #region M:VerifyInternal(ICollection<Exception>,Action<T1,T2,T3>)
        private static void VerifyInternal<T1,T2,T3>(ICollection<Exception> target, Action<T1,T2,T3> action, T1 arg1, T2 arg2, T3 arg3)
            {
            try
                {
                action(arg1, arg2, arg3);
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion
        #region M:VerifyInternal(ICollection<Exception>,Action<T1,T2,T3,T4>)
        private static void VerifyInternal<T1,T2,T3,T4>(ICollection<Exception> target, Action<T1,T2,T3,T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
            try
                {
                action(arg1, arg2, arg3, arg4);
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion
        #region M:VerifyInternal(ICollection<Exception>,Action<T1,T2,T3,T4,T5>)
        private static void VerifyInternal<T1,T2,T3,T4,T5>(ICollection<Exception> target, Action<T1,T2,T3,T4,T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            {
            try
                {
                action(arg1, arg2, arg3, arg4, arg5);
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion
        #region M:VerifyInternal(ICollection<Exception>,Action<T1,T2,T3,T4,T5,T6>)
        private static void VerifyInternal<T1,T2,T3,T4,T5,T6>(ICollection<Exception> target, Action<T1,T2,T3,T4,T5,T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            {
            try
                {
                action(arg1, arg2, arg3, arg4, arg5, arg6);
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion
        #region M:VerifyInternal(ICollection<Exception>,Action<T1,T2,T3,T4,T5,T6,T7>)
        private static void VerifyInternal<T1,T2,T3,T4,T5,T6,T7>(ICollection<Exception> target, Action<T1,T2,T3,T4,T5,T6,T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            {
            try
                {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion
        #region M:VerifyInternal(ICollection<Exception>,Action<T1,T2,T3,T4,T5,T6,T7,T8>)
        private static void VerifyInternal<T1,T2,T3,T4,T5,T6,T7,T8>(ICollection<Exception> target, Action<T1,T2,T3,T4,T5,T6,T7,T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            {
            try
                {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                }
            catch (Exception e)
                {
                target.Add(e);
                }
            }
        #endregion

        private Exception GetExceptionForChainErrorStatus(String key, CultureInfo culture)
            {
            var r = new CryptographicException($"{Resources.ResourceManager.GetString(key, culture)} {{{FriendlyName}}}");
            r.Data["Code"] = key;
            r.Data[nameof(Subject)] = Subject.ToString();
            r.Data[nameof(Issuer)] = Issuer.ToString();
            r.Data[nameof(SerialNumber)] = SerialNumber;
            r.Data[nameof(Thumbprint)] = Thumbprint;
            r.Data[nameof(NotAfter)] = NotAfter;
            r.Data[nameof(NotBefore)] = NotBefore;
            //r.Data["Certificate"] = this;
            return r;
            }

        private Exception[] GetExceptionForChainErrorStatus(CertificateChainErrorStatus value, CultureInfo culture)
            {
            var r = new List<Exception>();
            var n = (UInt32)value;
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_VALID)                 != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_VALID),                 culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_VALID;                 }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED)                != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED),                culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED;                }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED)                        != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED),                        culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED;                        }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID)            != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID),            culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID;            }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE)            != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE),            culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE;            }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT)                 != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT),                 culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT;                 }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN)         != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN),         culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN;         }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC)                         != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC),                         culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC;                         }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION)                 != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION),                 culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION;                 }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS)        != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS),        culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS;        }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS)         != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS),         culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS;         }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS)          != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS),          culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS;          }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT) != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT), culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT; }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT)   != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT),   culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT;   }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT) != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT), culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT; }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT)      != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT),      culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT;      }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION)             != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION),             culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION;             }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY)          != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY),          culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY;          }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST)              != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST),              culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST;              }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT)    != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT),    culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT;    }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN)                  != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN),                  culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN;                  }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID)             != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID),             culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID;             }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID)        != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID),        culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID;        }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE)        != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE),        culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE;        }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE)                != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE),                culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE;                }
            if ((n & (UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE)                  != 0) { r.Add(GetExceptionForChainErrorStatus(nameof(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE),                  culture)); n &= ~(UInt32)CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE;                  }
            return r.ToArray();                                                                                        
            }

        #region M:Validate(Boolean)
        protected sealed override void Validate(Boolean status) {
            if (!status) {
                Exception e;
                var i = (Int32)GetLastWin32Error();
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
        #region M:ValidateInternal(Boolean)
        protected static void ValidateInternal(Boolean status) {
            if (!status) {
                Exception e;
                var i = (Int32)GetLastWin32Error();
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

        protected override IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes) {
            foreach (var i in base.GetProperties(attributes)) {
                switch (i.Name) {
                    case nameof(Container):
                        {
                        if (!String.IsNullOrWhiteSpace(Container))
                            {
                            yield return i;
                            }
                        }
                        break;
                    default: { yield return i; } break;
                    }
                }
            }

        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            using (new TraceScope()) {
                publickey = null;
                Dispose(ref source);
                if (context != IntPtr.Zero) {
                    CertFreeCertificateContext(context);
                    context = IntPtr.Zero;
                    }
                base.Dispose(disposing);
                }
            }

        public unsafe PublicKey PublicKey { get {
            if (publickey == null) {
                var context = (CERT_CONTEXT*)Handle;
                var certinfo = context->CertInfo;
                var pi = certinfo->SubjectPublicKeyInfo;
                var pk = new Byte[pi.PublicKey.cbData];
                var data = (Byte*)pi.PublicKey.pbData;
                for (var i = 0; i < pi.PublicKey.cbData; i++)
                    {
                    pk[i] = data[i];
                    }
                Oid oid;
                #if NET40
                oid = new Oid(Marshal.PtrToStringAnsi(pi.Algorithm.ObjectId));
                publickey = new PublicKey(oid,
                    new AsnEncodedData(oid, pk),
                    new AsnEncodedData(oid, pk));
                #else
                oid = Oid.FromOidValue(
                    Marshal.PtrToStringAnsi(pi.Algorithm.ObjectId),
                    OidGroup.PublicKeyAlgorithm);
                publickey = new PublicKey(oid,
                    new AsnEncodedData(oid, Source.PublicKeyParameters),
                    new AsnEncodedData(oid, pk));
                #endif
                }
            return publickey;
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

        String IFileService.FileName { get { return $"{FriendlyName}.cer"; }}
        String IFileService.FullName { get { return ((IFileService)this).FileName; }}

        unsafe Byte[] IFileService.ReadAllBytes()
            {
            var src   = (CERT_CONTEXT*)Handle;
            var size  = src->CertEncodedSize;
            var bytes = src->CertEncoded;
            var r = new Byte[size];
            for (var i = 0U; i < size; ++i) {
                r[i] = bytes[i];
                }
            return r;
            }

        Stream IFileService.OpenRead()
            {
            return new MemoryStream(((IFileService)this).ReadAllBytes());
            }

        void IFileService.MoveTo(String target) {
            ((IFileService)this).MoveTo(target, false);
            }

        /// <summary>Move an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        public void MoveTo(String target, Boolean overwrite)
            {
            ((IFileService)this).CopyTo(target, overwrite);
            }

        /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void IFileService.CopyTo(String target, Boolean overwrite)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var sourcestream = ((IFileService)this).OpenRead()) {
                if (File.Exists(target)) {
                    if (!overwrite) { throw new IOException(); }
                    File.Delete(target);
                    }
                var folder = Path.GetDirectoryName(target);
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                using (var targetstream = File.OpenWrite(target)) {
                    sourcestream.CopyTo(targetstream);
                    }
                }
            }
        }
    }
