using System;

namespace BinaryStudio.Security.Cryptography.Interchange
    {
    public partial class DObject : IDObject
        {
        DObjectType IDObject.Type
            {
            get { return (DObjectType)Type; }
            set { Type = (Byte)value; }
            }
        }
    }