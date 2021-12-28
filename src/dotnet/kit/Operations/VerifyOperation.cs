using System;
using System.Collections.Generic;
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

        public VerifyOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            ProviderType  = (CRYPT_PROVIDER_TYPE)args.OfType<ProviderTypeOption>().First().Type;
            Policy        = args.OfType<PolicyOption>().FirstOrDefault()?.Value;
            InputFileName = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values.FirstOrDefault();
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
            switch (E) {
                case ".cer":
                    {
                    var certificate = new X509Certificate(File.ReadAllBytes(filename));
                    try
                        {
                        certificate.Verify(context, X509CertificateChainPolicy.POLICY_BASE);
                        }
                    catch (Exception e)
                        {
                        Logger.Log(LogLevel.Warning, e);
                        }
                    }
                    break;
                }
            }
        #endregion
        }
    }