using System;
using System.Globalization;

namespace BinaryStudio.PlatformUI
    {
    public struct SplitterLength
        {
        public SplitterUnitType SplitterUnitType {get;}
        public Double Value {get;}
        public Boolean IsStretch {get{ return SplitterUnitType == SplitterUnitType.Stretch; }}
        public Boolean IsFill {get{ return SplitterUnitType == SplitterUnitType.Fill; }}

        public SplitterLength(Double value, SplitterUnitType type) {
            SplitterUnitType = type;
            Value = value;
            }

        public SplitterLength(Double value)
            : this(value, SplitterUnitType.Stretch)
            {
            }

        public override String ToString() {
            return SplitterLengthConverter.ToString(this, CultureInfo.InvariantCulture);
            }
        }
    }