using System;

namespace Options
    {
    internal class DateTimeOption : Option<DateTime>
        {
        public DateTimeOption(DateTime value)
            : base(value)
            {
            }
        }
    }