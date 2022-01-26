using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using BinaryStudio.IO;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates.Internal;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    /// <summary>
    /// <see cref="X509CertificateChainPolicy"/> class represents a certificate chain verification policy.
    /// </summary>
    public abstract class X509CertificateChainPolicy : IX509CertificateChainPolicy
        {
        public static readonly IX509CertificateChainPolicy POLICY_BASE              = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_BASE);
        public static readonly IX509CertificateChainPolicy POLICY_AUTHENTICODE      = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_AUTHENTICODE);
        public static readonly IX509CertificateChainPolicy POLICY_AUTHENTICODE_TS   = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_AUTHENTICODE_TS);
        public static readonly IX509CertificateChainPolicy POLICY_SSL               = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_SSL);
        public static readonly IX509CertificateChainPolicy POLICY_BASIC_CONSTRAINTS = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_BASIC_CONSTRAINTS);
        public static readonly IX509CertificateChainPolicy POLICY_NT_AUTH           = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_NT_AUTH);
        public static readonly IX509CertificateChainPolicy POLICY_MICROSOFT_ROOT    = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_MICROSOFT_ROOT);
        public static readonly IX509CertificateChainPolicy POLICY_EV                = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_EV);
        public static readonly IX509CertificateChainPolicy POLICY_SSL_F12           = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_F12);
        public static readonly IX509CertificateChainPolicy POLICY_SSL_HPKP_HEADER   = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_HPKP_HEADER);
        public static readonly IX509CertificateChainPolicy POLICY_THIRD_PARTY_ROOT  = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_THIRD_PARTY_ROOT);
        public static readonly IX509CertificateChainPolicy POLICY_SSL_KEY_PIN       = new DefaultCertificateChainPolicy(CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_KEY_PIN);
        public static readonly IX509CertificateChainPolicy IcaoCertificateChainPolicy = new IcaoCertificateChainPolicy();

        public abstract CertificateChainPolicy Policy { get; }

        public abstract void Verify(ICryptographicContext context,
            DateTime datetime, IX509CertificateStorage store,
            CERT_CHAIN_FLAGS flags, ref CERT_CHAIN_CONTEXT chaincontext);

        #region M:Validate([Out]Exception,Boolean)
        protected virtual void Validate(Boolean status) {
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
        #region M:GetLastWin32Error:Win32ErrorCode
        protected static Win32ErrorCode GetLastWin32Error()
            {
            return (Win32ErrorCode)Marshal.GetLastWin32Error();
            }
        #endregion
        #region M:ToString(CERT_CONTEXT):String
        protected static unsafe String ToString(ref CERT_CONTEXT context) {
            var r = new StringBuilder();
            if (context.CertEncodedSize > 0) {
                var size  = context.CertEncodedSize;
                var bytes = context.CertEncoded;
                var buffer = new Byte[size];
                for (var i = 0U; i < size; ++i) {
                    buffer[i] = bytes[i];
                    }
                var o = new Asn1Certificate(Asn1Object.Load(new ReadOnlyMemoryMappingStream(buffer)).FirstOrDefault());
                if (o.IsFailed) { throw new NotImplementedException(); }
                r.Append($"SerialNumber:{{{o.SerialNumber.ToString().ToLowerInvariant()}}},");
                r.Append($"Subject:{{{o.Subject}}},");
                r.Append($"Issuer:{{{o.Issuer}}}");
                }
            return r.ToString();
            }
        #endregion
        #region M:ToString(HRESULT):String
        protected static String ToString(HRESULT source) {
            var r = new StringBuilder();
            if (Enum.IsDefined(typeof(HRESULT), source))
                {
                r.Append(source);
                }
            r.Append("{");
            r.Append(((Int32)source).ToString("x8"));
            r.Append("}");
            return r.ToString();
            }
        #endregion
        #region M:ToString(CRL_CONTEXT):String
        protected static unsafe String ToString(ref CRL_CONTEXT context) {
            var r = new StringBuilder();
            if (context.CrlEncodedSize > 0) {
                var size  = context.CrlEncodedSize;
                var bytes = context.CrlEncodedData;
                var buffer = new Byte[size];
                for (var i = 0U; i < size; ++i) {
                    buffer[i] = bytes[i];
                    }
                var o = new Asn1CertificateRevocationList(Asn1Object.Load(new ReadOnlyMemoryMappingStream(buffer)).FirstOrDefault());
                if (o.IsFailed) { throw new NotImplementedException(); }
                r.Append($"EffectiveDate:{{{o.EffectiveDate:yyyy-MM-ddThh:mm:ss}}},");
                if (o.NextUpdate.HasValue)
                    {
                    r.Append($"NextUpdate:{{{o.NextUpdate.Value:yyyy-MM-ddThh:mm:ss}}},");
                    }
                r.Append($"Issuer:{{{o.Issuer}}}");
                }
            return r.ToString();
            }
        #endregion

        protected static String ToString(IX509CertificateRevocationList value) {
            var r = new StringBuilder();
            r.Append($"EffectiveDate:{{{value.EffectiveDate:yyyy-MM-ddThh:mm:ss}}},");
            if (value.NextUpdate.HasValue)
                {
                r.Append($"NextUpdate:{{{value.NextUpdate.Value:yyyy-MM-ddThh:mm:ss}}},");
                }
            r.Append($"Issuer:{{{value.Issuer}}}");
            return r.ToString();
            }

        protected static unsafe Asn1Certificate BuildCertificate(ref CERT_CONTEXT context)
            {
            var size  = context.CertEncodedSize;
            var bytes = context.CertEncoded;
            var buffer = new Byte[size];
            for (var i = 0U; i < size; ++i) {
                buffer[i] = bytes[i];
                }
            var o = new Asn1Certificate(Asn1Object.Load(new ReadOnlyMemoryMappingStream(buffer)).FirstOrDefault());
            if (o.IsFailed) { throw new NotImplementedException(); }
            return o;
            }

        private static readonly IDictionary<CertificateChainErrorStatus,Type> types = new Dictionary<CertificateChainErrorStatus,Type>();
        static X509CertificateChainPolicy()
            {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(i => i.IsSubclassOf(typeof(Exception)))) {
                var status = (CertificateChainErrorStatusAttribute)type.GetCustomAttributes(typeof(CertificateChainErrorStatusAttribute),false).FirstOrDefault();
                if (status != null) {
                    try
                        {
                        types.Add(status.Status,type);
                        }
                    catch(ArgumentException e)
                        {
                        e.Data["Key"] = status.Status;
                        e.Data["Type"] = type.FullName;
                        if (types.TryGetValue(status.Status, out var r)) {
                            e.Data["ExistingType"] = r.FullName;
                            }
                        throw;
                        }
                    }
                }
            }

        private static Exception GetExceptionForStatus(CertificateChainErrorStatus status)
            {
            var e = (Exception)Activator.CreateInstance(types[status],
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic,
                null,
                null,
                null);
            e.Data["Status"] = status;
            return e;
            }

        protected static void RaiseExceptionForStatus(CertificateChainErrorStatus status, UInt32 mask)
            {
            var i = ((Int32)status) & mask;
            if (i == 0) { return; }
            var exceptions = new List<Exception>();
            if ((i & (Int32)CertificateChainErrorStatus.TrustIsNotTimeValid)                 != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.TrustIsNotTimeValid));                 i &= ~(Int32)CertificateChainErrorStatus.TrustIsNotTimeValid;                 }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED)                != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED));                i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED;                }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED)                        != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED));                        i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED;                        }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID)            != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID));            i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID;            }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE)            != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE));            i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE;            }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT)                 != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT));                 i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT;                 }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN)         != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN));         i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN;         }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC)                         != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC));                         i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC;                         }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION)                 != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION));                 i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION;                 }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS)        != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS));        i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS;        }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS)         != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS));         i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS;         }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS)          != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS));          i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS;          }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT) != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT)); i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT; }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT)   != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT));   i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT;   }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT) != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT)); i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT; }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT)      != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT));      i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT;      }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION)             != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION));             i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION;             }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY)          != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY));          i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY;          }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST)              != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST));              i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST;              }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT)    != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT));    i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT;    }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN)                  != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN));                  i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN;                  }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID)             != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID));             i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID;             }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID)        != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID));        i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID;        }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE)        != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE));        i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE;        }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE)                != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE));                i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE;                }
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE)                  != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE));                  i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE;                  }
            if (i != 0) { throw new NotSupportedException(); }
            throw ((exceptions.Count == 1)
                ? exceptions[0]
                : new AggregateException(exceptions));
            }
        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        [DllImport("crypt32.dll", ExactSpelling = true, SetLastError = true)] protected static extern Boolean CertVerifyCertificateChainPolicy([In] IntPtr policy, [In] ref CERT_CHAIN_CONTEXT chaincontext, [In] ref CERT_CHAIN_POLICY_PARA policypara, [In][Out] ref CERT_CHAIN_POLICY_STATUS policystatus);
        [DllImport("crypt32.dll", ExactSpelling = true, SetLastError = true)] protected static extern unsafe Boolean CertVerifyCRLRevocation(UInt32 CertEncodingType, IntPtr CertId,Int32 CrlInfoCount,CRL_INFO** CrlInfoArray);

        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 FORMAT_MESSAGE_FROM_HMODULE    = 0x00000800;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;
        protected const UInt32 X509_ASN_ENCODING              = 0x00000001;
        protected const UInt32 PKCS_7_ASN_ENCODING            = 0x00010000;
        }
    }