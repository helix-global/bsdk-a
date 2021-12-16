using Options.Descriptors;
using System;

namespace Options
    {
    internal class OptionRequiredException : Exception
        {
        public OptionDescriptor Descriptor { get; }
        public OptionRequiredException(OptionDescriptor descriptor) {
            Descriptor = descriptor;
            }
        }
    }