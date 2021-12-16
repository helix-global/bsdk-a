using System;

namespace Options
    {
    internal abstract class BooleanOption : Option<Boolean>
        {
        protected BooleanOption(Boolean value)
            :base(value)
            {
            }
        }
    }