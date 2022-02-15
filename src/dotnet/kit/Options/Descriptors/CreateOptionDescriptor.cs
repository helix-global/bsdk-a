using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class CreateOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "create"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("create:")) {
                    option = new CreateOption(
                        source.Substring(7).
                        Split(new []{','}, StringSplitOptions.RemoveEmptyEntries));
                    return true;
                    }
                if (source == "create")
                    {
                    option = new CreateOption();
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("create[:[option]+]");
            }
        }
    }