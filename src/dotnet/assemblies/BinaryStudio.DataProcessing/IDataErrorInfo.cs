using System;

namespace BinaryStudio.DataProcessing
    {
    public interface IDataErrorInfo
        {
        DataErrorInfoState State { get; }
        String Message { get; }
        }
    }