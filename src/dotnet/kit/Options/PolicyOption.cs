using System;

namespace Options
    {
    internal class PolicyOption : Option<String>
        {
        public PolicyOption(String value)
            : base(value)
            {
            }
        }
    }