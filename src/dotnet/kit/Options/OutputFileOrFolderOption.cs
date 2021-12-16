using System;
using System.Collections.Generic;

namespace Options
    {
    internal class OutputFileOrFolderOption : InputFileOrFolderOption
        {
        public OutputFileOrFolderOption(IList<String> values)
            : base(values)
            {
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return String.Format("output:{0}",String.Join(";", Values));
            }
        }
    }