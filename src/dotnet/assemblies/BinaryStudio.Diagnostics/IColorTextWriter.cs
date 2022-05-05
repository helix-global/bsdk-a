using System;

namespace BinaryStudio.Diagnostics
    {
    public interface IColorTextWriter
        {
        void WriteLine(String value);
        void WriteLine(String format, params Object[] args);
        }
    }