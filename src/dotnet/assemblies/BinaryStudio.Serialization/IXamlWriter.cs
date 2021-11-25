using System;

namespace BinaryStudio.Serialization
    {
    public interface IXamlWriter
        {
        IDisposable WriteElementScope(String prefix, String localname, String ns);
        IDisposable WriteElementScope(String localname);
        }
    }