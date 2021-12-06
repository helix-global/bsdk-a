using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Options
    {
    internal class InputFileOrFolderOption : OperationOption
        {
        public IList<String> Values { get; }
        public InputFileOrFolderOption(IList<String> values) {
            if (values == null) { throw new ArgumentNullException(nameof(values)); }
            if (values.Count == 0) { throw new ArgumentOutOfRangeException(nameof(values)); }
            Values = new ReadOnlyCollection<String>(values);
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return String.Format("input:{0}",String.Join(";", Values));
            }
        }
    }