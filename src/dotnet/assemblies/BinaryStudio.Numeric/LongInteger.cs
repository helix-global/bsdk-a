using System;
using System.Text;

namespace BinaryStudio.Numeric
    {
    public class LongInteger
        {
        private readonly Byte[] magnitude;

        public LongInteger(Byte[] value) {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            magnitude = value;
            Size = magnitude.Length;
            }

        public LongInteger(Byte[] value, Int32 size) {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            magnitude = value;
            Size = size;
            }

        public Int32 Size { get; }
        public Int32 Length { get {
            return (magnitude != null)
                ? magnitude.Length
                : 0;
            }}

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() {
            if (magnitude == null) { return "{none}"; }
            var r = new StringBuilder();
            if (Length < Size) {
                for (var i = 0; i < Size - Length; i++) { r.Append("00"); }
                }
            for (var i = 0; i < Length; i++) {
                r.AppendFormat("{X2}", magnitude[i]);
                }
            return base.ToString();
            }
        }
    }