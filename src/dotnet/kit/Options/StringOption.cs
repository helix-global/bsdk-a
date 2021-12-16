using System;

namespace Options
    {
    internal abstract class StringOption : Option<String>
        {
        protected StringOption(String value)
            :base(value)
            {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            if (String.IsNullOrWhiteSpace(value)) { throw new ArgumentOutOfRangeException(nameof(value)); }
            }
        }
    }