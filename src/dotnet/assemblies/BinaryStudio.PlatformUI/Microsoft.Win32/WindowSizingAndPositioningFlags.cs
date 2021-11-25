using System;

namespace Microsoft.Win32
    {
    [Flags]
    internal enum WindowSizingAndPositioningFlags : uint {
        RetainsTheCurrentSize    = 0x01,
        RetainsTheCurrentZOrder  = 0x04,
        DoesNotActivateTheWindow = 0x10,
        DisplaysTheWindow        = 0x40
        }
    }