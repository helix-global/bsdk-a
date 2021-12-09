﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Options;

namespace Operations
    {
    internal class UsageOperation : Operation
        {
        public UsageOperation(TextWriter output, IList<OperationOption> args)
            : base(output, args)
            {
            }

        public override void Execute(TextWriter output) {
            var i = 0;
            var version = Assembly.GetEntryAssembly().GetName().Version;
            output.WriteLine("# Version: {{{0}}}:{{{1}}}", version,
                (new DateTime(2000, 1, 1).
                    AddDays(version.Build).
                    AddSeconds(version.Revision * 2)).ToString("s"));
            output.WriteLine("# Available options:");
            foreach (var descriptor in descriptors) { 
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
  input:{file-name}.ldif filter:*.cer batch:install   storelocation:LocalMachine storename:Root
  input:{file-name}.ldif filter:*.cer batch:uninstall storelocation:LocalMachine storename:Root
  input:{file-name}.ldif filter:*.crl batch:install storelocation:LocalMachine storename:CA");
            }
        }
    }