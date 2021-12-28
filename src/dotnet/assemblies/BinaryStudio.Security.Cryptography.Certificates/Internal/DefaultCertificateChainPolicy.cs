using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Serialization;
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

        private static void RaiseExceptionForStatus(CertificateChainErrorStatus status, UInt32 mask)
            {
            var i = ((Int32)status) & mask;
            if (i == 0) { return; }
            var exceptions = new List<Exception>();
            if ((i & (Int32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_VALID)                 != 0) { exceptions.Add(GetExceptionForStatus(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_VALID));                 i &= ~(Int32)CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_VALID;                 }
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
            if (chaincontext.TrustStatus.ErrorStatus != 0) {
                var target = new HashSet<Exception>();
                if ((chaincontext.ChainCount > 0) && (chaincontext.ChainArray != null)) {
                    for (var i = 0; i < chaincontext.ChainCount; i++) {
                        var chain = chaincontext.ChainArray[i];
                        if (chain != null) {
                            try
                                {
                                var exceptions = new List<Exception>();
                                if ((chain->ElementCount > 0) && (chain->ElementArray != null)) {
                                    for (var j = 0; j < chain->ElementCount; j++) {
                                        var chainE = chain->ElementArray[j];
                                        if (chainE != null) {
                                            try
                                                {
                                                RaiseExceptionForStatus(chainE->TrustStatus.ErrorStatus, 0xffffffff);
                                                }
                                            catch (Exception e)
                                                {
                                                e.Data["ChainIndex"       ] = i;
                                                e.Data["ChainElementIndex"] = j;
                                                e.Data["ChainStatusError" ] = chainE->TrustStatus.ErrorStatus;
                                                e.Data["ChainStatusInfo"  ] = chainE->TrustStatus.InfoStatus;
                                                if (chainE->CertContext != null) { e.Data["ChainCertContext"] = ToString(ref *(chainE->CertContext)); }
                                                if (chainE->RevocationInfo != null) {
                                                    e.Data["ChainRevocationResult"] = ToString((HRESULT)chainE->RevocationInfo->RevocationResult);
                                                    if (chainE->RevocationInfo->CrlInfo != null) {
                                                        e.Data["ChainCrlInfo"] = ToString(ref *(chainE->RevocationInfo->CrlInfo->BaseCrlContext));
                                                        }
                                                    }
                                                exceptions.Add(e);
                                                }
                                            }
                                        }
                                    }

                                try
                                    {
                                    RaiseExceptionForStatus(chain->TrustStatus.ErrorStatus, 0x000f0000);
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
                                e.Data["ChainIndex"       ] = i;
                                e.Data["ChainStatusError" ] = chain->TrustStatus.ErrorStatus;
                                e.Data["ChainStatusInfo"  ] = chain->TrustStatus.InfoStatus;
                                if ((chain->ElementCount > 0) && (chain->ElementArray != null)) {
                                    var chaindetails = new List<String>();
                                    var c = chain->ElementCount;
                                    for (var j = 0; j < c; j++) {
                                        var chainE = chain->ElementArray[j];
                                        if (chainE != null) {
                                            var o = BuildCertificate(ref *(chainE->CertContext));
                                            chaindetails.Add($"Order:{{ {j}}}:SerialNumber:{{{o.SerialNumber.ToString().ToLowerInvariant()}}},Subject:{{{o.Subject}}},Issuer:{{{o.Issuer}}}");
                                            if ((j == c - 1) && (chain->TrustStatus.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN))) {
                                                var a = o.Extensions.OfType<CertificateAuthorityKeyIdentifier>().FirstOrDefault();
                                                var r = new StringBuilder();
                                                r.Append($"Order:{{*{j + 1}}}:");
                                                if (a != null)
                                                    {
                                                    r.Append((a.SerialNumber != null)
                                                        ? $"SerialNumber:{{{a.SerialNumber.ToLowerInvariant()}}},"
                                                        : $"SubjectKeyIdentifier:{{{a.KeyIdentifier.ToString("x")}}},");
                                                    }
                                                chaindetails.Add($"{r}Subject:{{{o.Issuer}}}");
                                                }
                                            }
                                        }
                                    e.Data["ChainInfo"] = $"\n{String.Join("\n", chaindetails)}";
                                    }
                                target.Add(e);
                                }
                            }
                        }
                    }
                if (target.Any())
                    {
                    throw new SecurityException("Certificate chain verification error.", target);
                    }
                }
            }

        [DllImport("crypt32.dll", ExactSpelling = true, SetLastError = true)] private static extern Boolean CertVerifyCertificateChainPolicy([In] IntPtr policy, [In] ref CERT_CHAIN_CONTEXT chaincontext, [In] ref CERT_CHAIN_POLICY_PARA policypara, [In][Out] ref CERT_CHAIN_POLICY_STATUS policystatus);

        private static readonly IDictionary<CertificateChainErrorStatus,Type> types = new Dictionary<CertificateChainErrorStatus,Type>();
        static DefaultCertificateChainPolicy()
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
        }
    }