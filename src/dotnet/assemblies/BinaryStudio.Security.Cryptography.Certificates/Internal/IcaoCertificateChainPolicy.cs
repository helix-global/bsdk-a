using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates.Internal
    {
    internal class IcaoCertificateChainPolicy : X509CertificateChainPolicy
        {
        public override unsafe Boolean Verify(ISet<Exception> target, ICryptographicContext context,
            OidCollection applicationpolicy, OidCollection certificatepolicy,
            TimeSpan timeout, DateTime datetime, IX509CertificateStorage store,
            CERT_CHAIN_FLAGS flags, ref CERT_CHAIN_CONTEXT chaincontext) {
            if (chaincontext.TrustStatus.ErrorStatus != CertificateChainErrorStatus.CERT_TRUST_NO_ERROR) {
                if (chaincontext.ChainCount > 0) {
                    for (var i = 0; i < chaincontext.ChainCount; ++i) {
                        var chain = chaincontext.ChainArray[i];
                        if ((chain != null) && (chain->ElementCount > 0)) {
                            var n = chain->ElementCount;
                            var S = chain->ElementArray[0];
                            if (n == 1)
                                {
                                var I = chain->ElementArray[0];
                                var subject = new X509Certificate((IntPtr)S->CertContext);
                                var issuer  = new X509Certificate((IntPtr)I->CertContext);
                                if (context != null) {
                                    if (!context.VerifyCertificateSignature(out var e,
                                        subject, issuer, CRYPT_VERIFY_CERT_SIGN.NONE))
                                        {
                                        target.Add(e);
                                        }
                                    }
                                }
                            else
                                {
                                var I = chain->ElementArray[1];
                                var subject = new X509Certificate((IntPtr)S->CertContext);
                                var issuer  = new X509Certificate((IntPtr)I->CertContext);
                                if (!issuer.Verify(out var e, context,
                                    store,applicationpolicy,certificatepolicy,
                                    timeout,datetime, flags, this))
                                    {
                                    if (e is AggregateException a)
                                        {
                                        target.UnionWith(a.InnerExceptions);
                                        }
                                    else
                                        {
                                        target.Add(e);
                                        }
                                    }
                                if (context != null)
                                    {
                                    if (!context.VerifyCertificateSignature(out e,
                                        subject, issuer,
                                        CRYPT_VERIFY_CERT_SIGN.NONE))
                                        {
                                        target.Add(e);
                                        }
                                    }
                                }
                            }
                        }
                    }
                
                }
            return true;
            }
        }
    }