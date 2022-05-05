using System;
using System.Collections.Generic;

namespace Options
    {
    internal class SetOption : OperationOptionWithParameters
        {
        public override String OptionName { get { return "set"; }}
        public IDictionary<String,String> Properties { get; }
        public SetOption(IList<String> values)
            : base(values)
            {
            Properties = new Dictionary<String, String>();
            foreach (var value in Values) {
                var index = value.IndexOf(':');
                if (index == -1)
                    {
                    Properties[value.ToLowerInvariant()] = null;
                    }
                else
                    {
                    Properties[value.Substring(0,index).ToLowerInvariant()] = value.Substring(index + 1);
                    }
                }
            }
        }
    }