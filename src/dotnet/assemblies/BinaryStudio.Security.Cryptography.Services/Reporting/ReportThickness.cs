using System;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public struct ReportThickness
        {
        public BorderStyleValues Left { get;set; }
        public BorderStyleValues Top { get;set; }
        public BorderStyleValues Right { get;set; }
        public BorderStyleValues Bottom { get;set; }

        public ReportThickness(BorderStyleValues value)
            {
            Left   = value;
            Top    = value;
            Right  = value;
            Bottom = value;
            }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <returns>true if <paramref name="other" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        /// <param name="other">Another object to compare to. </param>
        public override Boolean Equals(Object other)
            {
            return (other is ReportThickness r)
                ? ((r.Left == Left) && (r.Right == Right) && (r.Bottom == Bottom) && (r.Top == Top))
                : false;
            }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override Int32 GetHashCode()
            {
            return HashCodeCombiner.GetHashCode(Left, Top, Bottom, Right);
            }
        }
    }