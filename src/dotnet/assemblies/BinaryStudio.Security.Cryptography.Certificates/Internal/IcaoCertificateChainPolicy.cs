using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using BinaryStudio.PlatformComponents.Win32;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates.Internal
    {
    internal class IcaoCertificateChainPolicy : X509CertificateChainPolicy
        {
        public override unsafe void Verify(ICryptographicContext context,
            OidCollection applicationpolicy, OidCollection certificatepolicy,
            TimeSpan timeout, DateTime datetime, IX509CertificateStorage store,
            CERT_CHAIN_FLAGS flags, ref CERT_CHAIN_CONTEXT chaincontext) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            var policypara = new CERT_CHAIN_POLICY_PARA {
                Size = sizeof(CERT_CHAIN_POLICY_PARA),
                Flags = 0,
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
                new IntPtr((Int32)CERT_CHAIN_POLICY.CERT_CHAIN_POLICY_BASE),
                ref chaincontext,
                ref policypara,
                ref policystatus));
            if (chaincontext.TrustStatus.ErrorStatus != 0) {
                var source = new X509CertificateChainContext(ref chaincontext);
                var target = new HashSet<Exception>();
                var storage = new X509CombinedCertificateStorage(store,
                    new CertificateStorageContext(X509StoreName.Root, X509StoreLocation.LocalMachine),
                    new CertificateStorageContext(X509StoreName.CertificateAuthority, X509StoreLocation.LocalMachine));
                foreach (var chain in source) {
                    try
                        {
                        if (chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN)) { RaiseExceptionForStatus(chain.ErrorStatus, 0x000f0000); }
                        var c = chain.Count;
                        if (c > 1) {
                            for (var i = 0; i < c - 1; i++) {
                                if (chain[i].ErrorStatus != 0) {
                                    var subject = chain[i    ].Certificate;
                                    var issuer  = chain[i + 1].Certificate;
                                    context.VerifySignature(subject, issuer, CRYPT_VERIFY_CERT_SIGN.NONE);
                                    if (chain[i].ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION)) {
                                        var crls = new List<String>();
                                        var country = issuer.Country;
                                        issuer.Issuer.TryGetValue("2.5.4.10", out var isr_o);
                                        foreach (var crl in storage.CertificateRevocationLists.Where(o =>
                                             (o.Country == country)
                                            && o.Issuer.TryGetValue("2.5.4.10", out var crl_o)
                                            && String.Equals(crl_o, isr_o, StringComparison.OrdinalIgnoreCase)))
                                            {
                                            crls.Add(ToString(crl));
                                            var status = new List<String>();
                                            if (crl.EffectiveDate >= datetime) {
                                                status.Add("actual");
                                                if (context.VerifySignature(out var e, crl, issuer, CRYPT_VERIFY_CERT_SIGN.NONE)) {
                                                    status.Add("valid");
                                                    }
                                                else
                                                    {
                                                    status.Add("invalid");
                                                    }
                                                }
                                            else
                                                {
                                                status.Add("expired");
                                                }
                                            Console.WriteLine($"crl:{String.Join(",", status)}:{ToString(crl)}");
                                            }
                                        }
                                    //RaiseExceptionForStatus(chain[i].ErrorStatus, 0xffffffff);
                                    //if (chain[i].CertificateRevocationList == null) {
                                    //    throw new InvalidDataException("no crl");
                                    //    }
                                    }
                                }
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
                    e.Data["Policy"] = "ICAO";
                    throw e;
                    }
                }
            }
        }
    }