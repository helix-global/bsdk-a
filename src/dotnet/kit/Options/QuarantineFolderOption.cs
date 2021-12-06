using System;

namespace Options
    {
    internal class QuarantineFolderOption : Option<String>
        {
        public QuarantineFolderOption(String value)
            :base(value)
            {
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return String.Format("quarantine:{0}",Value);
            }
        }
    }