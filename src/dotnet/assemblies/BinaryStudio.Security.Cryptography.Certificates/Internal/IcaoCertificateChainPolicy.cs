using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
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
                            var exceptions = new List<Exception>();
                            for (var i = 0; i < c - 1; i++) {
                                try
                                    {
                                    Verify(chain, i, storage, datetime, context);
                                    }
                                catch (Exception e)
                                    {
                                    exceptions.Add(e);
                                    }
                                }
                            if (exceptions.Any())
                                {
                                throw new SecurityException("Certificate chain verification error.", exceptions);
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

        private void Verify(X509CertificateChain chain, Int32 index, IX509CertificateStorage store, DateTime datetime, ICryptographicContext context)
            {
            if (chain[index].ErrorStatus != 0) {
                try
                    {
                    var subject = chain[index    ].Certificate;
                    var issuer  = chain[index + 1].Certificate;
                    if (chain[index].ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION)) {
                        VerifyCRL(subject, issuer, store, datetime, context);
                        }
                    else
                        {
                        RaiseExceptionForStatus(chain[index].ErrorStatus, 0xffffffff);
                        }
                    }
                catch (Exception e)
                    {
                    e.Data["ChainElement"] = chain[index];
                    throw;
                    }
                }
            }

        private void VerifyCRL(X509Certificate subject, X509Certificate issuer, IX509CertificateStorage store, DateTime datetime, ICryptographicContext context)
            {
            var exceptions = new List<Exception>();
            var country = issuer.Country;
            var isr_o = GetO(issuer.Subject);
            foreach (var i in store.CertificateRevocationLists.Where(i => (i.Country == country) && String.Equals(GetO(i.Issuer), isr_o, StringComparison.OrdinalIgnoreCase))) {
                var descriptor = ToString(i);
                var status = new List<String>();
                try
                    {
                    if (i.EffectiveDate <= datetime) {
                        if (i.NextUpdate != null) {
                            if (i.NextUpdate.Value >= datetime) {
                                status.Add("actual");
                                if (context.VerifySignature(out var e, i, issuer, CRYPT_VERIFY_CERT_SIGN.NONE)) {
                                    status.Add("valid");
                                    throw new NotImplementedException();
                                    }
                                else
                                    {
                                    if ((HRESULT)Marshal.GetHRForException(e) == HRESULT.NTE_BAD_SIGNATURE) {
                                        /* issuer is not same */
                                        var a = i.Extensions.OfType<CertificateAuthorityKeyIdentifier>().FirstOrDefault();
                                        if (a != null) {
                                            var certificate = store.Certificates.FirstOrDefault(j => {
                                                if (j.Issuer.Equals(a.CertificateIssuer)) {

                                                    }
                                                return false;
                                                });
                                            }
                                        }
                                    status.Add("invalid");
                                    throw e;
                                    }
                                }
                            else
                                {
                                status.Add("expired");
                                throw (new CrlExpiredException("This certificate revocation list has expired.")).
                                    Add("NextUpdate", i.NextUpdate.Value.ToString("O"));
                                }
                            }
                        else
                            {
                            status.Add("skip");
                            }
                        }
                    else
                        {
                        status.Add("expired");
                        throw (new CrlInvalidTimeException("This certificate revocation list is invalid due effective date.")).
                            Add("EffectiveDate", i.EffectiveDate.ToString("O"));
                        }
                    }
                catch (Exception e)
                    {
                    e.Data["CRL"] = descriptor;
                    exceptions.Add(e);
                    }
                Console.WriteLine($"crl:{String.Join(",", status)}:{ToString(i)}");
                }
            if (exceptions.Any())
                {
                var e = new SecurityException("Certificate revocation list verification error.", exceptions);
                throw e;
                }
            }

        #region M:GetO(IX509RelativeDistinguishedNameSequence):String
        private static String GetO(IX509RelativeDistinguishedNameSequence source) {
            source.TryGetValue("2.5.4.10", out var r);
            return r;
            }
        #endregion
        }
    }