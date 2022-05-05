using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Options;

namespace Operations
    {
    internal class UsageOperation : Operation
        {
        public UsageOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            }

        public override void Execute(TextWriter output) {
            var i = 0;
            var version = Assembly.GetEntryAssembly().GetName().Version;
            output.WriteLine("# Version: {{{0}}}:{{{1}}}", version,
                (new DateTime(2000, 1, 1).
                    AddDays(version.Build).
                    AddSeconds(version.Revision * 2)).ToString("s"));
            output.WriteLine($"# Is64BitProcess:{Environment.Is64BitProcess}");
            output.WriteLine("# Available options:");
            foreach (var descriptor in descriptors.OrderBy(j => j.OptionName)) { 
                if (i > 0)
                    {
                    output.WriteLine();
                    }
                output.Write("  ");
                descriptor.Usage(output);
                i++;
                }
            output.Write("\n# Samples:");
            output.WriteLine(@"
  input:{file-name}.ldif output:{folder} batch:extract
  input:{file-name}.ldif output:{folder} batch:extract,group
  input:{file-name}.ldif filter:*.cer batch:{un}install storelocation:LocalMachine storename:Root
  input:{file-name}.ldif filter:*.crl batch:{un}install storelocation:LocalMachine storename:CA
  input:*.cer batch:{un}install storelocation:LocalMachine storename:Root
  input:*.crl batch:{un}install storelocation:LocalMachine storename:CA
  input:{file-name} hash algid:{algid}
  input:{file-name} hash algid:{algid} providertype:{number}
  input:{file-name} message verify
  input:*.crl [output:{folder}] batch:rename,group
  input:*.rar [output:{folder}] batch:rename,group filter:*.crl
  input:{file-name}.rar output:{folder} batch:extract,group
  input:{file-name}.rar output:{folder} batch:extract,group filter:*.crl
  input:{file-name}.rar\*.crl output:{folder} batch:extract,group
  input:*.cer verify policy:icao datetime:{datetime}
  input:{file-name} certificate:{thumbprint} message create storename:device
");
            }
        }
    }