using System;

namespace BinaryStudio.PlatformUI
    {
    /// <summary>
    /// Provides data about which objects were affected when resolving a name change.
    /// </summary>
    internal sealed class NameResolvedEventArgs : EventArgs
        {
        public Object OldObject { get; }
        public Object NewObject { get; }
        public NameResolvedEventArgs(Object o, Object n) {
            OldObject = o;
            NewObject = n;
            }
        }
    }