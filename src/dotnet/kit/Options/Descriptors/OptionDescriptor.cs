using System;
using System.IO;

namespace Options.Descriptors
    {
    internal abstract class OptionDescriptor
        {
        public abstract String OptionName { get; }
        public abstract Boolean TryParse(String source, out OperationOption option);
        public abstract void Usage(TextWriter output);
        }
    }