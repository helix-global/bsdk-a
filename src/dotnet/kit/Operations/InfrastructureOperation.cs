using System;
using System.Collections.Generic;
using System.IO;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Options;

namespace Operations
    {
    internal class InfrastructureOperation : Operation
        {
        public InfrastructureOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            }

        public override void Execute(TextWriter output)
            {
            output.WriteLine("AvailableProviders:");
            foreach (var type in SCryptographicContext.RegisteredProviders) {
                output.WriteLine($"  {type.ProviderName}:{type.ProviderType}");
                try
                    {
                    using (var context = new SCryptographicContext(null, type.ProviderName, type.ProviderType, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT, null)) {
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