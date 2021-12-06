using System;

namespace Options
    {
    internal class MessageGroupOption : OperationOption
        {
        public Boolean Value { get; }
        public MessageGroupOption(Boolean value) {
            Value = value;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return $"message:{Value.ToString().ToLower()}";
            }
        }
    }