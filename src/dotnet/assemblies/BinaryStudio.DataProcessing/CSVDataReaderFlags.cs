using System;

namespace BinaryStudio.DataProcessing
    {
    [Flags]
    public enum CSVDataReaderFlags
        {
        Delimited                  = 0x01,
        FixedWidth                 = 0x02,
        FirstRowContainsFieldNames = 0x04,
        HasMultiLineValue          = 0x08
        }
    }