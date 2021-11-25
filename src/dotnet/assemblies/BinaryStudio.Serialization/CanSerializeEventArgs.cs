using System;

namespace BinaryStudio.Serialization
    {
    public class CanSerializeEventArgs : EventArgs
        {
        public Boolean CanSerialize { get;set; }
        }

    public delegate void CanSerializeEventHandler(Object sender, CanSerializeEventArgs e);
    }