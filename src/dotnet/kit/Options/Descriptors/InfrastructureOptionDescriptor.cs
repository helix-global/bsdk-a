using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class InfrastructureOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "infrastructure"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("infrastructure:")) {
                    option = new InfrastructureOption(
                        source.Substring(15).
                        Split(new []{','}, StringSplitOptions.RemoveEmptyEntries));
                    return true;
                    }
                if (source == "infrastructure")
                    {
                    option = new InfrastructureOption();
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("infrastructure[:{value}]");
            }

        }
    }