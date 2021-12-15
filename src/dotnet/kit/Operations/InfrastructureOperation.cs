using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Microsoft.Win32;
using Options;

namespace Operations
    {
    internal class InfrastructureOperation : Operation
        {
        public InfrastructureFlags Flags { get; }
        public Int32? ProviderType { get; }

        public InfrastructureOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            var flags = args.OfType<InfrastructureOption>().FirstOrDefault();
            ProviderType  = args.OfType<ProviderTypeOption>().FirstOrDefault()?.Type;
            if (flags != null) {
                if (flags.HasValue("csp"))   { Flags |= InfrastructureFlags.CSP;      }
                if (flags.HasValue("types")) { Flags |= InfrastructureFlags.CSPtypes; }
                if (flags.HasValue("keys"))  { Flags |= InfrastructureFlags.CSPkeys;  }
                if (flags.HasValue("algs"))  { Flags |= InfrastructureFlags.CSPalgs;  }
                }
            if (Flags == 0)
                {
                Flags = InfrastructureFlags.CSP;
                }
            }

        public override void Execute(TextWriter output) {
            if (Flags.HasFlag(InfrastructureFlags.CSP)) {
                WriteLine(ConsoleColor.White, "AvailableProviders:");
                foreach (var type in SCryptographicContext.RegisteredProviders) {
                    Write(ConsoleColor.Gray,"  {");
                    Write(ConsoleColor.Yellow, $"{(Int32)type.ProviderType,2}");
                    Write(ConsoleColor.Gray,"} ");
                    Write(ConsoleColor.Gray,$"{type.ProviderName}:");
                    WriteLine(ConsoleColor.Yellow, $"{type.ProviderType}");
                    if (Flags.HasFlag(InfrastructureFlags.CSPalgs)) {
                        try
                            {
                            using (var context = new SCryptographicContext(null, type.ProviderName, type.ProviderType, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT, null)) {
                                foreach (var algid in context.SupportedAlgorithms) {
                                    WriteLine(ConsoleColor.Gray, $"    {algid.Key}:{algid.Value}");
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
            if (Flags.HasFlag(InfrastructureFlags.CSPtypes)) {
                WriteLine(ConsoleColor.White, "AvailableProviderTypes:");
                foreach (var type in SCryptographicContext.AvailableTypes) {
                    Write(ConsoleColor.Gray,"  {");
                    Write(ConsoleColor.Yellow, $"{(Int32)type.Key,2}");
                    Write(ConsoleColor.Gray,"} ");
                    Write(ConsoleColor.Yellow, $"{type.Key}");
                    WriteLine(ConsoleColor.Gray, $":{type.Value}");
                    }
                }
            if (Flags.HasFlag(InfrastructureFlags.CSPkeys)) {
                if (ProviderType == null) { throw new InvalidOperationException("required providertype:{number} option."); }
                var Is64Bit = Environment.Is64BitProcess;
                WriteLine(ConsoleColor.White, "Keys {{{0}}}:", (CRYPT_PROVIDER_TYPE)ProviderType.Value);
                WriteLine(ConsoleColor.White, "  User Keys:");
                var j = 0;
                using (var context = new CryptographicContext(Logger, (CRYPT_PROVIDER_TYPE)ProviderType.Value, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    foreach (var i in context.Keys) {
                        var handle = Is64Bit
                            ? ((Int64)i.Handle).ToString("X16")
                            : ((Int32)i.Handle).ToString("X8");
                        WriteLine(ConsoleColor.Gray, "    {{{0}}}:{{{1}}}:{2}", j, handle, i.Container);
                        j++;
                        }
                    }
                WriteLine(ConsoleColor.White, "  Machine Keys:");
                j = 0;
                using (var context = new CryptographicContext(Logger, (CRYPT_PROVIDER_TYPE)ProviderType.Value, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT|CryptographicContextFlags.CRYPT_MACHINE_KEYSET)) {
                    foreach (var i in context.Keys) {
                        var handle = Is64Bit
                            ? ((Int64)i.Handle).ToString("X16")
                            : ((Int32)i.Handle).ToString("X8");
                        WriteLine(ConsoleColor.Gray, "    {{{0}}}:{{{1}}}:{2}", j, handle, i.Container);
                        j++;
                        }
                    }
                }
            }
        }
    }