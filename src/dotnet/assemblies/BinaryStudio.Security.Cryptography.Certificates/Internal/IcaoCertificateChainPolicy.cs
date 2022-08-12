using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using BinaryStudio.Diagnostics;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Serialization;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates.Internal
    {
    internal class IcaoCertificateChainPolicy : X509CertificateChainPolicy
        {
        public override CertificateChainPolicy Policy { get { return CertificateChainPolicy.Icao; }}
        public override unsafe void Verify(ICryptographicContext context,
            DateTime datetime, IX509CertificateStorage store,
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
                new IntPtr((Int32)CertificateChainPolicy.CERT_CHAIN_POLICY_BASE),
                ref chaincontext,
                ref policypara,
                ref policystatus));
            if (chaincontext.TrustStatus.ErrorStatus != 0) {
                var source = new X509CertificateChainContext(ref chaincontext);
                var target = new HashSet<Exception>();
                foreach (var chain in source) {
                    Thread.Sleep(0);
                    try
                        {
                        if (chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN)) { RaiseExceptionForStatus(chain.ErrorStatus, 0x000f0000); }
                        var c = chain.Count;
                        if (c > 1) {
                            var exceptions = new List<Exception>();
                            for (var i = 0; i < c - 1; i++) {
                                try
                                    {
                                    Thread.Sleep(0);
                                    Verify(chain, i, store, datetime, context);
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

        #region M:Verify(IX509CertificateChain,Int32,IX509CertificateStorage,DateTime,ICryptographicContext)
        private void Verify(IX509CertificateChain chain, Int32 index, IX509CertificateStorage store, DateTime datetime, ICryptographicContext context)
            {
            if (chain[index].ErrorStatus != 0) {
                try
                    {
                    var subject = chain[index    ].Certificate;
                    var issuer  = chain[index + 1].Certificate;
                    if (chain[index].ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION) ||
                        chain[index].ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN))
                        {
                        Verify(subject, issuer, store, datetime, context);
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
        #endregion
        #region M:Verify(IX509Certificate,X509Certificate,IX509CertificateStorage,DateTime,ICryptographicContext)
        private void Verify(IX509Certificate subject, IX509Certificate issuer, IX509CertificateStorage store, DateTime datetime, ICryptographicContext context)
            {
            var exceptions = new List<Exception>();
            var country = issuer.Country;
            var isr_o = GetO(((X509Certificate)issuer).Subject);
            foreach (var i in store.CertificateRevocationLists.Where(i => (i.Country == country) && String.Equals(GetO(i.Issuer), isr_o, StringComparison.OrdinalIgnoreCase))) {
                Thread.Sleep(0);
                var descriptor = ToString(i);
                try
                    {
                    if (i.EffectiveDate <= datetime) {
                        if (i.NextUpdate != null) {
                            if (i.NextUpdate.Value >= datetime) {
                                if (context.VerifySignature(out var e, i, issuer, CRYPT_VERIFY_CERT_SIGN.NONE)) {
                                    Verify(subject, i);
                                    return;
                                    }
                                if ((HRESULT)Marshal.GetHRForException(e) == HRESULT.NTE_BAD_SIGNATURE) {
                                    /* issuer is not same */
                                    var aki = i.Extensions.OfType<CertificateAuthorityKeyIdentifier>().FirstOrDefault();
                                    if (aki != null) {
                                        var certificate = store.Certificates.FirstOrDefault(j => {
                                            Thread.Sleep(0);
                                            if ((aki.CertificateIssuer != null) && (aki.SerialNumber != null)) {
                                                return (j.SerialNumber == aki.SerialNumber) &&
                                                    j.Issuer.Equals(aki.CertificateIssuer);
                                                }
                                            if (aki.KeyIdentifier != null) {
                                                var ski = j.Extensions.OfType<CertificateSubjectKeyIdentifier>().FirstOrDefault();
                                                if (ski != null) {
                                                    if (String.Equals(
                                                        aki.KeyIdentifier.ToString("x"),
                                                        ski.KeyIdentifier.ToString("x")))
                                                        {
                                                        return true;
                                                        }
                                                    }
                                                }
                                            return false;
                                            });
                                        if (certificate == null) {
                                            throw (new CertificateMissingException("Cannot find certificate revocation list issuer.")).
                                                Add("SerialNumber", aki.SerialNumber).
                                                Add("Issuer", aki.CertificateIssuer?.ToString()).
                                                Add("AuthorityKeyIdentifier", aki.KeyIdentifier?.ToString("x"));
                                            }
                                        if (!context.VerifySignature(out var x, i, certificate, CRYPT_VERIFY_CERT_SIGN.NONE)) { throw new AggregateException(e,x); }
                                        Verify(subject, i);
                                        return;
                                        }
                                    }
                                throw e;
                                }
                            throw (new CrlExpiredException("This certificate revocation list has expired.")).
                                Add("NextUpdate", i.NextUpdate.Value.ToString("O"));
                            }
                        }
                    else
                        {
                        throw (new CrlInvalidTimeException("This certificate revocation list is invalid due effective date.")).
                            Add("EffectiveDate", i.EffectiveDate.ToString("O"));
                        }
                    }
                catch (Exception e)
                    {
                    e.Data["CRL"] = descriptor;
                    exceptions.Add(e);
                    }
                }
            if (exceptions.Any())
                {
                var e = new SecurityException("Certificate revocation list verification error.", exceptions);
                throw e;
                }
            }
        #endregion
        #region M:Verify(IX509Certificate,IX509CertificateRevocationList)
        private unsafe void Verify(IX509Certificate subject, IX509CertificateRevocationList crl) {
            var context = (CRL_CONTEXT*)crl.Handle;
            Validate(CertVerifyCRLRevocation(
                X509_ASN_ENCODING|PKCS_7_ASN_ENCODING,
                subject.Handle, 1, &context->CrlInfo));
            }
        #endregion
        #region M:GetO(IX509RelativeDistinguishedNameSequence):String
        private static String GetO(IX509RelativeDistinguishedNameSequence source) {
            source.TryGetValue("2.5.4.10", out var r);
            return r;
            }
        #endregion
        }
    }