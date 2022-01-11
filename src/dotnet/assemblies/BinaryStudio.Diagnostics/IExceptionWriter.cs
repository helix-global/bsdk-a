using System;

namespace BinaryStudio.Diagnostics
    {
    public interface IExceptionWriter
        {
        void WriteLine(String value);
        void WriteLine(String format, params Object[] args);
        void Write(String value);
        }
    }