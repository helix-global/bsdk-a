using System;

namespace Options
    {
    internal class StartOffset : Option<Int64>
        {
        public StartOffset(String value)
            :base(Int64.Parse(value))
            {
            }
        }
    }