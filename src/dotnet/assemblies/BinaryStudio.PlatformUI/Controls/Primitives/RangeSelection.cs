using System;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    internal struct RangeSelection
        {
        public Int64 Start { get; }
        public Int64 Length { get; }
        public RangeSelection(Int64 start, Int64 length)
            {
            Start = start;
            Length = length;
            }

        public override String ToString()
            {
            return $"{Start};{Length}";
            }
        }
    }