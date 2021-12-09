using System;

namespace BinaryStudio.Numeric
    {
    internal static class NumericHelper
        {
        public static unsafe void Copy(UInt32[] source, UInt32* target) {
            var c = source.Length;
            for (var i = 0; i < c; i++) {
                target[i] = source[i];
                }
            }
        }
    }