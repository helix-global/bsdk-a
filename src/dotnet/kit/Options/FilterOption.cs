using System;

namespace Options
    {
    internal class FilterOption : Option<String>
        {
        public FilterOption(String value)
            : base(value)
            {
            }
        }
    }