﻿using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class PolicyOptionDescriptor : OptionDescriptor
        {
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("policy:")) {
                    option = new PolicyOption(source.Substring(7).Trim());
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("policy:{value}");
            }
        }
    }