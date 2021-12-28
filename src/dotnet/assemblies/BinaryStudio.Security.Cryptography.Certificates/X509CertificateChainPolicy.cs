using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class X509CertificateChainPolicy : IX509CertificateChainPolicy
        {
        public static readonly IX509CertificateChainPolicy POLICY_BASE              = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_BASE);
        public static readonly IX509CertificateChainPolicy POLICY_AUTHENTICODE      = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_AUTHENTICODE);
        public static readonly IX509CertificateChainPolicy POLICY_AUTHENTICODE_TS   = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_AUTHENTICODE_TS);
        public static readonly IX509CertificateChainPolicy POLICY_SSL               = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_SSL);
        public static readonly IX509CertificateChainPolicy POLICY_BASIC_CONSTRAINTS = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_BASIC_CONSTRAINTS);
        public static readonly IX509CertificateChainPolicy POLICY_NT_AUTH           = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_NT_AUTH);
        public static readonly IX509CertificateChainPolicy POLICY_MICROSOFT_ROOT    = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_MICROSOFT_ROOT);
        public static readonly IX509CertificateChainPolicy POLICY_EV                = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_EV);
        public static readonly IX509CertificateChainPolicy POLICY_SSL_F12           = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_SSL_F12);
        public static readonly IX509CertificateChainPolicy POLICY_SSL_HPKP_HEADER   = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_SSL_HPKP_HEADER);
        public static readonly IX509CertificateChainPolicy POLICY_THIRD_PARTY_ROOT  = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_THIRD_PARTY_ROOT);
        public static readonly IX509CertificateChainPolicy POLICY_SSL_KEY_PIN       = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_SSL_KEY_PIN);
        public static readonly IX509CertificateChainPolicy IcaoCertificateChainPolicy = new IcaoCertificateChainPolicy();

        public abstract void Verify(ICryptographicContext context,
            OidCollection applicationpolicy, OidCollection certificatepolicy,
            TimeSpan timeout, DateTime datetime, IX509CertificateStorage store,
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

        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);

        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 FORMAT_MESSAGE_FROM_HMODULE    = 0x00000800;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;
        }
    }