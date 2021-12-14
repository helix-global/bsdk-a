using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Microsoft.Win32;
using Options;

namespace Operations
    {
    internal class HashOperation : Operation
        {
        public AlgId AlgId { get; }
        public IList<String> InputFileName { get; }
        public CRYPT_PROVIDER_TYPE ProviderType { get; }

        public HashOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            InputFileName = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values;
            AlgId         = args.OfType<AlgId>().FirstOrDefault();
            ProviderType  = (CRYPT_PROVIDER_TYPE)args.OfType<ProviderTypeOption>().First().Type;
            if (InputFileName == null) { throw new ArgumentOutOfRangeException(nameof(args)); }
            if (AlgId == null) { throw new ArgumentOutOfRangeException(nameof(args), "required algid:{OID} option."); }
            }

        public override void Execute(TextWriter output) {
            output = output ?? Console.Out;
            using (var context = new CryptographicContext(
                ProviderType,
                CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                foreach (var filename in InputFileName) {
                    if (Path.GetFileNameWithoutExtension(filename).Contains("*")) {
                        var flags = filename.StartsWith(":")
                            ? SearchOption.TopDirectoryOnly
                            : SearchOption.AllDirectories;
                        var fi = filename.StartsWith(":")
                            ? filename.Substring(1)
                            : filename;
                        var folder = Path.GetDirectoryName(fi);
                        var pattern = Path.GetFileName(fi);
                        if (String.IsNullOrEmpty(folder)) { folder = ".\\"; }
                        foreach (var i in Directory.GetFiles(folder, pattern, flags)) {
                            Execute(output, context, i);
                            }
                        }
                    else
                        {
                        Execute(output, context, filename);
                        }
                    }
                }
            }

        private void Execute(TextWriter output, CryptographicContext context, String filename) {
            using (var engine = context.CreateHashAlgorithm(AlgId.Value)) {
                var hash = engine.Compute(File.OpenRead(filename));
                output.WriteLine($"{String.Join(String.Empty, hash.Select(i => i.ToString("X2")))} {filename}");
                }
            }
        }
    }