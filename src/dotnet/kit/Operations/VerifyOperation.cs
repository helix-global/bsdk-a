using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Microsoft.Win32;
using Options;

namespace Operations
    {
    internal class VerifyOperation : Operation
        {
        public CRYPT_PROVIDER_TYPE ProviderType { get; }
        public String Policy { get; }
        public String InputFileName { get; }
        public DateTime DateTime { get; }
        private IX509CertificateChainPolicy CertificateChainPolicy { get; }

        public VerifyOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            CertificateChainPolicy = X509CertificateChainPolicy.POLICY_BASE;
            ProviderType  = (CRYPT_PROVIDER_TYPE)args.OfType<ProviderTypeOption>().First().Type;
            Policy        = args.OfType<PolicyOption>().FirstOrDefault()?.Value;
            InputFileName = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values.FirstOrDefault();
            DateTime      = args.OfType<DateTimeOption>().First().Value;
            if (Policy != null) {
                switch (Policy) {
                    case "base"    : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_BASE;                break;
                    case "auth"    : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_AUTHENTICODE;        break;
                    case "authts"  : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_AUTHENTICODE_TS;     break;
                    case "ssl"     : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_SSL;                 break;
                    case "basic"   : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_BASIC_CONSTRAINTS;   break;
                    case "ntauth"  : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_NT_AUTH;             break;
                    case "msroot"  : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_MICROSOFT_ROOT;      break;
                    case "ev"      : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_EV;                  break;
                    case "ssl_f12" : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_SSL_F12;             break;
                    case "ssl_hpkp": CertificateChainPolicy = X509CertificateChainPolicy.POLICY_SSL_HPKP_HEADER;     break;
                    case "third"   : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_THIRD_PARTY_ROOT;    break;
                    case "ssl_key" : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_SSL_KEY_PIN;         break;
                    case "icao"    : CertificateChainPolicy = X509CertificateChainPolicy.IcaoCertificateChainPolicy; break;
                    default: throw new NotSupportedException();
                    }
                }
            }

        #region M:Execute(TextWriter)
        public override void Execute(TextWriter output) {
            using (var context = new CryptographicContext(Logger, ProviderType, CryptographicContextFlags.CRYPT_VERIFYCONTEXT|CryptographicContextFlags.CRYPT_SILENT)) {
                if (Path.GetFileNameWithoutExtension(InputFileName).Contains("*")) {
                    var folder = Path.GetDirectoryName(InputFileName);
                    var pattern = Path.GetFileName(InputFileName);
                    if (String.IsNullOrEmpty(folder)) { folder = ".\\"; }
                    foreach (var filename in Directory.GetFiles(folder, pattern, SearchOption.AllDirectories)) {
                        Execute(context, filename);
                        }
                    }
                else
                    {
                    Execute(context, InputFileName);
                    }
                }
            }
        #endregion
        #region M:Execute(CryptographicContext,String)
        private void Execute(CryptographicContext context, String filename) {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            var E = Path.GetExtension(filename).ToLower();
            var timer = new Stopwatch();
            timer.Start();
            switch (E) {
                case ".cer":
                    {
                    var certificate = new X509Certificate(File.ReadAllBytes(filename));
                    try
                        {
                        certificate.Verify(context, CertificateChainPolicy, DateTime);
                        timer.Stop();
                        Write(ConsoleColor.Green, "{ok}");
                        Write(ConsoleColor.Gray, ":");
                        Write(ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                        WriteLine(ConsoleColor.Gray, $":{filename}");
                        }
                    catch (Exception e)
                        {
                        timer.Stop();
                        Write(ConsoleColor.Red, "{error}");
                        Write(ConsoleColor.Gray, ":");
                        Write(ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                        WriteLine(ConsoleColor.Gray, $":{filename}");
                        Logger.Log(LogLevel.Warning, e);
                        }
                    }
                    break;
                }
            }
        #endregion
        }
    }