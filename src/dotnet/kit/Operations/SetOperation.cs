using System;
using System.Collections.Generic;
using System.IO;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Options;

namespace Operations
    {
    internal class SetOperation : CreateOrEncryptMessageOperation
        {
        public SetOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            }

        protected override void Execute(TextWriter output, CryptographicContext context, IX509CertificateStorage store)
            {
            throw new NotImplementedException();
            }
        }
    }