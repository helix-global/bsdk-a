using System;
using System.Diagnostics;
using System.Reflection;

namespace BinaryStudio.Diagnostics
    {
    public class TraceContextStackFrameIdentity : TraceContextIdentity, IEquatable<TraceContextStackFrameIdentity>
        {
        internal StackFrame Source { get; }
        private Int32 FileLineNumber { get; }
        private Int32 FileColumnNumber { get; }
        private MethodBase Method { get; }

        public override String ShortName { get { return Method.Name; }}

        public TraceContextStackFrameIdentity(StackFrame source)
            {
            if (source  == null) { throw new ArgumentNullException(nameof(source)); }
            Source = source;
            FileColumnNumber = source.GetFileColumnNumber();
            FileLineNumber = source.GetFileLineNumber();
            Method = source.GetMethod();
            }

        /**
         * <summary>Serves as the default hash function.</summary>
         * <returns>A hash code for the current object.</returns>
         * */
        public override Int32 GetHashCode()
            {
            return Combine(Method.GetHashCode(), FileColumnNumber, FileLineNumber);
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
         * */
        public Boolean Equals(TraceContextStackFrameIdentity other)
            {
            if (other == null) { return false; }
            return ReferenceEquals(this, other) || (
                Method.Equals(other.Method) &&
                (FileColumnNumber == other.FileColumnNumber) &&
                (FileLineNumber == other.FileLineNumber));
            }

        /**
         * <summary>Determines whether the specified object is equal to the current object.</summary>
         * <param name="other">The object to compare with the current object.</param>
         * <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
         * */
        public override Boolean Equals(Object other)
            {
            return (other is TraceContextStackFrameIdentity r) && Equals(r);
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * */
        public override String ToString()
            {
            return ((FileLineNumber == 0) && (FileColumnNumber == 0))
                ? $"{ToString(Method)}"
                : $"{ToString(Method)}[{FileLineNumber},{FileColumnNumber}]";
            }

        #region M:Combine(Int32,Int32,Int32):Int32
        private static Int32 Combine(Int32 h1, Int32 h2, Int32 h3)
            {
            return Combine(h1, Combine(h2, h3));
            }
        #endregion
        #region M:Combine(Int32,Int32):Int32
        private static Int32 Combine(Int32 h1, Int32 h2)
            {
            // The jit optimizes this to use the ROL instruction on x86
            // Related GitHub pull request: dotnet/coreclr#1830
            var u = ((UInt32)h1 << 5) | ((UInt32)h1 >> 27);
            return ((Int32)u + h1) ^ h2;
            }
        #endregion
        }
    }