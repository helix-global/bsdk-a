using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Options;

namespace Operations
    {
    internal class VerifyOperation : Operation
        {
        public CRYPT_PROVIDER_TYPE ProviderType { get; }
        public String Policy { get; }
        public String InputFileName { get; }

        public VerifyOperation(TextWriter output, IList<OperationOption> args)
            : base(output, args)
            {
            ProviderType  = (CRYPT_PROVIDER_TYPE)args.OfType<ProviderTypeOption>().First().Type;
            Policy        = args.OfType<PolicyOption>().FirstOrDefault()?.Value;
            InputFileName = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values.FirstOrDefault();
            }

        public override void Execute(TextWriter output) {
            if (Path.GetFileNameWithoutExtension(InputFileName).Contains("*")) {
                var folder = Path.GetDirectoryName(InputFileName);
                var pattern = Path.GetFileName(InputFileName);
                if (String.IsNullOrEmpty(folder)) { folder = ".\\"; }
                foreach (var filename in Directory.GetFiles(folder, pattern, SearchOption.AllDirectories)) {
                    ProcessFile(filename);
                    }
                }
            else
                {
                ProcessFile(InputFileName);
                }
            }

        private void ProcessFile(String filename) {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            var E = Path.GetExtension(filename).ToLower();
            switch (E) {
                case ".cer":
                    {
                    
                    }
                    break;
                }
            }
        }
    }