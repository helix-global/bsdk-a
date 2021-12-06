using System;

namespace Options
    {
    internal class OutputTypeOption : OperationOption
        {
        public Boolean IsJson { get; }
        public OutputTypeOption(String source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            switch (source.ToLower()) {
                case "none": { break; }
                case "json":
                    {
                    IsJson = true;
                    }
                    break;
                default: { throw new ArgumentOutOfRangeException(nameof(source)); }
                }
            }
        }
    }