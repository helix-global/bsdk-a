using System;
using System.Collections.Generic;

namespace Options
    {
    internal abstract class OperationOption
        {
        protected static Boolean HasValue(IList<String> values, String value) {
            if (values != null) {
                foreach (var i in values) {
                    if (String.Equals(i, value, StringComparison.OrdinalIgnoreCase)) {
                        return true;
                        }
                    }
                }
            return false;
            }
        }
    }