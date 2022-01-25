using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using BinaryStudio.DataProcessing;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    [DefaultProperty(nameof(SignatureAlgorithm))]
    public class Asn1SignatureAlgorithm : Asn1LinkObject
        {
        [TypeConverter(typeof(Asn1ObjectIdentifierTypeConverter))] public Asn1ObjectIdentifier SignatureAlgorithm { get; }
        [TypeConverter(typeof(Asn1ObjectIdentifierTypeConverter))] public virtual Oid HashAlgorithm { get; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Asn1Object UnderlyingObject { get { return base.UnderlyingObject; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected internal override Boolean IsDecoded { get { return base.IsDecoded; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsFailed  { get { return base.IsFailed;  }}

        protected internal Asn1SignatureAlgorithm(Asn1Object source)
            : base(source)
            {
            SignatureAlgorithm = (Asn1ObjectIdentifier)source[0];
            HashAlgorithm = GetHashAlgorithm(SignatureAlgorithm.ToString());
            }

        protected internal Asn1SignatureAlgorithm(Asn1ObjectIdentifier source)
            : base(source)
            {
            SignatureAlgorithm = source;
            HashAlgorithm = GetHashAlgorithm(SignatureAlgorithm.ToString());
            }

        private static readonly ReaderWriterLockSlim o = new ReaderWriterLockSlim();
        private static readonly IDictionary<String, Type> types = new Dictionary<String, Type>();
        public static Asn1SignatureAlgorithm From(Asn1SignatureAlgorithm source)
            {
            EnsureFactory();
            var key = source.SignatureAlgorithm.ToString();
            using (ReadLock(o)) {
                if (types.TryGetValue(key, out Type type)) {
                    var ctor = type.GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any,
                        new Type[] { typeof(Asn1Object) }, null);
                    if (ctor == null) { throw new MissingMemberException(); }
                    var r = (Asn1SignatureAlgorithm)ctor.Invoke(new Object[] { source.UnderlyingObject });
                    return r;
                    }
                }
            return source;
            }

        private static void EnsureFactory() {
            using (UpgradeableReadLock(o)) {
                if (types.Count == 0) {
                    using (WriteLock(o)) {
                        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(i => i.IsSubclassOf(typeof(Asn1SignatureAlgorithm)))) {
                            var attribute = (Asn1SpecificObjectAttribute)type.GetCustomAttributes(typeof(Asn1SpecificObjectAttribute), false).FirstOrDefault();
                            if (attribute != null) {
                                types.Add(attribute.Key, type);
                                }
                            }
                        }
                    }
                }
            }

        public override String ToString()
            {
            return SignatureAlgorithm.ToString();
            }

        #region M:GetHashAlgorithm(String):Oid
        public static Oid GetHashAlgorithm(String oid) {
            switch (oid) {
                #region ГОСТ Р 34.11-94
                case szOID_CP_GOST_R3411_R3410EL:
                    {
                    return new Oid(szOID_CP_GOST_R3411);
                    }
                #endregion
                #region SHA1
                case szOID_RSA_SHA1RSA:
                case szOID_X957_SHA1DSA:
                case szOID_DH_SINGLE_PASS_STDDH_SHA1_KDF:
                case szOID_OIWSEC_dsaSHA1:
                case szOID_OIWSEC_dsaCommSHA1:
                case szOID_OIWSEC_sha1RSASign:
                case szOID_ECDSA_SHA1:
                case szOID_RSA_SSA_PSS:
                    {
                    return new Oid(szOID_OIWSEC_sha1);
                    }
                #endregion
                #region SHA256
                case szOID_ECDSA_SHA256:
                case szOID_DH_SINGLE_PASS_STDDH_SHA256_KDF:
                case szOID_RSA_SHA256RSA:
                    {
                    return new Oid(szOID_NIST_sha256);
                    }
                #endregion
                #region ГОСТ Р 34.11-2012-256
                case szOID_CP_GOST_R3410_12_256:
                case szOID_tc26_gost_3410_12_256_paramSetA:
                case szOID_CP_GOST_R3411_12_256_R3410:
                    {
                    return new Oid(szOID_CP_GOST_R3411_12_256);
                    }
                #endregion
                #region ГОСТ Р 34.11-2012-512
                case szOID_tc26_gost_3410_12_512_paramSetA:
                case szOID_tc26_gost_3410_12_512_paramSetB:
                case szOID_tc26_gost_3410_12_512_paramSetC:
                case szOID_CP_GOST_R3410_12_512:
                case szOID_CP_GOST_R3411_12_512_R3410:
                    {
                    return new Oid(szOID_CP_GOST_R3411_12_512);
                    }
                #endregion
                #region SHA384
                case szOID_ECDSA_SHA384:
                case szOID_DH_SINGLE_PASS_STDDH_SHA384_KDF:
                case szOID_RSA_SHA384RSA:
                    {
                    return new Oid(szOID_NIST_sha384);
                    }
                #endregion
                #region SHA512
                case szOID_ECDSA_SHA512:
                case szOID_RSA_SHA512RSA:
                    {
                    return new Oid(szOID_NIST_sha512);
                    }
                #endregion
                #region MD2
                case szOID_RSA_MD2RSA :
                case szOID_OIWDIR_md2:
                case szOID_OIWDIR_md2RSA:
                case szOID_OIWSEC_md2RSASign:
                    {
                    return new Oid(szOID_RSA_MD2);
                    }
                #endregion
                #region MD4
                case szOID_OIWSEC_md4RSA:
                case szOID_OIWSEC_md4RSA2:
                case szOID_RSA_MD4RSA :
                    {
                    return new Oid(szOID_RSA_MD4);
                    }
                #endregion
                #region MD5
                case szOID_OIWSEC_md5RSA:
                case szOID_OIWSEC_md5RSASign:
                case szOID_RSA_MD5RSA :
                    {
                    return new Oid(szOID_RSA_MD5);
                    }
                #endregion
                }
            return null;
            }
        #endregion

        private  const String szOID_RSA                                                                          = "1.2.840.113549";
        private  const String szOID_PKCS                                                                         = szOID_RSA + ".1";
        private  const String szOID_RSA_HASH                                                                     = szOID_RSA + ".2";
        private  const String szOID_RSA_ENCRYPT                                                                  = szOID_RSA + ".3";
        private  const String szOID_PKCS_1                                                                       = szOID_PKCS + ".1";
        private  const String szOID_RSA_RSA                                                                      = "1.2.840.113549.1.1.1";
        private  const String szOID_RSA_MD2RSA                                                                   = "1.2.840.113549.1.1.2";
        private  const String szOID_RSA_MD4RSA                                                                   = "1.2.840.113549.1.1.3";
        private  const String szOID_RSA_MD5RSA                                                                   = "1.2.840.113549.1.1.4";
        private  const String szOID_RSA_SHA1RSA                                                                  = "1.2.840.113549.1.1.5";
        private  const String szOID_RSA_SETOAEP_RSA                                                              = "1.2.840.113549.1.1.6";
        private  const String szOID_RSAES_OAEP                                                                   = "1.2.840.113549.1.1.7";
        private  const String szOID_RSA_MGF1                                                                     = "1.2.840.113549.1.1.8";
        private  const String szOID_RSA_PSPECIFIED                                                               = "1.2.840.113549.1.1.9";
        private  const String szOID_RSA_SSA_PSS                                                                  = "1.2.840.113549.1.1.10";
        private  const String szOID_RSA_SHA256RSA                                                                = "1.2.840.113549.1.1.11";
        private  const String szOID_RSA_SHA384RSA                                                                = "1.2.840.113549.1.1.12";
        private  const String szOID_RSA_SHA512RSA                                                                = "1.2.840.113549.1.1.13";
        private  const String szOID_PKCS_2                                                                       = "1.2.840.113549.1.2";
        private  const String szOID_PKCS_3                                                                       = "1.2.840.113549.1.3";
        private  const String szOID_PKCS_4                                                                       = "1.2.840.113549.1.4";
        private  const String szOID_PKCS_5                                                                       = "1.2.840.113549.1.5";
        private  const String szOID_PKCS_6                                                                       = "1.2.840.113549.1.6";
        private  const String szOID_PKCS_7                                                                       = "1.2.840.113549.1.7";
        private  const String szOID_PKCS_8                                                                       = "1.2.840.113549.1.8";
        private  const String szOID_PKCS_9                                                                       = "1.2.840.113549.1.9";
        private  const String szOID_PKCS_10                                                                      = "1.2.840.113549.1.10";
        private  const String szOID_PKCS_12                                                                      = "1.2.840.113549.1.12";
        private  const String szOID_RSA_DH                                                                       = "1.2.840.113549.1.3.1";
        private  const String szOID_RSA_emailAddr                                                                = "1.2.840.113549.1.9.1";
        private  const String szOID_RSA_unstructName                                                             = "1.2.840.113549.1.9.2";
        private  const String szOID_RSA_contentType                                                              = "1.2.840.113549.1.9.3";
        private  const String szOID_RSA_messageDigest                                                            = "1.2.840.113549.1.9.4";
        private  const String szOID_RSA_signingTime                                                              = "1.2.840.113549.1.9.5";
        private  const String szOID_RSA_counterSign                                                              = "1.2.840.113549.1.9.6";
        private  const String szOID_RSA_challengePwd                                                             = "1.2.840.113549.1.9.7";
        private  const String szOID_RSA_unstructAddr                                                             = "1.2.840.113549.1.9.8";
        private  const String szOID_RSA_extCertAttrs                                                             = "1.2.840.113549.1.9.9";
        private  const String szOID_RSA_certExtensions                                                           = "1.2.840.113549.1.9.14";
        private  const String szOID_RSA_SMIMECapabilities                                                        = "1.2.840.113549.1.9.15";
        private  const String szOID_RSA_preferSignedData                                                         = "1.2.840.113549.1.9.15.1";
        private  const String szOID_TIMESTAMP_TOKEN                                                              = "1.2.840.113549.1.9.16.1.4";
        private  const String szOID_RSA_SMIMEalg                                                                 = "1.2.840.113549.1.9.16.3";
        private  const String szOID_RSA_SMIMEalgESDH                                                             = "1.2.840.113549.1.9.16.3.5";
        private  const String szOID_RSA_SMIMEalgCMS3DESwrap                                                      = "1.2.840.113549.1.9.16.3.6";
        private  const String szOID_RSA_SMIMEalgCMSRC2wrap                                                       = "1.2.840.113549.1.9.16.3.7";
        private  const String szOID_RSA_MD2                                                                      = "1.2.840.113549.2.2";
        private  const String szOID_RSA_MD4                                                                      = "1.2.840.113549.2.4";
        private  const String szOID_RSA_MD5                                                                      = "1.2.840.113549.2.5";
        private  const String szOID_RSA_RC2CBC                                                                   = "1.2.840.113549.3.2";
        private  const String szOID_RSA_RC4                                                                      = "1.2.840.113549.3.4";
        private  const String szOID_RSA_DES_EDE3_CBC                                                             = "1.2.840.113549.3.7";
        private  const String szOID_RSA_RC5_CBCPad                                                               = "1.2.840.113549.3.9";
        private  const String szOID_RFC3161_counterSign                                                          = "1.3.6.1.4.1.311.3.3.1";
        private  const String szOID_ANSI_X942                                                                    = "1.2.840.10046";
        private  const String szOID_ANSI_X942_DH                                                                 = "1.2.840.10046.2.1";
        private  const String szOID_X957                                                                         = "1.2.840.10040";
        private  const String szOID_X957_DSA                                                                     = "1.2.840.10040.4.1";
        private  const String szOID_X957_SHA1DSA                                                                 = "1.2.840.10040.4.3";
        private  const String szOID_ECC_PUBLIC_KEY                                                               = "1.2.840.10045.2.1";
        private  const String szOID_ECC_CURVE_P256                                                               = "1.2.840.10045.3.1.7";
        private  const String szOID_ECC_CURVE_P384                                                               = "1.3.132.0.34";
        private  const String szOID_ECC_CURVE_P521                                                               = "1.3.132.0.35";
        private  const String szOID_ECDSA_SHA1                                                                   = "1.2.840.10045.4.1";
        private  const String szOID_ECDSA_SPECIFIED                                                              = "1.2.840.10045.4.3";
        private  const String szOID_ECDSA_SHA256                                                                 = "1.2.840.10045.4.3.2";
        private  const String szOID_ECDSA_SHA384                                                                 = "1.2.840.10045.4.3.3";
        private  const String szOID_ECDSA_SHA512                                                                 = "1.2.840.10045.4.3.4";
        private  const String szOID_NIST_AES128_CBC                                                              = "2.16.840.1.101.3.4.1.2";
        private  const String szOID_NIST_AES192_CBC                                                              = "2.16.840.1.101.3.4.1.22";
        private  const String szOID_NIST_AES256_CBC                                                              = "2.16.840.1.101.3.4.1.42";
        private  const String szOID_NIST_AES128_WRAP                                                             = "2.16.840.1.101.3.4.1.5";
        private  const String szOID_NIST_AES192_WRAP                                                             = "2.16.840.1.101.3.4.1.25";
        private  const String szOID_NIST_AES256_WRAP                                                             = "2.16.840.1.101.3.4.1.45";
        private  const String szOID_DH_SINGLE_PASS_STDDH_SHA1_KDF                                                = "1.3.133.16.840.63.0.2";
        private  const String szOID_DH_SINGLE_PASS_STDDH_SHA256_KDF                                              = "1.3.132.1.11.1";
        private  const String szOID_DH_SINGLE_PASS_STDDH_SHA384_KDF                                              = "1.3.132.1.11.2";
        private  const String szOID_DS                                                                           = "2.5";
        private  const String szOID_DSALG                                                                        = szOID_DS + ".8";
        private  const String szOID_DSALG_CRPT                                                                   = szOID_DSALG + ".1";
        private  const String szOID_DSALG_HASH                                                                   = szOID_DSALG + ".2";
        private  const String szOID_DSALG_SIGN                                                                   = szOID_DSALG + ".3";
        private  const String szOID_DSALG_RSA                                                                    = szOID_DSALG + ".1.1";
        private  const String szOID_OIW                                                                          = "1.3.14";
        private  const String szOID_OIWSECSIG                                                                    = szOID_OIW + ".3";
        private  const String szOID_OIWSEC                                                                       = szOID_OIWSECSIG + ".2";
        private  const String szOID_OIWSEC_md4RSA                                                                = szOID_OIWSEC +  ".2";
        private  const String szOID_OIWSEC_md5RSA                                                                = szOID_OIWSEC +  ".3";
        private  const String szOID_OIWSEC_md4RSA2                                                               = szOID_OIWSEC +  ".4";
        private  const String szOID_OIWSEC_desECB                                                                = szOID_OIWSEC +  ".6";
        private  const String szOID_OIWSEC_desCBC                                                                = szOID_OIWSEC +  ".7";
        private  const String szOID_OIWSEC_desOFB                                                                = szOID_OIWSEC +  ".8";
        private  const String szOID_OIWSEC_desCFB                                                                = szOID_OIWSEC +  ".9";
        private  const String szOID_OIWSEC_desMAC                                                                = szOID_OIWSEC + ".10";
        private  const String szOID_OIWSEC_rsaSign                                                               = szOID_OIWSEC + ".11";
        private  const String szOID_OIWSEC_dsa                                                                   = szOID_OIWSEC + ".12";
        private  const String szOID_OIWSEC_shaDSA                                                                = szOID_OIWSEC + ".13";
        private  const String szOID_OIWSEC_mdc2RSA                                                               = szOID_OIWSEC + ".14";
        private  const String szOID_OIWSEC_shaRSA                                                                = szOID_OIWSEC + ".15";
        private  const String szOID_OIWSEC_dhCommMod                                                             = szOID_OIWSEC + ".16";
        private  const String szOID_OIWSEC_desEDE                                                                = szOID_OIWSEC + ".17";
        private  const String szOID_OIWSEC_sha                                                                   = szOID_OIWSEC + ".18";
        private  const String szOID_OIWSEC_mdc2                                                                  = szOID_OIWSEC + ".19";
        private  const String szOID_OIWSEC_dsaComm                                                               = szOID_OIWSEC + ".20";
        private  const String szOID_OIWSEC_dsaCommSHA                                                            = szOID_OIWSEC + ".21";
        private  const String szOID_OIWSEC_rsaXchg                                                               = szOID_OIWSEC + ".22";
        private  const String szOID_OIWSEC_keyHashSeal                                                           = szOID_OIWSEC + ".23";
        private  const String szOID_OIWSEC_md2RSASign                                                            = szOID_OIWSEC + ".24";
        private  const String szOID_OIWSEC_md5RSASign                                                            = szOID_OIWSEC + ".25";
        internal const String szOID_OIWSEC_sha1                                                                  = szOID_OIWSEC + ".26";
        private  const String szOID_OIWSEC_dsaSHA1                                                               = szOID_OIWSEC + ".27";
        private  const String szOID_OIWSEC_dsaCommSHA1                                                           = szOID_OIWSEC + ".28";
        private  const String szOID_OIWSEC_sha1RSASign                                                           = szOID_OIWSEC + ".29";
        private  const String szOID_OIWDIR                                                                       = "1.3.14.7.2";
        private  const String szOID_OIWDIR_CRPT                                                                  = szOID_OIWDIR + ".1";
        private  const String szOID_OIWDIR_HASH                                                                  = szOID_OIWDIR + ".2";
        private  const String szOID_OIWDIR_SIGN                                                                  = szOID_OIWDIR + ".3";
        private  const String szOID_OIWDIR_md2                                                                   = szOID_OIWDIR + ".2.1";
        private  const String szOID_OIWDIR_md2RSA                                                                = szOID_OIWDIR + ".3.1";
        private  const String szOID_INFOSEC                                                                      = "2.16.840.1.101.2.1";
        private  const String szOID_INFOSEC_sdnsSignature                                                        = "2.16.840.1.101.2.1.1.1";
        private  const String szOID_INFOSEC_mosaicSignature                                                      = "2.16.840.1.101.2.1.1.2";
        private  const String szOID_INFOSEC_sdnsConfidentiality                                                  = "2.16.840.1.101.2.1.1.3";
        private  const String szOID_INFOSEC_mosaicConfidentiality                                                = "2.16.840.1.101.2.1.1.4";
        private  const String szOID_INFOSEC_sdnsIntegrity                                                        = "2.16.840.1.101.2.1.1.5";
        private  const String szOID_INFOSEC_mosaicIntegrity                                                      = "2.16.840.1.101.2.1.1.6";
        private  const String szOID_INFOSEC_sdnsTokenProtection                                                  = "2.16.840.1.101.2.1.1.7";
        private  const String szOID_INFOSEC_mosaicTokenProtection                                                = "2.16.840.1.101.2.1.1.8";
        private  const String szOID_INFOSEC_sdnsKeyManagement                                                    = "2.16.840.1.101.2.1.1.9";
        private  const String szOID_INFOSEC_mosaicKeyManagement                                                  = "2.16.840.1.101.2.1.1.10";
        private  const String szOID_INFOSEC_sdnsKMandSig                                                         = "2.16.840.1.101.2.1.1.11";
        private  const String szOID_INFOSEC_mosaicKMandSig                                                       = "2.16.840.1.101.2.1.1.12";
        private  const String szOID_INFOSEC_SuiteASignature                                                      = "2.16.840.1.101.2.1.1.13";
        private  const String szOID_INFOSEC_SuiteAConfidentiality                                                = "2.16.840.1.101.2.1.1.14";
        private  const String szOID_INFOSEC_SuiteAIntegrity                                                      = "2.16.840.1.101.2.1.1.15";
        private  const String szOID_INFOSEC_SuiteATokenProtection                                                = "2.16.840.1.101.2.1.1.16";
        private  const String szOID_INFOSEC_SuiteAKeyManagement                                                  = "2.16.840.1.101.2.1.1.17";
        private  const String szOID_INFOSEC_SuiteAKMandSig                                                       = "2.16.840.1.101.2.1.1.18";
        private  const String szOID_INFOSEC_mosaicUpdatedSig                                                     = "2.16.840.1.101.2.1.1.19";
        private  const String szOID_INFOSEC_mosaicKMandUpdSig                                                    = "2.16.840.1.101.2.1.1.20";
        private  const String szOID_INFOSEC_mosaicUpdatedInteg                                                   = "2.16.840.1.101.2.1.1.21";
        private  const String szOID_NIST_sha256                                                                  = "2.16.840.1.101.3.4.2.1";
        private  const String szOID_NIST_sha384                                                                  = "2.16.840.1.101.3.4.2.2";
        private  const String szOID_NIST_sha512                                                                  = "2.16.840.1.101.3.4.2.3";
        private  const String szOID_CP_GOST_PRIVATE_KEYS_V1                                                      = "1.2.643.2.2.37.1";
        private  const String szOID_CP_GOST_PRIVATE_KEYS_V2                                                      = "1.2.643.2.2.37.2";
        private  const String szOID_CP_GOST_PRIVATE_KEYS_V2_FULL                                                 = "1.2.643.2.2.37.2.1";
        private  const String szOID_CP_GOST_PRIVATE_KEYS_V2_PARTOF                                               = "1.2.643.2.2.37.2.2";
        private  const String szOID_CP_GOST_R3411                                                                = "1.2.643.2.2.9";
        private  const String szOID_CP_GOST_R3411_12_256                                                         = "1.2.643.7.1.1.2.2";
        private  const String szOID_CP_GOST_R3411_12_512                                                         = "1.2.643.7.1.1.2.3";
        private  const String szOID_CP_GOST_28147                                                                = "1.2.643.2.2.21";
        private  const String szOID_CP_GOST_R3412_2015_M                                                         = "1.2.643.7.1.1.5.1";
        private  const String szOID_CP_GOST_R3412_2015_K                                                         = "1.2.643.7.1.1.5.2";
        private  const String szOID_CP_GOST_R3410                                                                = "1.2.643.2.2.20";
        private  const String szOID_CP_GOST_R3410EL                                                              = "1.2.643.2.2.19";
        private  const String szOID_CP_GOST_R3410_12_256                                                         = "1.2.643.7.1.1.1.1";
        private  const String szOID_CP_GOST_R3410_12_512                                                         = "1.2.643.7.1.1.1.2";
        private  const String szOID_CP_DH_EX                                                                     = "1.2.643.2.2.99";
        private  const String szOID_CP_DH_EL                                                                     = "1.2.643.2.2.98";
        private  const String szOID_CP_DH_12_256                                                                 = "1.2.643.7.1.1.6.1";
        private  const String szOID_CP_DH_12_512                                                                 = "1.2.643.7.1.1.6.2";
        private  const String szOID_CP_GOST_R3410_94_ESDH                                                        = "1.2.643.2.2.97";
        private  const String szOID_CP_GOST_R3410_01_ESDH                                                        = "1.2.643.2.2.96";
        private  const String szOID_CP_GOST_R3411_R3410                                                          = "1.2.643.2.2.4";
        private  const String szOID_CP_GOST_R3411_R3410EL                                                        = "1.2.643.2.2.3";
        private  const String szOID_CP_GOST_R3411_12_256_R3410                                                   = "1.2.643.7.1.1.3.2";
        private  const String szOID_CP_GOST_R3411_12_512_R3410                                                   = "1.2.643.7.1.1.3.3";
        private  const String szOID_KP_TLS_PROXY                                                                 = "1.2.643.2.2.34.1";
        private  const String szOID_KP_RA_CLIENT_AUTH                                                            = "1.2.643.2.2.34.2";
        private  const String szOID_KP_WEB_CONTENT_SIGNING                                                       = "1.2.643.2.2.34.3";
        private  const String szOID_KP_RA_ADMINISTRATOR                                                          = "1.2.643.2.2.34.4";
        private  const String szOID_KP_RA_OPERATOR                                                               = "1.2.643.2.2.34.5";
        private  const String szOID_CP_GOST_R3411_94_HMAC                                                        = "1.2.643.2.2.10";
        private  const String szOID_CP_GOST_R3411_2012_256_HMAC                                                  = "1.2.643.7.1.1.4.1";
        private  const String szOID_CP_GOST_R3411_2012_512_HMAC                                                  = "1.2.643.7.1.1.4.2";
        private  const String szOID_OGRN                                                                         = "1.2.643.100.1";
        private  const String szOID_OGRNIP                                                                       = "1.2.643.100.5";
        private  const String szOID_SNILS                                                                        = "1.2.643.100.3";
        private  const String szOID_INN                                                                          = "1.2.643.3.131.1.1";
        private  const String szOID_SIGN_TOOL_KC1                                                                = "1.2.643.100.113.1";
        private  const String szOID_SIGN_TOOL_KC2                                                                = "1.2.643.100.113.2";
        private  const String szOID_SIGN_TOOL_KC3                                                                = "1.2.643.100.113.3";
        private  const String szOID_SIGN_TOOL_KB1                                                                = "1.2.643.100.113.4";
        private  const String szOID_SIGN_TOOL_KB2                                                                = "1.2.643.100.113.5";
        private  const String szOID_SIGN_TOOL_KA1                                                                = "1.2.643.100.113.6";
        private  const String szOID_CA_TOOL_KC1                                                                  = "1.2.643.100.114.1";
        private  const String szOID_CA_TOOL_KC2                                                                  = "1.2.643.100.114.2";
        private  const String szOID_CA_TOOL_KC3                                                                  = "1.2.643.100.114.3";
        private  const String szOID_CA_TOOL_KB1                                                                  = "1.2.643.100.114.4";
        private  const String szOID_CA_TOOL_KB2                                                                  = "1.2.643.100.114.5";
        private  const String szOID_CA_TOOL_KA1                                                                  = "1.2.643.100.114.6";
        private  const String szOID_CEP_BASE_PERSONAL                                                            = "1.2.643.2.2.38.1";
        private  const String szOID_CEP_BASE_NETWORK                                                             = "1.2.643.2.2.38.2";
        private  const String szOID_GostR3411_94_TestParamSet                                                    = "1.2.643.2.2.30.0";
        private  const String szOID_GostR3411_94_CryptoProParamSet                                               = "1.2.643.2.2.30.1";	/* ГОСТ Р 34.11-94, параметры по умолчанию */
        private  const String szOID_GostR3411_94_CryptoPro_B_ParamSet                                            = "1.2.643.2.2.30.2";
        private  const String szOID_GostR3411_94_CryptoPro_C_ParamSet                                            = "1.2.643.2.2.30.3";
        private  const String szOID_GostR3411_94_CryptoPro_D_ParamSet                                            = "1.2.643.2.2.30.4";
        private  const String szOID_Gost28147_89_TestParamSet                                                    = "1.2.643.2.2.31.0";
        private  const String szOID_Gost28147_89_CryptoPro_A_ParamSet                                            = "1.2.643.2.2.31.1";      /* ГОСТ 28147-89, параметры по умолчанию */
        private  const String szOID_Gost28147_89_CryptoPro_B_ParamSet                                            = "1.2.643.2.2.31.2";      /* ГОСТ 28147-89, параметры шифрования 1 */
        private  const String szOID_Gost28147_89_CryptoPro_C_ParamSet                                            = "1.2.643.2.2.31.3";      /* ГОСТ 28147-89, параметры шифрования 2 */
        private  const String szOID_Gost28147_89_CryptoPro_D_ParamSet                                            = "1.2.643.2.2.31.4";      /* ГОСТ 28147-89, параметры шифрования 3 */
        private  const String szOID_Gost28147_89_CryptoPro_Oscar_1_1_ParamSet                                    = "1.2.643.2.2.31.5";      /* ГОСТ 28147-89, параметры Оскар 1.1 */
        private  const String szOID_Gost28147_89_CryptoPro_Oscar_1_0_ParamSet                                    = "1.2.643.2.2.31.6";      /* ГОСТ 28147-89, параметры Оскар 1.0 */
        private  const String szOID_Gost28147_89_CryptoPro_RIC_1_ParamSet                                        = "1.2.643.2.2.31.7";      /* ГОСТ 28147-89, параметры РИК 1 */
        private  const String szOID_Gost28147_89_TC26_A_ParamSet                                                 = "1.2.643.2.2.31.12";     /* ГОСТ 28147-89, параметры шифрования TC26 2 */
        private  const String szOID_Gost28147_89_TC26_B_ParamSet                                                 = "1.2.643.2.2.31.13";     /* ГОСТ 28147-89, параметры шифрования TC26 1 */
        private  const String szOID_Gost28147_89_TC26_C_ParamSet                                                 = "1.2.643.2.2.31.14";     /* ГОСТ 28147-89, параметры шифрования TC26 3 */
        private  const String szOID_Gost28147_89_TC26_D_ParamSet                                                 = "1.2.643.2.2.31.15";     /* ГОСТ 28147-89, параметры шифрования TC26 4 */
        private  const String szOID_Gost28147_89_TC26_E_ParamSet                                                 = "1.2.643.2.2.31.16";     /* ГОСТ 28147-89, параметры шифрования TC26 5 */
        private  const String szOID_Gost28147_89_TC26_F_ParamSet                                                 = "1.2.643.2.2.31.17";     /* ГОСТ 28147-89, параметры шифрования TC26 6 */
        private  const String szOID_Gost28147_89_TC26_Z_ParamSet                                                 = "1.2.643.7.1.2.5.1.1";   /* ГОСТ 28147-89, параметры шифрования ТС26 Z */
        private  const String szOID_GostR3410_94_CryptoPro_A_ParamSet                                            = "1.2.643.2.2.32.2"; 	/*VerbaO*/
        private  const String szOID_GostR3410_94_CryptoPro_B_ParamSet                                            = "1.2.643.2.2.32.3";
        private  const String szOID_GostR3410_94_CryptoPro_C_ParamSet                                            = "1.2.643.2.2.32.4";
        private  const String szOID_GostR3410_94_CryptoPro_D_ParamSet                                            = "1.2.643.2.2.32.5";
        private  const String szOID_GostR3410_94_TestParamSet                                                    = "1.2.643.2.2.32.0"; 	/*Test*/
        private  const String szOID_GostR3410_94_CryptoPro_XchA_ParamSet                                         = "1.2.643.2.2.33.1";
        private  const String szOID_GostR3410_94_CryptoPro_XchB_ParamSet                                         = "1.2.643.2.2.33.2";
        private  const String szOID_GostR3410_94_CryptoPro_XchC_ParamSet                                         = "1.2.643.2.2.33.3";
        private  const String szOID_GostR3410_2001_TestParamSet                                                  = "1.2.643.2.2.35.0";      /* ГОСТ Р 34.10-2001, тестовые параметры */
        private  const String szOID_GostR3410_2001_CryptoPro_A_ParamSet                                          = "1.2.643.2.2.35.1";	/* ГОСТ Р 34.10-2001, параметры по умолчанию */
        private  const String szOID_GostR3410_2001_CryptoPro_B_ParamSet                                          = "1.2.643.2.2.35.2";	/* ГОСТ Р 34.10-2001, параметры Оскар 2.x */
        private  const String szOID_GostR3410_2001_CryptoPro_C_ParamSet                                          = "1.2.643.2.2.35.3";	/* ГОСТ Р 34.10-2001, параметры подписи 1 */
        private  const String szOID_tc26_gost_3410_12_256_paramSetA                                              = "1.2.643.7.1.2.1.1.1";	/* ГОСТ Р 34.10-2012, 256 бит, параметры ТК-26, набор A */
        private  const String szOID_tc26_gost_3410_12_512_paramSetA                                              = "1.2.643.7.1.2.1.2.1";	/* ГОСТ Р 34.10-2012, 512 бит, параметры по умолчанию */
        private  const String szOID_tc26_gost_3410_12_512_paramSetB                                              = "1.2.643.7.1.2.1.2.2";	/* ГОСТ Р 34.10-2012, 512 бит, параметры ТК-26, набор B */
        private  const String szOID_tc26_gost_3410_12_512_paramSetC                                              = "1.2.643.7.1.2.1.2.3";	/* ГОСТ Р 34.10-2012, 512 бит, параметры ТК-26, набор С */
        private  const String szOID_GostR3410_2001_CryptoPro_XchA_ParamSet                                       = "1.2.643.2.2.36.0";	/* ГОСТ Р 34.10-2001, параметры обмена по умолчанию */
        private  const String szOID_GostR3410_2001_CryptoPro_XchB_ParamSet                                       = "1.2.643.2.2.36.1";	/* ГОСТ Р 34.10-2001, параметры обмена 1 */
        private  const String szOID_CryptoPro_private_keys_extension_intermediate_store                          = "1.2.643.2.2.37.3.1";
        private  const String szOID_CryptoPro_private_keys_extension_signature_trust_store                       = "1.2.643.2.2.37.3.2";
        private  const String szOID_CryptoPro_private_keys_extension_exchange_trust_store                        = "1.2.643.2.2.37.3.3";
        private  const String szOID_CryptoPro_private_keys_extension_container_friendly_name                     = "1.2.643.2.2.37.3.4";
        private  const String szOID_CryptoPro_private_keys_extension_container_key_usage_period                  = "1.2.643.2.2.37.3.5";
        private  const String szOID_CryptoPro_private_keys_extension_container_uec_symmetric_key_derive_counter  = "1.2.643.2.2.37.3.6";
        private  const String szOID_CryptoPro_private_keys_extension_container_primary_key_properties            = "1.2.643.2.2.37.3.7";
        private  const String szOID_CryptoPro_private_keys_extension_container_secondary_key_properties          = "1.2.643.2.2.37.3.8";
        private  const String szOID_CryptoPro_private_keys_extension_container_signature_key_usage_period        = "1.2.643.2.2.37.3.9";
        private  const String szOID_CryptoPro_private_keys_extension_container_exchange_key_usage_period         = "1.2.643.2.2.37.3.10";
        private  const String szOID_CryptoPro_private_keys_extension_container_key_time_validity_control_mode    = "1.2.643.2.2.37.3.11";
        private  const String szOID_CryptoPro_extensions_certificate_and_crl_matching_technique                  = "1.2.643.2.2.49.1";
        private  const String szCPOID_SubjectSignTool                                                            = "1.2.643.100.111";
        private  const String szCPOID_IssuerSignTool                                                             = "1.2.643.100.112";
        private  const String szCPOID_RSA_SMIMEaaSigningCertificate                                              = "1.2.840.113549.1.9.16.2.12";
        private  const String szCPOID_RSA_SMIMEaaSigningCertificateV2                                            = "1.2.840.113549.1.9.16.2.47";
        private  const String szCPOID_RSA_SMIMEaaETSotherSigCert                                                 = "1.2.840.113549.1.9.16.2.19";

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartObject();
            writer.WriteBlock(serializer, new []
                {
                nameof(SignatureAlgorithm),
                nameof(HashAlgorithm)
                }, new []
                {
                SignatureAlgorithm.ToString(),
                HashAlgorithm?.ToString()
                }, new []
                {
                SignatureAlgorithm.FriendlyName,
                HashAlgorithm?.FriendlyName
                });
            //WriteValue(writer, serializer, nameof(SignatureAlgorithm), SignatureAlgorithm.ToString());
            //writer.WriteIndentSpace(1);
            //writer.WriteComment($" {SignatureAlgorithm.FriendlyName} ");
            //WriteValue(writer, serializer, nameof(HashAlgorithm), HashAlgorithm.ToString());
            //writer.WriteIndentSpace(6);
            //writer.WriteComment($" {HashAlgorithm.FriendlyName} ");
            writer.WriteEndObject();
            }
        }
    }