﻿//#define FEATURE_CRYPT_VERIFY_CERTIFICATE_SIGNATURE_EX
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using BinaryStudio.DataProcessing;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider.Internal;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider.Internal.Fintech;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider.Properties;
using BinaryStudio.Security.Cryptography.Win32;
using BinaryStudio.Serialization;
using Microsoft.Win32;
#if NET40
using System.DirectoryServices.AccountManagement;
#endif

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    using CERT_BLOB = CRYPT_BLOB;
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1060:Move pinvokes to native methods class")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    #endif
    public partial class CryptographicContext : CryptographicObject, ICryptographicContext
        {
        public const String XmlDsigSHA1                      = "http://www.w3.org/2000/09/xmldsig#sha1";
        public const String XmlDsigDSA                       = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
        public const String XmlDsigRSASHA1                   = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        public const String XmlDsigHMACSHA1                  = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
        public const String XmlDsigSHA256                    = "http://www.w3.org/2001/04/xmlenc#sha256";
        public const String XmlDsigRSASHA256                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        public const String XmlDsigSHA384                    = "http://www.w3.org/2001/04/xmldsig-more#sha384";
        public const String XmlDsigRSASHA384                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";
        public const String XmlDsigSHA512                    = "http://www.w3.org/2001/04/xmlenc#sha512";
        public const String XmlDsigRSASHA512                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
        public const String URI_GOST_CIPHER	                 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gost28147";
        public const String	URI_GOST_DIGEST	                 = "http://www.w3.org/2001/04/xmldsig-more#gostr3411";
        public const String	URI_GOST_HMAC_GOSTR3411	         = "http://www.w3.org/2001/04/xmldsig-more#hmac-gostr3411";
        public const String	URI_GOST_SIGN                    = "http://www.w3.org/2001/04/xmldsig-more#gostr34102001-gostr3411";
        public const String	URI_GOST_TRANSPORT               = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2001";
        public const String	URI_GOST_TRANSPORT_GOST_2012_256 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2012-256";
        public const String	URI_GOST_TRANSPORT_GOST_2012_512 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2012-512";
        public const String	URN_GOST_DIGEST                  = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr3411";
        public const String	URN_GOST_DIGEST_2012_256         = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-256";
        public const String	URN_GOST_DIGEST_2012_512         = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-512";
        public const String	URN_GOST_HMAC_GOSTR3411          = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:hmac-gostr3411";
        public const String	URN_GOST_SIGN                    = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102001-gostr3411";
        public const String	URN_GOST_SIGN_2012_256           = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102012-gostr34112012-256";
        public const String	URN_GOST_SIGN_2012_512           = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102012-gostr34112012-512";

        static CryptographicContext()
            {
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_256_SignatureDescription), URN_GOST_SIGN_2012_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_512_SignatureDescription), URN_GOST_SIGN_2012_512);
            CryptoConfig.AddAlgorithm(typeof(Gost3411_12_256), URN_GOST_DIGEST_2012_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3411_12_512), URN_GOST_DIGEST_2012_512);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_256_SignatureDescription), ObjectIdentifiers.szOID_CP_GOST_R3410_12_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_512_SignatureDescription), ObjectIdentifiers.szOID_CP_GOST_R3410_12_512);
            RegisterCustomCryptographicMessageProvider(new FintechCryptographicMessageProvider());
            //foreach (var type in AvailableTypes) {
            //    Console.WriteLine($"{type.Key}:{type.Value}");
            //    }
            //Console.WriteLine("---");
            //foreach (var type in AvailableProviders)
            //    {
            //    Console.WriteLine($"{type.Key}:{type.Value}");
            //    }
            //Console.WriteLine("---");
            }

        #region P:AvailableProviders:IDictionary<String,CRYPT_PROVIDER_TYPE>
        public static IEnumerable<KeyValuePair<String, CRYPT_PROVIDER_TYPE>> AvailableProviders { get {
            var i = 0;
            var r = new Dictionary<String, CRYPT_PROVIDER_TYPE>();
            var builder = new StringBuilder(512);
            for (;;) {
                var sz = builder.Capacity;
                if (!EntryPoint.CryptEnumProviders(i, IntPtr.Zero, 0, out var type, builder, ref sz)) {
                    var e = (Win32ErrorCode)GetLastWin32Error();
                    if (e == Win32ErrorCode.ERROR_MORE_DATA) {
                        builder.Capacity = sz + 1;
                        continue;
                        }
                    break;
                    }
                r.Add(builder.ToString(), (CRYPT_PROVIDER_TYPE)type);
                i++;
                }
            //yield return new KeyValuePair<String, CRYPT_PROVIDER_TYPE>("Infotecs GOST 2012/1024 Cryptographic Service Provider",CRYPT_PROVIDER_TYPE.VPN_PROV_TYPE_2012_1024);
            //yield return new KeyValuePair<String, CRYPT_PROVIDER_TYPE>("Infotecs GOST 2012/512 Cryptographic Service Provider", CRYPT_PROVIDER_TYPE.VPN_PROV_TYPE_2012_512);
            foreach (var o in r)
                {
                yield return o;
                }
            }}
        #endregion
        #region P:AvailableTypes:IDictionary<CRYPT_PROVIDER_TYPE,String>
        public static IDictionary<CRYPT_PROVIDER_TYPE, String> AvailableTypes { get {
            var r = new Dictionary<CRYPT_PROVIDER_TYPE, String>();
            var i = 0;
            var builder = new StringBuilder(512);
            for (;;) {
                var sz = builder.Capacity;
                if (!EntryPoint.CryptEnumProviderTypes(i, IntPtr.Zero, 0, out var type, builder, ref sz)) {
                    var e = (Win32ErrorCode)GetLastWin32Error();
                    if (e == Win32ErrorCode.ERROR_MORE_DATA) {
                        builder.Capacity = sz + 1;
                        continue;
                        }
                    break;
                    }
                r.Add((CRYPT_PROVIDER_TYPE)type, builder.ToString());
                i++;
                }
            return new ReadOnlyDictionary<CRYPT_PROVIDER_TYPE, String>(r);
            }}
        #endregion
        #region P:SupportedAlgorithms:IDictionary<ALG_ID, String>
        public unsafe IDictionary<ALG_ID, String> SupportedAlgorithms { get {
            var r = new Dictionary<ALG_ID, String>();
            Int32 sz = 1024;
            var buffer = new LocalMemory(sz);
            var cflags = CRYPT_FIRST;
            while (EntryPoint.CryptGetProvParam(context.Handle, (Int32)CRYPT_PARAM.PP_ENUMALGS, buffer, ref sz, cflags)) {
                var alg = (PROV_ENUMALGS*)buffer;
                r.Add(alg->AlgId, ToString(&(alg->Name), alg->NameLength, Encoding.ASCII));
                cflags = CRYPT_NEXT;
                }
            return r;
            }}
        #endregion
        #region P:FullQualifiedContainerName:String
        public String FullQualifiedContainerName { get {
            var r = GetParameter<String>(CRYPT_PARAM.PP_FQCN, 0, Encoding.ASCII);
            return (r != null)
                    ? r.TrimEnd('\0')
                    : null;
            }}
        #endregion
        #region P:SmartCardReaderName:String
        public String SmartCardReaderName { get {
            var r = GetParameter<String>(CRYPT_PARAM.PP_SMARTCARD_READER, 0, Encoding.ASCII);
            return (r != null)
                    ? r.TrimEnd('\0')
                    : null;
            }}
        #endregion
        #region P:UniqueContainer:String
        public String UniqueContainer { get {
            var r = GetParameter<String>(CRYPT_PARAM.PP_UNIQUE_CONTAINER, 0, Encoding.ASCII);
            return (r != null)
                    ? r.TrimEnd('\0')
                    : null;
            }}
        #endregion
        #region P:SecurityDescriptor:RawSecurityDescriptor
        public RawSecurityDescriptor SecurityDescriptor { get
            {
            var r = GetParameter(CRYPT_PARAM.PP_KEYSET_SEC_DESCR,
                SECURITY_INFORMATION.OWNER_SECURITY_INFORMATION |
                SECURITY_INFORMATION.GROUP_SECURITY_INFORMATION |
                SECURITY_INFORMATION.DACL_SECURITY_INFORMATION |
                SECURITY_INFORMATION.SACL_SECURITY_INFORMATION);
            return (r != null)
                ? new RawSecurityDescriptor(r, 0)
                : null;
            }}
        #endregion
        public Version Version { get; }

        public SecureString SecureCode {
            set
                {
                if (value != null) {
                    var i = Marshal.SecureStringToGlobalAllocAnsi(value);
                    try
                        {
                        Validate(EntryPoint.CryptSetProvParam(Handle, PP_KEYEXCHANGE_PIN, i, 0));
                        }
                    finally
                        {
                        Marshal.ZeroFreeGlobalAllocAnsi(i);
                        }
                    }
                else
                    {
                    Validate(EntryPoint.CryptSetProvParam(Handle, PP_KEYEXCHANGE_PIN, IntPtr.Zero, 0));
                    }
                }
            }

        public event PercentageChangedEventHandler PercentageChanged;

        #region M:EnumUserKeys:IEnumerable<CryptKey>
        public IEnumerable<CryptKey> EnumUserKeys(Boolean @throw) {
            using (var context = new CryptographicContext(this,
                CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT|
                (UseMachineKeySet
                    ? CryptographicContextFlags.CRYPT_MACHINE_KEYSET
                    : CryptographicContextFlags.CRYPT_NONE)))
                {
                var c = GetParameter<String>(CRYPT_PARAM.PP_ENUMCONTAINERS, CRYPT_FIRST, Encoding.ASCII)?.TrimEnd('\0');
                while (c != null) {
                    using (var ctx = new CryptographicContext(context, c,
                        CryptographicContextFlags.CRYPT_SILENT|
                        (UseMachineKeySet
                            ? CryptographicContextFlags.CRYPT_MACHINE_KEYSET
                            : CryptographicContextFlags.CRYPT_NONE))) {
                        var key = ctx.GetUserKey(X509KeySpec.Exchange|X509KeySpec.Signature, ctx.FullQualifiedContainerName, @throw);
                        if (key != null)
                            {
                            yield return key;
                            }
                        }
                    c = GetParameter<String>(CRYPT_PARAM.PP_ENUMCONTAINERS, CRYPT_NEXT, Encoding.ASCII)?.TrimEnd('\0');
                    }
                }
            }
        #endregion

        public CryptographicContext(Oid algid, CryptographicContextFlags flags)
            {
            if (algid == null) { throw new ArgumentNullException(nameof(algid)); }
            Logger = null;
            var nalgid = OidToAlgId(algid);
            foreach (var type in AvailableProviders) {
                if (EntryPoint.CryptAcquireContext(out var r, container, type.Key, (Int32)type.Value, (Int32)flags)) {
                    foreach (var alg in GetSupportedAlgorithms(r)) {
                        if (alg.Key == nalgid) {
                            Flags = flags;
                            Type = type.Value;
                            context = CryptographicContextInternal.Create(Type, r, Logger);
                            CallerFree = true;
                            return;
                            }
                        }
                    }
                }
            throw new NotSupportedException();
            }

        #region M:CryptographicContext(String,String,CRYPT_PROVIDER_TYPE,CryptographicContextFlags,ILogger)
        public CryptographicContext(String container, String provider, CRYPT_PROVIDER_TYPE providertype, CryptographicContextFlags flags, ILogger logger) {
            UseMachineKeySet = flags.HasFlag(CryptographicContextFlags.CRYPT_MACHINE_KEYSET);
            Logger = logger;
            Type = providertype;
            if (provider == null) {
                foreach (var type in AvailableProviders) {
                    if (type.Value == providertype) {
                        provider = type.Key;
                        break;
                        }
                    }
                }
            Name = provider;
            Validate(EntryPoint.CryptAcquireContext(out var r, container, provider, (Int32)providertype, (Int32)flags));
            context = CryptographicContextInternal.Create(Type, r, logger);
            Flags = flags;
            this.container = container;
            CallerFree = true;
            Version = GetVersionInternal();
            }
        #endregion
        #region M:CryptographicContext(CRYPT_PROVIDER_TYPE,CryptographicContextFlags)
        public CryptographicContext(CRYPT_PROVIDER_TYPE providertype, CryptographicContextFlags flags)
            :this(null, null, providertype, flags, null)
            {
            }
        #endregion
        #region M:CryptographicContext(CRYPT_PROVIDER_TYPE,CryptographicContextFlags,ILogger)
        public CryptographicContext(CRYPT_PROVIDER_TYPE providertype, CryptographicContextFlags flags, ILogger logger)
            :this(null, null, providertype, flags, logger)
            {
            }
        #endregion
        #region M:CryptographicContext(CRYPT_PROVIDER_TYPE,String,CryptographicContextFlags)
        public CryptographicContext(CRYPT_PROVIDER_TYPE providertype, String container, CryptographicContextFlags flags)
            :this(container, null, providertype, flags, null)
            {
            }
        #endregion
        #region M:CryptographicContext(CRYPT_PROVIDER_TYPE,String,CryptographicContextFlags,ILogger)
        public CryptographicContext(CRYPT_PROVIDER_TYPE providertype, String container, CryptographicContextFlags flags, ILogger logger)
            :this(container, null, providertype, flags, logger)
            {
            }
        #endregion
        #region M:CryptographicContext(CryptographicContext,CryptographicContextFlags)
        internal CryptographicContext(CryptographicContext provider, CryptographicContextFlags flags) {
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }
            Logger = provider.Logger;
            container = provider.container;
            Flags = flags;
            Type = provider.Type;
            Name = provider.Name;
            UseMachineKeySet = flags.HasFlag(CryptographicContextFlags.CRYPT_MACHINE_KEYSET);
            Validate(EntryPoint.CryptAcquireContext(out var r, container, Name, (Int32)Type, (Int32)flags));
            context = CryptographicContextInternal.Create(Type, r, provider.Logger);
            CallerFree = true;
            Version = GetVersionInternal();
            }
        #endregion
        #region M:CryptographicContext(CryptographicContext,String,CryptographicContextFlags)
        internal CryptographicContext(CryptographicContext provider, String container, CryptographicContextFlags flags) {
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }
            if (container == null) { throw new ArgumentNullException(nameof(container)); }
            Logger = provider.Logger;
            Type = provider.Type;
            UseMachineKeySet = flags.HasFlag(CryptographicContextFlags.CRYPT_MACHINE_KEYSET);
            Validate(EntryPoint.CryptAcquireContext(out var r, container, null, (Int32)Type, (Int32)flags));
            context = CryptographicContextInternal.Create(Type, r, provider.Logger);
            Flags = flags;
            CallerFree = true;
            Version = GetVersionInternal();
            Name = provider.Name;
            }
        #endregion
        #region M:CryptographicContext(CryptographicContext,IX509Certificate)
        public CryptographicContext(CryptographicContext provider, IX509Certificate certificate, CRYPT_ACQUIRE_FLAGS flags) {
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            Logger = provider.Logger;
            Type = provider.Type;
            UseMachineKeySet = provider.UseMachineKeySet;
            try
                {
                #if CAPILITE
                Validate(CryptAcquireContext(out var r, certificate.Container, provider.Name, Type, (UseMachineKeySet
                    ? CryptographicContextFlags.CRYPT_MACHINE_KEYSET
                    : CryptographicContextFlags.CRYPT_NONE)));
                handle = new CryptographicContextHandle(r);
                CallerFree = false;
                #else
                Validate(EntryPoint.CryptAcquireCertificatePrivateKey(certificate.Handle, flags, IntPtr.Zero, out var r, out var keyspec, out var freeprov));
                context = CryptographicContextInternal.Create(Type, r, provider.Logger);
                CallerFree = freeprov;
                Version = GetVersionInternal();
                #endif
                }
            catch (Exception e)
                {
                e.Data["Flags"] = flags.ToString();
                e.Data["CertificateThumbprint"] = certificate.Thumbprint;
                e.Data["Certificate"] = certificate.ToString();
                throw;
                }
            }
        #endregion
        #region M:RequestSigningSecureString(IX509Certificate,RequestSecureStringEventHandler):CryptographicContext
        private CryptographicContext RequestSigningSecureString(IX509Certificate certificate, RequestSecureStringEventHandler requesthandler) {
            var context = new CryptographicContext(this, certificate, CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_NONE);
            if (requesthandler != null) {
                if (!context.IsSecureCodeStored()) {
                    context.Dispose();
                    context = new CryptographicContext(this, certificate, CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_SILENT_FLAG);
                    var e = new RequestSecureStringEventArgs
                        {
                        Container = context.FullQualifiedContainerName
                        };
                    requesthandler(this, e);
                    if (e.Canceled) { throw GetExceptionForHR(HRESULT.SCARD_W_CANCELLED_BY_USER); }
                    context.SecureCode = e.SecureString;
                    if (e.StoreSecureString) {
                        StoreSecureCode(certificate, e.SecureString);
                        }
                    }
                }
            return context;
            }
        #endregion
        #region M:VerifyCertificateSignature(IX509Certificate,IX509Certificate)
        private void VerifyCertificateSignature(IX509Certificate subject, IX509Certificate issuer)
            {
            if (subject == null) { throw new ArgumentNullException(nameof(subject)); }
            if (issuer  == null) { throw new ArgumentNullException(nameof(issuer));  }
            if (!VerifyCertificateSignature(out var e, subject, issuer, CRYPT_VERIFY_CERT_SIGN.NONE)) {
                throw e;
                }
            }
        #endregion
        #region M:VerifyObjectSignature(IX509Object,IX509Certificate)
        public void VerifyObjectSignature(IX509Object source, IX509Certificate signer)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (signer == null) { throw new ArgumentNullException(nameof(signer)); }
            switch (source.ObjectType)
                {
                case X509ObjectType.Certificate: { VerifyCertificateSignature((IX509Certificate)source, signer); } break;
                default: { throw new ArgumentOutOfRangeException(nameof(source)); }
                }
            }
        #endregion
        #region M:VerifyCertificateSignature(IX509Certificate,IX509Certificate,CRYPT_VERIFY_CERT_SIGN)
        public void VerifyCertificateSignature(IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            if (subject == null) { throw new ArgumentNullException(nameof(subject)); }
            if (issuer  == null) { throw new ArgumentNullException(nameof(issuer));  }
            if (!VerifyCertificateSignature(out var e, subject, issuer, flags)) {
                throw e;
                }
            }
        #endregion
        #region M:VerifyCertificateSignature([Out]Exception,IX509Certificate,IX509Certificate,CRYPT_VERIFY_CERT_SIGN):Boolean
        public Boolean VerifyCertificateSignature(out Exception e, IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            if (subject == null) { throw new ArgumentNullException(nameof(subject)); }
            if (issuer  == null) { throw new ArgumentNullException(nameof(issuer));  }
            #if FEATURE_CRYPT_VERIFY_CERTIFICATE_SIGNATURE_EX
            return Validate(out e, EntryPoint.CryptVerifyCertificateSignatureEx(handle,
                X509_ASN_ENCODING,
                CRYPT_VERIFY_CERT_SIGN_SUBJECT.CRYPT_VERIFY_CERT_SIGN_SUBJECT_CERT, subject.Handle,
                CRYPT_VERIFY_CERT_SIGN_ISSUER.CRYPT_VERIFY_CERT_SIGN_ISSUER_CERT, issuer.Handle,
                flags, IntPtr.Zero));
            #else
            using (var key = ImportPublicKey(out e, issuer.Handle)) {
                if ((e != null) || (key == null)) { return false; }
                using (var engine = (CryptHashAlgorithm)CreateHashAlgorithm(issuer.HashAlgorithm)) {
                    engine.HashCore(subject.GetSigningStream());
                    return engine.VerifySignature(out e,
                        subject.SignatureValue,
                        key);
                    }
                }
            #endif
            }
        #endregion
        #region M:CreateMessageSignature(Stream,Stream,IList<IX509Certificate>,CryptographicMessageFlags,RequestSecureStringEventHandler)
        public unsafe void CreateMessageSignature(Stream inputstream, Stream output, IList<IX509Certificate> certificates, CryptographicMessageFlags flags, RequestSecureStringEventHandler requesthandler)
            {
            if (inputstream == null)     { throw new ArgumentNullException(nameof(inputstream));        }
            if (output == null)          { throw new ArgumentNullException(nameof(output));             }
            if (certificates == null)    { throw new ArgumentNullException(nameof(certificates));       }
            if (certificates.Count == 0) { throw new ArgumentOutOfRangeException(nameof(certificates)); }
            if ((flags.HasFlag(CryptographicMessageFlags.Attached)) && (flags.HasFlag(CryptographicMessageFlags.Detached))) {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(flags),
                    message: String.Format(
                        PlatformSettings.DefaultCulture,
                        Resources.Cryptography_InvalidFlagCombination,
                            nameof(CryptographicMessageFlags.Attached) + "," +
                            nameof(CryptographicMessageFlags.Detached)));
                }

            var si_32 = new CMSG_SIGNED_ENCODE_INFO32();
            var si_64 = new CMSG_SIGNED_ENCODE_INFO64();
            var contextes = new List<CryptographicContext>();
            try
                {
                #region x86
                if (IntPtr.Size == 4)
                    {
                    si_32 = new CMSG_SIGNED_ENCODE_INFO32
                        {
                        Size = sizeof(CMSG_SIGNED_ENCODE_INFO32),
                        SignerCount = certificates.Count,
                        Signers = (CMSG_SIGNER_ENCODE_INFO32*)LocalAlloc(sizeof(CMSG_SIGNER_ENCODE_INFO32)*certificates.Count)
                        };
                    if (flags.HasFlag(CryptographicMessageFlags.IncludeSigningCertificate))
                        {
                        si_32.Certificates = (CERT_BLOB*)LocalAlloc(sizeof(CERT_BLOB)*si_32.SignerCount);
                        si_32.CertificateCount = si_32.SignerCount;
                        }
                    var certs = new IntPtr[si_32.SignerCount];
                    for (var i = 0; i < si_32.SignerCount; i++)
                        {
                        var certificate = certificates[i];
                        certificate.VerifyPrivateKeyUsagePeriod();
                        var certinfo = (CERT_CONTEXT*)certificate.Handle;
                        var context = RequestSigningSecureString(certificate, requesthandler);
                        contextes.Add(context);
                        si_32.Signers[i].Size = sizeof(CMSG_SIGNER_ENCODE_INFO32);
                        if (flags.HasFlag(CryptographicMessageFlags.IncludeSigningCertificate))
                            {
                            si_32.Signers[i].CertInfo = certinfo->CertInfo;
                            si_32.Signers[i].KeySpec = (KEY_SPEC_TYPE)certificate.KeySpec;
                            }
                        else
                            {
                            si_32.Signers[i].CertInfo = (CERT_INFO*)(certs[i] = (IntPtr)MakeCopy(certinfo->CertInfo));
                            si_32.Signers[i].KeySpec = (KEY_SPEC_TYPE)certificate.KeySpec;
                            }
                        si_32.Signers[i].CryptProvOrKey = context.Handle;
                        si_32.Signers[i].HashAlgorithm.ObjectId = (IntPtr)StringToMem(certificate.HashAlgorithm.Value, Encoding.ASCII);
                        si_32.Signers[i].HashEncryptionAlgorithm.ObjectId = (IntPtr)StringToMem(certificate.SignatureAlgorithm.Value, Encoding.ASCII);
                        if (flags.HasFlag(CryptographicMessageFlags.IncludeSigningCertificate))
                            {
                            si_32.Certificates[i].Size = certinfo->CertEncodedSize;
                            si_32.Certificates[i].Data = certinfo->CertEncoded; 
                            }
                        }
                    }
                #endregion
                #region x64
                else
                    {
                    si_64 = new CMSG_SIGNED_ENCODE_INFO64
                        {
                        Size = sizeof(CMSG_SIGNED_ENCODE_INFO64),
                        SignerCount = certificates.Count,
                        Signers = (CMSG_SIGNER_ENCODE_INFO64*)LocalAlloc(sizeof(CMSG_SIGNER_ENCODE_INFO64)*certificates.Count)
                        };
                    if (flags.HasFlag(CryptographicMessageFlags.IncludeSigningCertificate))
                        {
                        si_64.Certificates = (CERT_BLOB*)LocalAlloc(sizeof(CERT_BLOB)*si_64.SignerCount);
                        si_64.CertificateCount = si_64.SignerCount;
                        }
                    var certs = new IntPtr[si_64.SignerCount];
                    for (var i = 0; i < si_64.SignerCount; i++)
                        {
                        var certificate = certificates[i];
                        certificate.VerifyPrivateKeyUsagePeriod();
                        var certinfo = (CERT_CONTEXT*)certificate.Handle;
                        var context = RequestSigningSecureString(certificate, requesthandler);
                        contextes.Add(context);
                        si_64.Signers[i].Size = sizeof(CMSG_SIGNER_ENCODE_INFO64);
                        if (flags.HasFlag(CryptographicMessageFlags.IncludeSigningCertificate))
                            {
                            si_64.Signers[i].CertInfo = certinfo->CertInfo;
                            si_64.Signers[i].KeySpec = (KEY_SPEC_TYPE)certificate.KeySpec;
                            }
                        else
                            {
                            si_64.Signers[i].CertInfo = (CERT_INFO*)(certs[i] = (IntPtr)MakeCopy(certinfo->CertInfo));
                            si_64.Signers[i].KeySpec = (KEY_SPEC_TYPE)certificate.KeySpec;
                            }
                        si_64.Signers[i].CryptProvOrKey = context.Handle;
                        si_64.Signers[i].HashAlgorithm.ObjectId = (IntPtr)StringToMem(certificate.HashAlgorithm.Value, Encoding.ASCII);
                        si_64.Signers[i].HashEncryptionAlgorithm.ObjectId = (IntPtr)StringToMem(certificate.SignatureAlgorithm.Value, Encoding.ASCII);
                        if (flags.HasFlag(CryptographicMessageFlags.IncludeSigningCertificate))
                            {
                            si_64.Certificates[i].Size = certinfo->CertEncodedSize;
                            si_64.Certificates[i].Data = certinfo->CertEncoded; 
                            }
                        }
                    }
                #endregion
                CMSG_STREAM_INFO so;
                var nflags = flags.HasFlag(CryptographicMessageFlags.Detached)
                    ? CMSG_FLAGS.CMSG_DETACHED_FLAG
                    : CMSG_FLAGS.CMSG_NONE;
                #region BER
                if (flags.HasFlag(CryptographicMessageFlags.IndefiniteLength))
                    {
                    so = new CMSG_STREAM_INFO(CMSG_INDEFINITE_LENGTH,
                        delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final)
                            {
                            var bytes = new Byte[size];
                            for (var i = 0; i < size; i++) {
                                bytes[i] = data[i];
                                }
                            output.Write(bytes, 0, bytes.Length);
                            return true;
                            }, IntPtr.Zero);
                    }
                #endregion
                #region DER
                else
                    {
                    so = new CMSG_STREAM_INFO((UInt32)inputstream.Length,
                        delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final)
                            {
                            var bytes = new Byte[size];
                            for (var i = 0; i < size; i++) {
                                bytes[i] = data[i];
                                }
                            output.Write(bytes, 0, bytes.Length);
                            return true;
                            }, IntPtr.Zero);
                    }
                #endregion
                SetLastError(0);
                using (var message = new CryptographicMessage(
                    Validate(
                        (IntPtr.Size == 4)
                            ? EntryPoint.CryptMsgOpenToEncode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,nflags, CMSG_TYPE.CMSG_SIGNED,ref si_32, IntPtr.Zero, ref so)
                            : EntryPoint.CryptMsgOpenToEncode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,nflags, CMSG_TYPE.CMSG_SIGNED,ref si_64, IntPtr.Zero, ref so))
                        ))
                    {
                    var percentage = inputstream.CanSeek;
                    var length = (percentage) ? inputstream.Length : 1;
                    if (length == 0)
                        {
                        percentage = false;
                        length = 1;
                        }
                    var state  = (percentage) ? ProgressState.Normal : ProgressState.Indeterminate;
                    OnPercentageChanged(0.0, state);
                    var block = new Byte[BLOCK_SIZE_64K];
                    var size = 0.0;
                    for (;;) {
                        Yield();
                        var sz = inputstream.Read(block, 0, block.Length);
                        if (sz == 0) { break; }
                        message.Update(block, sz, false);
                        size += sz;
                        OnPercentageChanged(size / length, state);
                        }
                    message.Update(block, 0, true);
                    OnPercentageChanged(percentage ? 1.0 : size, ProgressState.None);
                    }
                }
            finally
                {
                for (var i = 0; i < contextes.Count; i++) {
                    if (contextes[i] != null) {
                        contextes[i].Dispose();
                        contextes[i] = null;
                        }
                    }
                contextes.Clear();
                }
            }
        #endregion
        #region M:VerifyAttachedMessageSignature(Stream,Stream,[Out]IList<IX509Certificate>,IX509CertificateResolver)
        public unsafe void VerifyAttachedMessageSignature(Stream input, Stream output, out IList<IX509Certificate> certificates, IX509CertificateResolver finder)
            {
            if (input == null) { throw new ArgumentNullException(nameof(input));   }
            certificates = new List<IX509Certificate>();
            var e = new List<Exception>();
            var position = input.CanSeek
                ? input.Position
                : -1;
            try
                {
                using (input.CanSeek
                        ? new TraceScope(input.Length)
                        : new TraceScope())
                        {
                    if (input.CanSeek)
                        {
                        foreach (var provider in CustomMessageProviders) {
                            if (provider.VerifyMessage(this, input, output, CryptographicMessageFlags.Attached, out var localcertificates, out var x)) {
                                certificates.Clear();
                                ((List<IX509Certificate>)certificates).AddRange(localcertificates);
                                return;
                                }
                            input.Seek(position, SeekOrigin.Begin);
                            ((List<IX509Certificate>)certificates).AddRange(localcertificates);
                            if (x != null)
                                {
                                e.Add(x);
                                }
                            }
                        }
                    using (var message = CryptographicMessage.OpenToDecode((bytes, final) => {
                        if (output != null) {
                            output.Write(bytes, 0, bytes.Length);
                            }
                        }))
                        {
                        var block = new Byte[BLOCK_SIZE_64K];
                        for (;;) {
                            Yield();
                            var sz = input.Read(block, 0, block.Length);
                            if (sz == 0) { break; }
                            message.Update(block, sz, false);
                            }
                        message.Update(EmptyArray<Byte>.Value, 0, true);
                        using (var store = new X509CertificateStorage(message)) {
                            for (var signerindex = 0;; signerindex++) {
                                var r = message.GetParameter(CMSG_PARAM.CMSG_SIGNER_CERT_INFO_PARAM, signerindex);
                                if (r.Length != 0) {
                                    fixed (Byte* blob = r) {
                                        var digest    = message.GetParameter(CMSG_PARAM.CMSG_COMPUTED_HASH_PARAM, signerindex);
                                        var encdigest = message.GetParameter(CMSG_PARAM.CMSG_ENCRYPTED_DIGEST,    signerindex);
                                        #if DEBUG
                                        #if NET35
                                        Debug.Print("SIGNER_{0}:CMSG_COMPUTED_HASH_PARAM:{1}", signerindex, String.Join(String.Empty, digest.Select(i => i.ToString("X2")).ToArray()));
                                        Debug.Print("SIGNER_{0}:CMSG_ENCRYPTED_DIGEST:[{2}]{1}", signerindex, String.Join(String.Empty, encdigest.Select(i => i.ToString("X2")).ToArray()), encdigest.Length);
                                        #else
                                        Debug.Print("SIGNER_{0}:CMSG_COMPUTED_HASH_PARAM:{1}", signerindex, String.Join(String.Empty, digest.ToString("X")));
                                        Debug.Print("SIGNER_{0}:CMSG_ENCRYPTED_DIGEST:[{2}]{1}", signerindex, String.Join(String.Empty, encdigest.ToString("X")), encdigest.Length);
                                        #endif
                                        #endif
                                        var certinfo = (CERT_INFO*)blob;
                                        var certificate = store.Find(certinfo, out var exception);
                                        if (certificate == null) {
                                            if (finder == null) { e.Add(exception); }
                                            else
                                                {
                                                var serialnumber = DecodeSerialNumberString(ref certinfo->SerialNumber);
                                                var issuer = DecodeNameString(ref certinfo->Issuer);
                                                certificate = finder.Find(serialnumber, issuer);
                                                if (certificate == null)
                                                    {
                                                    e.Add(Populate(Populate(new Exception("Невозможно проверить сообщение, так как не найден открытый ключ."),
                                                        "SerialNumber", serialnumber),
                                                        "Issuer", issuer));
                                                    }
                                                else
                                                    {
                                                    certificates.Add(certificate);
                                                    }
                                                }
                                            }
                                        else
                                            {
                                            certificates.Add(certificate);
                                            }
                                        if (certificate != null) {
                                            if (!EntryPoint.CryptMsgControl(message.Handle,
                                                    CRYPT_MESSAGE_FLAGS.CRYPT_MESSAGE_NONE,
                                                    CMSG_CTRL.CMSG_CTRL_VERIFY_SIGNATURE,
                                                    (IntPtr)((CERT_CONTEXT*)certificate.Handle)->CertInfo))
                                                {
                                                var hr = (HRESULT)(Marshal.GetLastWin32Error());
                                                if (hr == HRESULT.NTE_BAD_SIGNATURE)
                                                    {
                                                    using (var hashalg = new CryptHashAlgorithm(this, OidToAlgId(certificate.HashAlgorithm))) {
                                                        using (var key = ImportPublicKey(certificate.Handle)) {
                                                            if ((hr = hashalg.VerifySignatureInternal(encdigest, digest, key)) == 0)
                                                                {
                                                                continue;
                                                                }
                                                            }
                                                        }
                                                    }
                                                e.Add(new CryptographicException((Int32)hr));
                                                }
                                            }
                                        }
                                    }
                                else
                                    {
                                    break;
                                    }
                                }
                            }
                        }
                    }
                if (e.Count > 0)
                    {
                    if (e.Count == 1)
                        {
                        var x = e[0];
                        e.Clear();
                        throw x;
                        }
                    throw new AggregateException(e);
                    }
                }
            catch (COMException x) {
                if (x.ErrorCode == (Int32)HRESULT.CRYPT_E_ASN1_BADTAG) {
                    if (position >= 0) {
                        input.Seek(position, SeekOrigin.Begin);
                        var r = input.ReadByte();
                        var c = (Asn1ObjectClass)((r & 0xc0) >> 6);
                        if (c == Asn1ObjectClass.Application) {
                            x = null;
                            Asn1Object.DecodeLength(input);
                            try
                                {
                                VerifyAttachedMessageSignature(input, output, out certificates, finder);
                                }
                            catch (Exception z)
                                {
                                e.Add(z);
                                if (e.Count == 1) { throw e[0]; }
                                throw new AggregateException(e);
                                }
                            }
                        }
                    }
                if (x != null) {
                    e.Add(x);
                    if (e.Count == 1) { throw e[0]; }
                    throw new AggregateException(e);
                    }
                }
            catch (Exception x)
                {
                e.Add(x);
                if (e.Count == 1) { throw e[0]; }
                throw new AggregateException(e);
                }
            finally
                {
                certificates = certificates.Distinct().ToArray();
                }
            }
        #endregion
        #region M:VerifyAttachedMessageSignature(Byte[],[Out]IList<IX509Certificate>,IX509CertificateResolver)
        public void VerifyAttachedMessageSignature(Byte[] input, out IList<IX509Certificate> signers,IX509CertificateResolver finder)
            {
            if (input   == null) { throw new ArgumentNullException(nameof(input)); }
            using (new TraceScope()) {
                using (var inputstream = new MemoryStream(input)) {
                    VerifyAttachedMessageSignature(inputstream, null, out signers, finder);
                    }
                }
            }
        #endregion
        #region M:VerifyDetachedMessageSignature(Stream,Stream,[Out]IList<IX509Certificate>,IX509CertificateResolver)
        public unsafe void VerifyDetachedMessageSignature(Stream input, Stream inputdata, out IList<IX509Certificate> certificates, IX509CertificateResolver finder)
            {
            if (input == null) { throw new ArgumentNullException(nameof(input)); }
            if (inputdata == null) { throw new ArgumentNullException(nameof(inputdata)); }
            certificates = new List<IX509Certificate>();
            var e = new List<Exception>();
            try
                {
                using (new TraceScope()) {
                    var so = new CMSG_STREAM_INFO(CMSG_INDEFINITE_LENGTH,delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final) {
                        var bytes = new Byte[size];
                        for (var i = 0; i < size; i++) {
                            bytes[i] = data[i];
                            }
                        return true;
                        }, IntPtr.Zero);
                    using (var message = new CryptographicMessage(EntryPoint.CryptMsgOpenToDecode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING, CRYPT_OPEN_MESSAGE_FLAGS.CMSG_DETACHED_FLAG, CMSG_TYPE.CMSG_NONE, IntPtr.Zero, IntPtr.Zero, ref so)))
                        {
                        var block = new Byte[input.Length];
                        for (;;) {
                            var sz = input.Read(block, 0, block.Length);
                            if (sz == 0) { break; }
                            message.Update(block, sz, true);
                            break;
                            }
                        block = new Byte[BLOCK_SIZE_64K];
                        for (;;) {
                            Yield();
                            var sz = inputdata.Read(block, 0, block.Length);
                            if (sz == 0) { break; }
                            message.Update(block, sz, false);
                            }
                        #if NET40
                        message.Update(new Byte[0], 0, true);
                        #else
                        message.Update(Array.Empty<Byte>(), 0, true);
                        #endif
                        using (var store = new X509CertificateStorage(message)) {
                            for (var signerindex = 0;; signerindex++) {
                                var r = message.GetParameter(CMSG_PARAM.CMSG_SIGNER_CERT_INFO_PARAM, signerindex);
                                if (r.Length != 0) {
                                    fixed (Byte* blob = r) {
                                        var digest    = message.GetParameter(CMSG_PARAM.CMSG_COMPUTED_HASH_PARAM, signerindex);
                                        var encdigest = message.GetParameter(CMSG_PARAM.CMSG_ENCRYPTED_DIGEST,    signerindex);
                                        #if DEBUG
                                        #if NET35
                                        Debug.Print("SIGNER_{0}:CMSG_COMPUTED_HASH_PARAM:{1}", signerindex, String.Join(String.Empty, digest.Select(i => i.ToString("X2")).ToArray()));
                                        Debug.Print("SIGNER_{0}:CMSG_ENCRYPTED_DIGEST:[{2}]{1}", signerindex, String.Join(String.Empty, encdigest.Select(i => i.ToString("X2")).ToArray()), encdigest.Length);
                                        #else
                                        Debug.Print("SIGNER_{0}:CMSG_COMPUTED_HASH_PARAM:{1}", signerindex, String.Join(String.Empty, digest.ToString("X")));
                                        Debug.Print("SIGNER_{0}:CMSG_ENCRYPTED_DIGEST:[{2}]{1}", signerindex, String.Join(String.Empty, encdigest.ToString("X")), encdigest.Length);
                                        #endif
                                        #endif
                                        var certinfo = (CERT_INFO*)blob;
                                        var certificate = store.Find(certinfo, out var exception);
                                        if (certificate == null) {
                                            if (finder == null) { e.Add(exception); }
                                            else
                                                {
                                                certificate = finder.Find
                                                    (
                                                    DecodeSerialNumberString(ref certinfo->SerialNumber),
                                                    DecodeNameString(ref certinfo->Issuer)
                                                    );
                                                if (certificate == null)
                                                    {
                                                    e.Add(new Exception());
                                                    }
                                                else
                                                    {
                                                    certificates.Add(certificate);
                                                    }
                                                }
                                            }
                                        else
                                            {
                                            certificates.Add(certificate);
                                            }
                                        if (certificate != null) {
                                            if (!EntryPoint.CryptMsgControl(message.Handle,
                                                    CRYPT_MESSAGE_FLAGS.CRYPT_MESSAGE_NONE,
                                                    CMSG_CTRL.CMSG_CTRL_VERIFY_SIGNATURE,
                                                    (IntPtr)((CERT_CONTEXT*)certificate.Handle)->CertInfo))
                                                {
                                                var hr = (HRESULT)(Marshal.GetLastWin32Error());
                                                if (hr == HRESULT.NTE_BAD_SIGNATURE)
                                                    {
                                                    using (var hashalg = new CryptHashAlgorithm(this, OidToAlgId(certificate.HashAlgorithm))) {
                                                        using (var key = ImportPublicKey(certificate.Handle)) {
                                                            if ((hr = hashalg.VerifySignatureInternal(encdigest, digest, key)) == 0)
                                                                {
                                                                continue;
                                                                }
                                                            }
                                                        }
                                                    }
                                                e.Add(new CryptographicException((Int32)hr));
                                                }
                                            }
                                        }
                                    }
                                else
                                    {
                                    break;
                                    }
                                }
                            }
                        }
                    }
                if (e.Count > 0)
                    {
                    if (e.Count == 1) { throw e[0]; }
                    throw new AggregateException(e);
                    }
                }
            catch (Exception x)
                {
                e.Add(x);
                if (e.Count == 1) { throw e[0]; }
                throw new AggregateException(e);
                }
            finally
                {
                certificates = certificates.Distinct().ToArray();
                }
            }
        #endregion
        #region M:VerifyDetachedMessageSignature(Byte[],[Out]IList<IX509Certificate>,IX509CertificateResolver)
        public void VerifyDetachedMessageSignature(Byte[] input, out IList<IX509Certificate> signers,IX509CertificateResolver finder)
            {
            if (input   == null) { throw new ArgumentNullException(nameof(input)); }
            using (new TraceScope()) {
                using (var inputstream = new MemoryStream(input)) {
                    VerifyDetachedMessageSignature(inputstream, null, out signers, finder);
                    }
                }
            }
        #endregion
        #region M:ImportPublicKey([Out]Exception,IntPtr):CryptKey
        public unsafe CryptKey ImportPublicKey(out Exception e, IntPtr certificate) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            var context = (CERT_CONTEXT*)certificate;
            Validate(out e, EntryPoint.CryptImportPublicKeyInfo(Handle,
                CRYPT_MSG_TYPE.X509_ASN_ENCODING|CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                &(context->CertInfo->SubjectPublicKeyInfo),
                out var r
                ));
            return (e == null)
                ? new CryptKey(this, r)
                : null;
            }
        #endregion
        #region M:ImportPublicKey(IntPtr):CryptKey
        public unsafe CryptKey ImportPublicKey(IntPtr certificate) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            using (new TraceScope())
                {
                var context = (CERT_CONTEXT*)certificate;
                var pubkey = context->CertInfo->SubjectPublicKeyInfo;
                Validate(EntryPoint.CryptImportPublicKeyInfo(Handle,
                    CRYPT_MSG_TYPE.X509_ASN_ENCODING|CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                    &(context->CertInfo->SubjectPublicKeyInfo),
                    out var r
                    ));
                return new CryptKey(this, r);
                }
            }
        #endregion
        #region M:GetUserKey(KeySpec,String):CryptKey
        internal CryptKey GetUserKey(X509KeySpec keyspec, String container, Boolean @throw)
            {
            try
                {
                IntPtr r;
                     if (keyspec == X509KeySpec.Exchange)  { Validate(EntryPoint.CryptGetUserKey(Handle, (Int32)keyspec, out r)); }
                else if (keyspec == X509KeySpec.Signature) { Validate(EntryPoint.CryptGetUserKey(Handle, (Int32)keyspec, out r)); }
                else
                     {
                     if (!EntryPoint.CryptGetUserKey(Handle, (Int32)X509KeySpec.Exchange, out r)) {
                        keyspec = X509KeySpec.Signature;
                        if (!EntryPoint.CryptGetUserKey(Handle, (Int32)X509KeySpec.Signature, out r)) {
                            if (@throw)
                                {
                                Validate(GetHRForLastWin32Error());
                                }
                            return null;
                            }
                        }
                     else
                        {
                        keyspec = X509KeySpec.Exchange;
                        }
                     }
                return new CryptKey(this, r, container, keyspec);
                }
            catch (Exception e)
                {
                e.Data.Add("KeySpec", keyspec);
                e.Data.Add("Container", container);
                throw;
                }
            }
        #endregion
        #region M:OnPercentageChanged(Double)
        protected void OnPercentageChanged(Double value, ProgressState state)
            {
            PercentageChanged?.Invoke(this, new PercentageChangedEventArgs(value, state));
            }
        #endregion

        #if UBUNTU
        #if CAPILITE
        [DllImport(CAPI20, ExactSpelling = true, EntryPoint = "CryptAcquireContextA")]    private static extern Boolean CryptAcquireContext([Out] out IntPtr hCryptProv, [MarshalAs(UnmanagedType.LPStr)] [In] String pszContainer, [MarshalAs(UnmanagedType.LPStr)] [In] String pszProvider, [In] CRYPT_PROVIDER_TYPE dwProvType, [In] CryptographicContextFlags dwFlags);
        [DllImport(CAPI20, ExactSpelling = true, EntryPoint = "CryptEnumProviderTypesA")] private static extern Boolean CryptEnumProviderTypes([In] Int32 index, [In] IntPtr reserved, [In] UInt32 flags, [Out] out CRYPT_PROVIDER_TYPE type, [In][Out] StringBuilder name, [In][Out] ref Int32 sz);
        [DllImport(CAPI20, CharSet = CharSet.Auto)] private static extern Boolean CryptEnumProviders([In] Int32 index, [In] IntPtr reserved, [In] UInt32 flags, [Out] out CRYPT_PROVIDER_TYPE type, [In][Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder name, [In][Out] ref Int32 sz);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptAcquireCertificatePrivateKey(IntPtr cert, CRYPT_ACQUIRE_FLAGS flags, IntPtr parameters,[Out] out IntPtr prov, out KeySpec keyspec, out Boolean freeprov);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptVerifyCertificateSignatureEx(IntPtr provider,UInt32 encoding, CRYPT_VERIFY_CERT_SIGN_SUBJECT subjecttype, IntPtr subject, CRYPT_VERIFY_CERT_SIGN_ISSUER issuertype, IntPtr issuer, CRYPT_VERIFY_CERT_SIGN flags, IntPtr extra);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptMsgControl(IntPtr msg, CRYPT_MESSAGE_FLAGS flags, CMSG_CTRL ctrltype, void* ctrlpara);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptImportPublicKeyInfo(IntPtr provider, CRYPT_MSG_TYPE encoding, CERT_PUBLIC_KEY_INFO* info,  out CryptKeyHandle handle);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern CryptographicMessageHandle CryptMsgOpenToDecode(CRYPT_MSG_TYPE dwMsgEncodingType, CRYPT_OPEN_MESSAGE_FLAGS flags, CMSG_TYPE type, IntPtr hCryptProv, IntPtr pRecipientInfo, ref CMSG_STREAM_INFO si);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern CryptographicMessageHandle CryptMsgOpenToDecode(CRYPT_MSG_TYPE dwMsgEncodingType, CRYPT_OPEN_MESSAGE_FLAGS flags, CMSG_TYPE type, IntPtr hCryptProv, IntPtr pRecipientInfo, IntPtr si);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptDecryptMessage(ref CRYPT_DECRYPT_MESSAGE_PARA para, [MarshalAs(UnmanagedType.LPArray)] Byte[] encblob, Int32 encblobsize, [MarshalAs(UnmanagedType.LPArray)] Byte[] decblob, ref Int32 decblobsize, ref IntPtr r);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptDecryptMessage(ref CRYPT_DECRYPT_MESSAGE_PARA para, [MarshalAs(UnmanagedType.LPArray)] Byte[] encblob, Int32 encblobsize, [MarshalAs(UnmanagedType.LPArray)] Byte[] decblob, ref Int32 decblobsize, IntPtr r);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptDecryptMessage(ref CRYPT_DECRYPT_MESSAGE_PARA para, IntPtr encblob, Int32 encblobsize, [MarshalAs(UnmanagedType.LPArray)] Byte[] decblob, ref Int32 decblobsize, ref IntPtr r);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptDecryptMessage(ref CRYPT_DECRYPT_MESSAGE_PARA para, IntPtr encblob, Int32 encblobsize, [MarshalAs(UnmanagedType.LPArray)] Byte[] decblob, ref Int32 decblobsize, IntPtr r);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptGetUserKey(CryptographicContextHandle provider, X509KeySpec keyspec, out CryptKeyHandle r);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptEncryptMessage([In] CRYPT_ENCRYPT_MESSAGE_PARA* para, [In] Int32 recipientcount, [In] CERT_CONTEXT** recipients, [In] Byte* block, [In] Int32 blocksize, [Out] Byte* encryptedblob, [In, Out] Int32* encryptedblobsize);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe CryptographicMessageHandle CryptMsgOpenToEncode([In] CRYPT_MSG_TYPE dwmsgencodingtype, [In] CMSG_FLAGS flags, [In] CMSG_TYPE dwmsgtype, [In] ref CMSG_ENVELOPED_ENCODE_INFO pvmsgencodeinfo, [In] IntPtr pszinnercontentobjid, [In] ref CMSG_STREAM_INFO pstreaminfo);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe CryptographicMessageHandle CryptMsgOpenToEncode([In] CRYPT_MSG_TYPE dwmsgencodingtype, [In] CMSG_FLAGS flags, [In] CMSG_TYPE dwmsgtype, [In] ref CMSG_SIGNED_ENCODE_INFO pvmsgencodeinfo, [In] IntPtr pszinnercontentobjid, [In] ref CMSG_STREAM_INFO pstreaminfo);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe CRYPT_OID_INFO* CryptFindOIDInfo(CRYPT_OID_INFO_TYPE type, IntPtr key, CRYPT_OID_GROUP_ID group);
        [DllImport(CAPI20, CharSet = CharSet.None)][return: MarshalAs(UnmanagedType.Bool)] private static extern unsafe Boolean CryptGetProvParam(CryptographicContextHandle provider, CRYPT_PARAM parameter, void* data, ref Int32 pdwDataLen, UInt32 flags);
        [DllImport(CAPI20, CharSet = CharSet.None)][return: MarshalAs(UnmanagedType.Bool)] private static extern unsafe Boolean CryptGetProvParam(IntPtr provider, CRYPT_PARAM parameter, void* data, ref Int32 pdwDataLen, UInt32 flags);
        [DllImport(CAPI20, CharSet = CharSet.None)][return: MarshalAs(UnmanagedType.Bool)] private static extern unsafe Boolean CryptGetProvParam(IntPtr provider, CRYPT_PARAM parameter, void* data, ref Int32 pdwDataLen, SECURITY_INFORMATION flags);
        [DllImport(CAPI20, CharSet = CharSet.None)][return: MarshalAs(UnmanagedType.Bool)] private static extern unsafe Boolean CryptGetProvParam(CryptographicContextHandle provider, CRYPT_PARAM parameter, [MarshalAs(UnmanagedType.LPArray)] Byte[] data, ref Int32 pdwDataLen, UInt32 flags);
        [DllImport(CAPI20, CharSet = CharSet.None)][return: MarshalAs(UnmanagedType.Bool)] private static extern unsafe Boolean CryptGetProvParam(CryptographicContextHandle provider, CRYPT_PARAM parameter, [MarshalAs(UnmanagedType.LPArray)] Byte[] data, ref Int32 pdwDataLen, SECURITY_INFORMATION flags);
        #endif
        #else
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern Int16 SetThreadUILanguage(UInt16 LangId);
        #endif

        private const Int32 CRYPT_FIRST = 1;
        private const Int32 CRYPT_NEXT  = 2;
        private const Int32 PP_KEYEXCHANGE_PIN = 32;
        private const Int32 CERT_KEY_PROV_INFO_PROP_ID = 2;
        private const Int32 DEFAULT_BLOCK_SIZE = 64*1024;
        private const Int32 BLOCK_SIZE_64K = 64*1024;
        private const Int32 BLOCK_SIZE_64M = 64*1024*1024;
        private const Int32 BLOCK_SIZE_128 = 128;
        private const Int32 BLOCK_SIZE_512 = 512;
        private const UInt32 CMSG_INDEFINITE_LENGTH = 0xFFFFFFFF;
        private const UInt32 CRYPT_MESSAGE_SILENT_KEYSET_FLAG = 0x00000040;
        private const UInt32 MUI_LANGUAGE_ID = 0x4;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PROV_ENUMALGS
            {
            public readonly ALG_ID AlgId;
            public readonly Int32 Length;
            public readonly Int32 NameLength;
            public readonly Byte Name;
            }

        #region M:ToString(Byte*,Int32,Encoding):String
        private static unsafe String ToString(Byte* source, Int32 size, Encoding encoding) {
            var r = new Byte[size];
            for (var i = 0; i < size; ++i) {
                r[i] = source[i];
                }
            return encoding.GetString(r);
            }
        #endregion
        #region M:ToString(Byte*,Int32,Encoding):String
        private static unsafe String ToString(Byte* source, Encoding encoding) {
            if (source == null) { return null; }
            var r = new List<Byte>();
            while (*source != 0) { r.Add(*source++); }
            return encoding.GetString(r.ToArray());
            }
        #endregion
        #region M:ToByteArray(Byte*,Int32):Byte[]
        private unsafe Byte[] ToByteArray(Byte* source,Int32 size) {
            var r = new Byte[size];
            for (var i = 0; i < size; i++) {
                r[i] = source[i];
                }
            return r;
            }
        #endregion
        #region M:Validate(Boolean)
        protected sealed override void Validate(Boolean status) {
            if (!status) {
                Exception e;
                var i = GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]");
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]");
                    }
                e.Data["ProviderType"] = $"[{AvailableTypes[Type]}][{Type}][{(UInt32)Type}]";
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                throw e;
                }
            }
        #endregion
        #region M:Validate([Out]Exception,Boolean)
        protected sealed override Boolean Validate(out Exception e, Boolean status) {
            e = null;
            if (!status) {
                var i = GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]");
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]");
                    }
                e.Data["ProviderType"] = $"[{AvailableTypes[Type]}][{Type}][{(UInt32)Type}]";
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                return false;
                }
            return true;
            }
        #endregion
        #region M:Validate(HRESULT)
        protected override void Validate(HRESULT hr) {
            if (hr != HRESULT.S_OK) {
                Exception e = new COMException($"{FormatMessage((Int32)hr)} [HRESULT:{hr}]");
                e.Data["ProviderType"] = $"[{AvailableTypes[Type]}][{Type}][{(UInt32)Type}]";
                throw e;
                }
            }
        #endregion
        #region M:Validate(IntPtr):IntPtr
        protected IntPtr Validate(IntPtr value) {
            if (value == IntPtr.Zero) {
                Exception e;
                var i = GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]");
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]");
                    }
                e.Data["ProviderType"] = $"[{AvailableTypes[Type]}][{Type}][{(UInt32)Type}]";
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                throw e;
                }
            return value;
            }
        #endregion
        #region M:RegisterCustomCryptographicMessageProvider(ICustomCryptographicMessageProvider):Boolean
        public static Boolean RegisterCustomCryptographicMessageProvider(ICustomCryptographicMessageProvider provider)
            {
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }
            return CustomMessageProviders.Add(provider);
            }
        #endregion
        #region M:OIDToXmlDSig(Oid):String
        public static String OIDToXmlDSig(Oid algid)
            {
            if (algid == null) { throw new ArgumentNullException(nameof(algid)); }
            switch (algid.Value) {
                case ObjectIdentifiers.szOID_NIST_sha256: { return XmlDsigSHA256; }
                case ObjectIdentifiers.szOID_NIST_sha384: { return XmlDsigSHA384; }
                case ObjectIdentifiers.szOID_NIST_sha512: { return XmlDsigSHA512; }
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_256: { return URN_GOST_DIGEST_2012_256; }
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_512: { return URN_GOST_DIGEST_2012_512; }
                case ObjectIdentifiers.szOID_CP_GOST_R3410_12_256: { return URN_GOST_SIGN_2012_256; }
                case ObjectIdentifiers.szOID_CP_GOST_R3410_12_512: { return URN_GOST_SIGN_2012_512; }
                default: throw new ArgumentOutOfRangeException(nameof(algid));
                }
            }
        #endregion
        #region M:GetParameter<T>(CRYPT_PARAM,Int32,Encoding):T
        internal unsafe T GetParameter<T>(CRYPT_PARAM key, Int32 flags, Encoding encoding) {
            var r = GetParameter(key, flags);
            if (r == null) { return default; }
            if (typeof(T) == typeof(String)) { return (T)(Object)encoding.GetString(r, 0, r.Length); }
            fixed (Byte* i = r)
                {
                if (typeof(T) == typeof(Int32))  { return (T)(Object)(*(Int32*)i);  }
                if (typeof(T) == typeof(UInt32)) { return (T)(Object)(*(UInt32*)i); }
                if (typeof(T) == typeof(IntPtr)) { return (T)(Object)(*(IntPtr*)i); }
                }
            return default(T);
            }
        #endregion
        #region M:GetParameter(CRYPT_PARAM,Int32):void*
        internal Byte[] GetParameter(CRYPT_PARAM key, Int32 flags) {
            for (var i = 0x200;;) {
                var r = new Byte[i];
                if (EntryPoint.CryptGetProvParam(Handle, key, r, ref i, flags)) { return r; }
                var e = (Win32ErrorCode)Marshal.GetLastWin32Error();
                if (e == Win32ErrorCode.ERROR_MORE_DATA)
                    {
                    continue;
                    }
                var hr = (HRESULT)e;
                break;
                }
            return null;
            }
        #endregion
        #region M:GetParameter(CRYPT_PARAM,SECURITY_INFORMATION):void*
        internal Byte[] GetParameter(CRYPT_PARAM key, SECURITY_INFORMATION flags) {
            for (var i = 0x200;;) {
                var r = new Byte[i];
                if (EntryPoint.CryptGetProvParam(Handle, key, r, ref i, flags)) { return r; }
                var e = (Win32ErrorCode)Marshal.GetLastWin32Error();
                if (e == Win32ErrorCode.ERROR_MORE_DATA)
                    {
                    continue;
                    }
                var hr = (HRESULT)e;
                break;
                }
            return null;
            }
        #endregion
        #region M:SetParameter(CRYPT_PARAM,Int32):void*
        internal Boolean SetParameter(out Exception e, CRYPT_PARAM key, Int32 flags, Byte[] value) {
            return Validate(out e, EntryPoint.CryptSetProvParam(Handle, (Int32)key, value, flags));
            }
        #endregion
        #region M:SetParameter<T>(CRYPT_PARAM,Int32,Encoding):T
        public Boolean SetParameter<T>(out Exception e, CRYPT_PARAM key, Int32 flags, Encoding encoding, T value) {
            if ((typeof(T) == typeof(String)) && (encoding == null)) { throw new ArgumentNullException(nameof(encoding)); }
            if (typeof(T) == typeof(String)) { return SetParameter(out e, key, flags, encoding.GetBytes((String)(Object)value)); }
            e = new NotImplementedException();
            return false;
            }
        #endregion

        public static Oid SignatureToHashAlg(Oid algid)
            {
            return (algid != null)
                ? new Oid(Asn1SignatureAlgorithm.GetHashAlgorithm(algid.Value).ToString())
                : null;
            }

        public override IntPtr Handle { get { return context.Handle; }}
        protected internal override ILogger Logger { get; }
        private Boolean CallerFree { get; }
        public Boolean UseMachineKeySet { get; }
        public CRYPT_PROVIDER_TYPE Type { get; }
        public String Name { get; }

        internal CryptographicContextInternal context;
        private CryptographicContextFlags Flags { get; }
        private readonly String container;
        private static readonly HashSet<ICustomCryptographicMessageProvider> CustomMessageProviders = new HashSet<ICustomCryptographicMessageProvider>();

        #region M:GetHashAlgorithm(Oid):Oid
        private static Oid GetHashAlgorithm(Oid oid) {
            if (oid == null) { return null; }
            switch (oid.Value) {
                #region ГОСТ Р 34.11-94
                case ObjectIdentifiers.szOID_CP_GOST_R3411_R3410EL:
                    {
                    return new Oid(ObjectIdentifiers.szOID_CP_GOST_R3411);
                    }
                #endregion
                #region SHA1
                case ObjectIdentifiers.szOID_RSA_SHA1RSA:
                case ObjectIdentifiers.szOID_X957_SHA1DSA:
                case ObjectIdentifiers.szOID_DH_SINGLE_PASS_STDDH_SHA1_KDF:
                case ObjectIdentifiers.szOID_OIWSEC_dsaSHA1:
                case ObjectIdentifiers.szOID_OIWSEC_dsaCommSHA1:
                case ObjectIdentifiers.szOID_OIWSEC_sha1RSASign:
                case ObjectIdentifiers.szOID_ECDSA_SHA1:
                    {
                    return new Oid(ObjectIdentifiers.szOID_OIWSEC_sha1);
                    }
                #endregion
                #region SHA256
                case ObjectIdentifiers.szOID_ECDSA_SHA256:
                case ObjectIdentifiers.szOID_DH_SINGLE_PASS_STDDH_SHA256_KDF:
                case ObjectIdentifiers.szOID_RSA_SHA256RSA:
                    {
                    return new Oid(ObjectIdentifiers.szOID_NIST_sha256);
                    }
                #endregion
                #region ГОСТ Р 34.11-2012-256
                case ObjectIdentifiers.szOID_CP_GOST_R3410_12_256:
                case ObjectIdentifiers.szOID_tc26_gost_3410_12_256_paramSetA:
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_256_R3410:
                    {
                    return new Oid(ObjectIdentifiers.szOID_CP_GOST_R3411_12_256);
                    }
                #endregion
                #region ГОСТ Р 34.11-2012-512
                case ObjectIdentifiers.szOID_tc26_gost_3410_12_512_paramSetA:
                case ObjectIdentifiers.szOID_tc26_gost_3410_12_512_paramSetB:
                case ObjectIdentifiers.szOID_tc26_gost_3410_12_512_paramSetC:
                case ObjectIdentifiers.szOID_CP_GOST_R3410_12_512:
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_512_R3410:
                    {
                    return new Oid(ObjectIdentifiers.szOID_CP_GOST_R3411_12_512);
                    }
                #endregion
                #region SHA384
                case ObjectIdentifiers.szOID_ECDSA_SHA384:
                case ObjectIdentifiers.szOID_DH_SINGLE_PASS_STDDH_SHA384_KDF:
                case ObjectIdentifiers.szOID_RSA_SHA384RSA:
                    {
                    return new Oid(ObjectIdentifiers.szOID_NIST_sha384);
                    }
                #endregion
                #region SHA512
                case ObjectIdentifiers.szOID_ECDSA_SHA512:
                case ObjectIdentifiers.szOID_RSA_SHA512RSA:
                    {
                    return new Oid(ObjectIdentifiers.szOID_NIST_sha512);
                    }
                #endregion
                #region MD2
                case ObjectIdentifiers.szOID_RSA_MD2RSA :
                case ObjectIdentifiers.szOID_OIWDIR_md2:
                case ObjectIdentifiers.szOID_OIWDIR_md2RSA:
                case ObjectIdentifiers.szOID_OIWSEC_md2RSASign:
                    {
                    return new Oid(ObjectIdentifiers.szOID_RSA_MD2);
                    }
                #endregion
                #region MD4
                case ObjectIdentifiers.szOID_OIWSEC_md4RSA:
                case ObjectIdentifiers.szOID_OIWSEC_md4RSA2:
                case ObjectIdentifiers.szOID_RSA_MD4RSA :
                    {
                    return new Oid(ObjectIdentifiers.szOID_RSA_MD4);
                    }
                #endregion
                #region MD5
                case ObjectIdentifiers.szOID_OIWSEC_md5RSA:
                case ObjectIdentifiers.szOID_OIWSEC_md5RSASign:
                case ObjectIdentifiers.szOID_RSA_MD5RSA :
                    {
                    return new Oid(ObjectIdentifiers.szOID_RSA_MD5);
                    }
                #endregion
                }
            return null;
            }
        #endregion

        private static unsafe IDictionary<ALG_ID, String> GetSupportedAlgorithms(IntPtr handle) {
            var r = new Dictionary<ALG_ID, String>();
            var sz = 1024;
            var buffer = new LocalMemory(sz);
            var cflags = CRYPT_FIRST;
            while (EntryPoint.CryptGetProvParam(handle, (Int32)CRYPT_PARAM.PP_ENUMALGS, buffer, ref sz, cflags)) {
                var alg = (PROV_ENUMALGS*)buffer;
                r.Add(alg->AlgId, ToString(&(alg->Name), alg->NameLength, Encoding.ASCII));
                cflags = CRYPT_NEXT;
                }
            return r;
            }

        public IHashAlgorithm CreateHashAlgorithm(Oid algid)
            {
            return new CryptHashAlgorithm(this, OidToAlgId(algid));
            }

        public IHashAlgorithm CreateHashAlgorithm(ALG_ID algid)
            {
            return new CryptHashAlgorithm(this, algid);
            }

        private unsafe CERT_INFO* MakeCopy(CERT_INFO* source)
            {
            if (source == null) { return null; }
            var size = sizeof(CERT_INFO);
            size += source->Issuer.Size;
            size += source->SerialNumber.Size;
            var r = (CERT_INFO*)LocalAlloc(size);
            r->Version = source->Version;
            r->SerialNumber.Size = source->SerialNumber.Size;
            r->SerialNumber.Data = (Byte*)(r + 1);
            r->Issuer.Size = source->Issuer.Size;
            r->Issuer.Data = r->SerialNumber.Data + r->SerialNumber.Size;
            for (var i = 0; i < source->SerialNumber.Size; i++) { r->SerialNumber.Data[i] = source->SerialNumber.Data[i]; }
            for (var i = 0; i < source->Issuer.Size; i++) { r->Issuer.Data[i] = source->Issuer.Data[i]; }
            return r;
            }

        protected override void Dispose(Boolean disposing) {
            using (new TraceScope()) {
                if (disposing) {
                    Dispose(ref context);
                    }
                base.Dispose(disposing);
                }
            }

        public Boolean IsSupported(Oid algid) {
            if (algid == null) { return false; }
            var nalgid = OidToAlgId(algid);
            foreach (var alg in GetSupportedAlgorithms(Handle)) {
                if (alg.Key == nalgid) { return true; }
                }
            return false;
            }

        public static String GetOidFriendlyName(String oid) {
            return GetOidFriendlyName(oid, PlatformSettings.DefaultCulture);
            }

        public static unsafe String GetOidFriendlyName(String oid, CultureInfo culture) {
            #if !UBUNTU
            var lcid = (UInt16)(culture ?? PlatformSettings.DefaultCulture).LCID;
            SetThreadUILanguage(lcid);
            #endif
            var r = EntryPoint.CryptFindOIDInfo(CRYPT_OID_INFO_TYPE.CRYPT_OID_INFO_OID_KEY, Marshal.StringToHGlobalAnsi(oid), 0);
            return (r != null)
                ? Marshal.PtrToStringUni(r->pwszName)
                : oid;
            }

        #region M:IsSecureCodeStored:Boolean
        private Boolean IsSecureCodeStored() {
            return context.IsSecureCodeStored(this);
            }
        #endregion
        #region M:Populate<T>(T,String,String):T
        private static T Populate<T>(T source, String key, String value)
            where T:Exception
            {
            source.Data[key] = value;
            return source;
            }
        #endregion

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="service">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="service" />.
        /// -or-
        /// <see langword="null" /> if there is no service object of type <paramref name="service" />.</returns>
        public override Object GetService(Type service) {
            if (service == typeof(IX509CertificateStorage)) { return new CryptographicContextStorage(this); }
            return base.GetService(service);
            }

        #region M:GetVersionInternal:Version
        private Version GetVersionInternal()
            {
            var r = GetParameter<UInt32>(CRYPT_PARAM.PP_VERSION, 0, null);
            return new Version((Int32)(r & 0xFF00) >> 8, (Int32)(r & 0xFF));
            }
        #endregion

        private unsafe void StoreSecureCode(IX509Certificate certificate, SecureString value) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            if (value != null) {
                var i = new HGlobalAnsi(value);
                var H = certificate.Handle;
                var T = (CRYPT_KEY_PROV_PARAM*)manager.Alloc(sizeof(CRYPT_KEY_PROV_PARAM)*2);
                T[1].ParameterData = T[0].ParameterData = i;
                T[1].ParameterDataSize = T[0].ParameterDataSize = i.Length;
                T[0].ParameterIdentifier = CRYPT_PARAM.PP_SIGNATURE_PIN;
                T[1].ParameterIdentifier = CRYPT_PARAM.PP_KEYEXCHANGE_PIN;
                var P = new CRYPT_KEY_PROV_INFO {
                    ContainerName = (IntPtr)manager.StringToMem(certificate.Container, Encoding.ASCII),
                    ProviderType = Type,
                    KeySpec = (CRYPT_KEY_SPEC)certificate.KeySpec,
                    ProviderParameterCount = 2,
                    ProviderParameters = T
                    };
                Validate(EntryPoint.CertSetCertificateContextProperty(H, CERT_KEY_PROV_INFO_PROP_ID, 0, ref P));
                }
            }
        }
    }