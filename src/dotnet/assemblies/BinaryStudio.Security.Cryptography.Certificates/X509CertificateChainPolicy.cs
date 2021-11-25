using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates.Internal;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public abstract class X509CertificateChainPolicy : IX509CertificateChainPolicy
        {
        public readonly IX509CertificateChainPolicy POLICY_BASE              = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_BASE);
        public readonly IX509CertificateChainPolicy POLICY_AUTHENTICODE      = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_AUTHENTICODE);
        public readonly IX509CertificateChainPolicy POLICY_AUTHENTICODE_TS   = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_AUTHENTICODE_TS);
        public readonly IX509CertificateChainPolicy POLICY_SSL               = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_SSL);
        public readonly IX509CertificateChainPolicy POLICY_BASIC_CONSTRAINTS = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_BASIC_CONSTRAINTS);
        public readonly IX509CertificateChainPolicy POLICY_NT_AUTH           = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_NT_AUTH);
        public readonly IX509CertificateChainPolicy POLICY_MICROSOFT_ROOT    = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_MICROSOFT_ROOT);
        public readonly IX509CertificateChainPolicy POLICY_EV                = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_EV);
        public readonly IX509CertificateChainPolicy POLICY_SSL_F12           = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_SSL_F12);
        public readonly IX509CertificateChainPolicy POLICY_SSL_HPKP_HEADER   = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_SSL_HPKP_HEADER);
        public readonly IX509CertificateChainPolicy POLICY_THIRD_PARTY_ROOT  = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_THIRD_PARTY_ROOT);
        public readonly IX509CertificateChainPolicy POLICY_SSL_KEY_PIN       = new DefaultCertificateChainPolicy(CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_SSL_KEY_PIN);
        public readonly IX509CertificateChainPolicy IcaoCertificateChainPolicy = new IcaoCertificateChainPolicy();

        public abstract Boolean Verify(ISet<Exception> target, ICryptographicContext context,
            OidCollection applicationpolicy, OidCollection certificatepolicy,
            TimeSpan timeout, DateTime datetime, IX509CertificateStorage store,
            CERT_CHAIN_FLAGS flags, ref CERT_CHAIN_CONTEXT chaincontext);

        #region M:Validate([Out]Exception,Boolean)
        protected virtual Boolean Validate(out Exception e, Boolean status) {
            e = null;
            if (!status) {
                var i = (Int32)GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]");
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]");
                    }
                }
            return status;
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

        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);

        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 FORMAT_MESSAGE_FROM_HMODULE    = 0x00000800;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;
        }
    }