using System;

namespace Options
    {
    internal class PinCodeRequestType : Option<PinCodeRequestTypeKind>
        {
        public PinCodeRequestType(PinCodeRequestTypeKind value)
            : base(value)
            {
            }

        public PinCodeRequestType(String value)
            :this(Convert(value))
            {
            }

        private static PinCodeRequestTypeKind Convert(String value)
            {
            switch(value)
                {
                case "DEFAULT": { return PinCodeRequestTypeKind.Default; }
                case "CONSOLE": { return PinCodeRequestTypeKind.Console; }
                case "WINDOW":  { return PinCodeRequestTypeKind.Window;  }
                default: { throw new ArgumentOutOfRangeException(nameof(value)); }
                }
            }
        }
    }