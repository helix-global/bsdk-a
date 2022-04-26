using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Options
    {
    internal abstract class OperationOptionWithParameters : OperationOption
        {
        public IList<String> Values { get; }
        public abstract String OptionName { get; }
        protected OperationOptionWithParameters(IList<String> values) {
            if (values == null) { throw new ArgumentNullException(nameof(values)); }
            Values = new ReadOnlyCollection<String>(values);
            }

        protected OperationOptionWithParameters()
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
            return String.Format("{0}{1}",
                OptionName,
                (Values.Count == 0)
                    ? String.Empty
                    : $":{String.Join(",", Values)}"
                );
            }
        }
    }