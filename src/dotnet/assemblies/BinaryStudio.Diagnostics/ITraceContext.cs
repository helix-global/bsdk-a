using System;

namespace BinaryStudio.Diagnostics
    {
    public interface ITraceContext : IDisposable
        {
        Int64 DataSize { get;set; }
        }
    }