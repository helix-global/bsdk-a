using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        protected CERT_CHAIN_POLICY_FLAGS Flags { get; }

        public DefaultCertificateChainPolicy(CERT_CHAIN_POLICY policy, CERT_CHAIN_POLICY_FLAGS flags = 0) {
            Policy = policy;
            Flags = flags;
            }

        public override unsafe void Verify(ICryptographicContext context,
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
            Validate(CertVerifyCertificateChainPolicy(
                new IntPtr((Int32)Policy),
                ref chaincontext,
                ref policypara,
                ref policystatus));
            var source = new X509CertificateChainContext(ref chaincontext);
            if (source.ErrorStatus != 0) {
                var target = new HashSet<Exception>();
                foreach (var chain in source) {
                    try
                        {
                        var exceptions = new List<Exception>();
                        foreach (var chainE in chain) {
                            try
                                {
                                RaiseExceptionForStatus(chainE.ErrorStatus, 0xffffffff);
                                }
                            catch (Exception e)
                                {
                                e.Data["ChainElement"] = chainE;
                                exceptions.Add(e);
                                }
                            }
                        try
                            {
                            RaiseExceptionForStatus(chain.ErrorStatus, 0x000f0000);
                            }
                        catch (Exception e)
                            {
                            exceptions.Add(e);
                            }
                        if (exceptions.Count > 0) {
                            throw ((exceptions.Count == 1)
                                ? exceptions[0]
                                : new AggregateException(exceptions));
                            }
                        }
                    catch (Exception e)
                        {
                        e.Data["Chain"] = chain;
                        target.Add(e);
                        }
                    }
                if (target.Any())
                    {
                    var e = new SecurityException("Certificate chain verification error.", target);
                    e.Data["Policy"] = Policy;
                    throw e;
                    }
                }
            }
        }
    }