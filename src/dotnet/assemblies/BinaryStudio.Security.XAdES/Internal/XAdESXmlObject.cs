using System;

namespace BinaryStudio.Security.XAdES.Internal
    {
    internal class XAdESXmlObject<T>
        {
        public T Source {get; }
        public XAdESXmlObject(T source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Source = source;
            }
        }
    }