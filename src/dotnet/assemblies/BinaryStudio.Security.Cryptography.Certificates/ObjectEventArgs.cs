using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public class ObjectEventArgs : EventArgs
        {
        public Object Target { get; }
        public ObjectEventArgs(Object target)
            {
            Target = target;
            }
        }
    }