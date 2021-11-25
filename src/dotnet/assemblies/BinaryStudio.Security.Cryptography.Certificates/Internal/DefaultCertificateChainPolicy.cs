using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BinaryStudio.PlatformComponents.Win32;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates.Internal
    {
    [DebuggerDisplay(@"\{{Policy}\}")]
    internal class DefaultCertificateChainPolicy : X509CertificateChainPolicy
        {
        private CERT_CHAIN_POLICY Policy { get; }
        protected CERT_CHAIN_POLICY_FLAGS Flags { get { return 0; }}

        public DefaultCertificateChainPolicy(CERT_CHAIN_POLICY policy) {
            Policy = policy;
            }

        public override unsafe Boolean Verify(ISet<Exception> target, ICryptographicContext context,
            OidCollection applicationpolicy, OidCollection certificatepolicy,
            TimeSpan timeout, DateTime datetime, IX509CertificateStorage store,
            CERT_CHAIN_FLAGS flags, ref CERT_CHAIN_CONTEXT chaincontext)
            {
            var policypara = new CERT_CHAIN_POLICY_PARA {
                Size = sizeof(CERT_CHAIN_POLICY_PARA),
                Flags = Flags,
                ExtraPolicyPara = IntPtr.Zero
                };
            var policystatus = new CERT_CHAIN_POLICY_STATUS {
                Size = sizeof(CERT_CHAIN_POLICY_STATUS),
                ChainIndex = 0,
                ElementIndex = 0,
                Error = 0,
                ExtraPolicyStatus = IntPtr.Zero
                };
            var r = Validate(out var e, CertVerifyCertificateChainPolicy(
                new IntPtr((Int32)Policy),
                ref chaincontext,
                ref policypara,
                ref policystatus));
            if (!r || (e != null)) {
                target.Add(e);
                }
            return r;
            }

        [DllImport("crypt32.dll", ExactSpelling = true, SetLastError = true)] private static extern Boolean CertVerifyCertificateChainPolicy([In] IntPtr policy, [In] ref CERT_CHAIN_CONTEXT chaincontext, [In] ref CERT_CHAIN_POLICY_PARA policypara, [In][Out] ref CERT_CHAIN_POLICY_STATUS policystatus);
        }
    }