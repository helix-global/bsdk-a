using System;
using System.Collections.Generic;
using System.IO;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Options;

namespace Operations
    {
    internal class InfrastructureOperation : Operation
        {
        public InfrastructureOperation(TextWriter output, IList<OperationOption> args)
            : base(output, args)
            {
            }

        public override void Execute(TextWriter output)
            {
            output.WriteLine("AvailableProviders:");
            foreach (var type in CryptographicContext.AvailableProviders) {
                output.WriteLine($"  {type.Key}:{type.Value}");
                try
                    {
                    using (var context = new CryptographicContext(null, type.Key, type.Value, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT, null)) {
                        foreach (var algid in context.SupportedAlgorithms) {
                            output.WriteLine($"    {algid.Key}:{algid.Value}");
                            }
                        }
                    }
                catch (Exception e)
                    {
                    Console.WriteLine(e);
                    }
                }
            }
        }
    }