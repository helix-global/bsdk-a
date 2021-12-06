using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Options
    {
    internal class VerifyOption : OperationOption
        {
        public IList<String> Values { get; }
        public VerifyOption(IList<String> values) {
            if (values == null) { throw new ArgumentNullException(nameof(values)); }
            if (values.Count == 0) { throw new ArgumentOutOfRangeException(nameof(values)); }
            Values = new ReadOnlyCollection<String>(values);
            }

        public VerifyOption()
            {
            Values = new ReadOnlyCollection<String>(new String[0]);
            }

        public Boolean HasValue(String value)
            {
            return HasValue(Values, value);
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return String.Format("verify{0}",
                (Values.Count == 0)
                    ? String.Empty
                    : $":{String.Join(";", Values)}"
                );
            }
        }
    }