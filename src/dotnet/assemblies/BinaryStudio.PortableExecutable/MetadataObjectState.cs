using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    public enum MetadataObjectState
        {
        Pending,
        Loading,
        Loaded,
        Failed,
        Disposed
        }
    }
