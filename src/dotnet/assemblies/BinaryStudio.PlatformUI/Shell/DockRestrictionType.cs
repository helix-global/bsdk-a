using System;

namespace BinaryStudio.PlatformUI
    {
    [Flags]
    public enum DockRestrictionType
        {
        None = 0,
        DocumentGroup = 1,
        Document = 2,
        OutsideView = 4,
        AlwaysFloating = -1
        }
    }