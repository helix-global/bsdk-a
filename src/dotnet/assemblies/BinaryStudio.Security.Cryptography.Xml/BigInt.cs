using System;
using BinaryStudio.Security.Cryptography.Xml;

namespace BinaryStudio.Security.Cryptography
{

    //
    // This is a pretty "crude" implementation of BigInt arithmetic operations.
    // This class is used in particular to convert certificate serial numbers from
    // hexadecimal representation to decimal format and vice versa.
    //
    // We are not very concerned about the perf characterestics of this implementation
    // for now. We perform all operations up to 128 bytes (which is enough for the current
    // purposes although this constant can be increased). Any overflow bytes are going to be lost.
    //
    // A BigInt is represented as a little endian byte array of size 128 bytes. All
    // arithmetic operations are performed in base 0x100 (256). The algorithms used
    // are simply the common primary school techniques for doing operations in base 10.
    //

    internal sealed class BigInt {
        private Byte[] m_elements;
        private const Int32 m_maxbytes = 128; // 128 bytes is the maximum we can handle.
                                            // This means any overflow bits beyond 128 bytes
                                            // will be lost when doing arithmetic operations.
        private const Int32 m_base     = 0x100;
        private Int32 m_size;

        internal BigInt () {
            m_elements = new Byte[m_maxbytes];
        }

        internal BigInt(Byte b) {
            m_elements = new Byte[m_maxbytes];
            SetDigit(0, b);
        }

        //
        // Gets or sets the size of a BigInt.
        //

        internal Int32 Size {
            get {
                return m_size;
            }
            set {
                if (value > m_maxbytes)
                    m_size = m_maxbytes;
                if (value < 0)
                    m_size = 0;
                m_size = value;
            }
        }

        //
        // Gets the digit at the specified index.
        //

        internal Byte GetDigit (Int32 index) {
            if (index < 0 || index >= m_size)
                return 0;

            return m_elements[index];
        }

        //
        // Sets the digit at the specified index.
        //

        internal void SetDigit (Int32 index, Byte digit) {
            if (index >= 0 && index < m_maxbytes) {
                m_elements[index] = digit;
                if (index >= m_size && digit != 0)
                    m_size = (index + 1);
                if (index == m_size - 1 && digit == 0)
                    m_size--;
            }
        }

        internal void SetDigit (Int32 index, Byte digit, ref Int32 size) {
            if (index >= 0 && index < m_maxbytes) {
                m_elements[index] = digit;
                if (index >= size && digit != 0)
                    size = (index + 1);
                if (index == size - 1 && digit == 0)
                    size = (size - 1);
            }
        }

        //
        // overloaded operators.
        //

        public static Boolean operator < (BigInt value1, BigInt value2) {
            if (value1 == null)
                return true;
            else if (value2 == null)
                return false;

            var Len1 = value1.Size;
            var Len2 = value2.Size;

            if (Len1 != Len2) 
                return (Len1 < Len2);

            while (Len1-- > 0) {
                if (value1.m_elements[Len1] != value2.m_elements[Len1])
                    return (value1.m_elements[Len1] < value2.m_elements[Len1]);
            }

            return false;
        }

        public static Boolean operator > (BigInt value1, BigInt value2) {
            if (value1 == null)
                return false;
            else if (value2 == null)
                return true;

            var Len1 = value1.Size;
            var Len2 = value2.Size;

            if (Len1 != Len2) 
                return (Len1 > Len2);

            while (Len1-- > 0) {
                if (value1.m_elements[Len1] != value2.m_elements[Len1])
                    return (value1.m_elements[Len1] > value2.m_elements[Len1]);
            }

            return false;
        }

        public static Boolean operator == (BigInt value1, BigInt value2) {
            if ((Object) value1 == null)
                return ((Object) value2 == null);
            else if ((Object) value2 == null)
                return ((Object) value1 == null);

            var Len1 = value1.Size;
            var Len2 = value2.Size;

            if (Len1 != Len2) 
                return false;

            for (var index = 0; index < Len1; index++) {
                if (value1.m_elements[index] != value2.m_elements[index]) 
                    return false;
            }

            return true;
        }

        public static Boolean operator != (BigInt value1, BigInt value2)  {
            return !(value1 == value2);
        }

        public override Boolean Equals (Object obj) {
            if (obj is BigInt) {
                return (this == (BigInt) obj);
            }
            return false;
        }
    
        public override Int32 GetHashCode () {
            var hash = 0;
            for (var index = 0; index < m_size; index++) {
                hash += GetDigit(index);
            }
            return hash;
        }

        //
        // Adds a and b and outputs the result in c.
        //

        internal static void Add (BigInt a, Byte b, ref BigInt c) {
            var carry = b;
            var sum = 0;

            var size = a.Size;
            var newSize = 0;
            for (var index = 0; index < size; index++) {
                sum = a.GetDigit(index) + carry;
                c.SetDigit(index, (Byte) (sum & 0xFF), ref newSize);
                carry = (Byte) ((sum >> 8) & 0xFF);
            }

            if (carry != 0)
                c.SetDigit(a.Size, carry, ref newSize);

            c.Size = newSize;
        }

        //
        // Negates a BigInt value. Each byte is complemented, then we add 1 to it.
        //

        internal static void Negate (ref BigInt a) {
            var newSize = 0;
            for (var index = 0; index < m_maxbytes; index++) {
                a.SetDigit(index, (Byte) (~a.GetDigit(index) & 0xFF), ref newSize);
            }
            for (var index = 0; index < m_maxbytes; index++) {
                a.SetDigit(index, (Byte) (a.GetDigit(index) + 1), ref newSize);
                if ((a.GetDigit(index) & 0xFF) != 0) break;
                a.SetDigit(index, (Byte) (a.GetDigit(index) & 0xFF), ref newSize);
            }            
            a.Size = newSize;
        }

        //
        // Subtracts b from a and outputs the result in c.
        //

        internal static void Subtract (BigInt a, BigInt b, ref BigInt c) {
            Byte borrow = 0;
            var diff = 0;

            if (a < b) {
                Subtract(b, a, ref c);
                Negate(ref c);
                return;
            }

            var index = 0;
            var size = a.Size;
            var newSize = 0;
            for (index = 0; index < size; index++) {
                diff = a.GetDigit(index) - b.GetDigit(index) - borrow;
                borrow = 0;
                if (diff < 0) {
                    diff += m_base;
                    borrow = 1;
                }
                c.SetDigit(index, (Byte) (diff & 0xFF), ref newSize);
            }

            c.Size = newSize;
        }

        //
        // multiplies a BigInt by an integer.
        //

        private void Multiply (Int32 b) {
            if (b == 0) {
                Clear();
                return;
            }

            Int32 carry = 0, product = 0;
            var size = Size;
            var newSize = 0;
            for (var index = 0; index < size; index++) {
                product = b * GetDigit(index) + carry;
                carry = product / m_base;
                SetDigit(index, (Byte) (product % m_base), ref newSize);
            }

            if (carry != 0) {
                var bytes = BitConverter.GetBytes(carry);
                for (var index = 0; index < bytes.Length; index++) {
                    SetDigit(size + index, bytes[index], ref newSize);
                }
            }

            Size = newSize;
        }

        private static void Multiply (BigInt a, Int32 b, ref BigInt c) {
            if (b == 0) {
                c.Clear();
                return;
            }
                
            Int32 carry = 0, product = 0;
            var size = a.Size;
            var newSize = 0;
            for (var index = 0; index < size; index++) {
                product = b * a.GetDigit(index) + carry;
                carry = product / m_base;
                c.SetDigit(index, (Byte) (product % m_base), ref newSize);
            }

            if (carry != 0) {
                var bytes = BitConverter.GetBytes(carry);
                for (var index = 0; index < bytes.Length; index++) {
                    c.SetDigit(size + index, bytes[index], ref newSize);
                }
            }

            c.Size = newSize;
        }

        //
        // Divides a BigInt by a single byte.
        //

        private void Divide (Int32 b) {
            Int32 carry = 0, quotient = 0;
            var bLen = Size;
            
            var newSize = 0;
            while (bLen-- > 0) {
                quotient = m_base * carry + GetDigit(bLen);
                carry = quotient % b;
                SetDigit(bLen, (Byte) (quotient / b), ref newSize);
            }

            Size = newSize;
        }

        //
        // Integer division of one BigInt by another.
        //

        internal static void Divide (BigInt numerator, BigInt denominator, ref BigInt quotient, ref BigInt remainder) {
            // Avoid extra computations in special cases.

            if (numerator < denominator) {
                quotient.Clear();
                remainder.CopyFrom(numerator);
                return;
            }
    
            if (numerator == denominator) {
                quotient.Clear(); quotient.SetDigit(0, 1); 
                remainder.Clear();
                return;
            }

            var dividend = new BigInt();
            dividend.CopyFrom(numerator);
            var divisor = new BigInt();
            divisor.CopyFrom(denominator);

            UInt32 zeroCount = 0;
            // We pad the divisor with zeros until its size equals that of the dividend.
            while (divisor.Size < dividend.Size) {
                divisor.Multiply(m_base);
                zeroCount++; 
            }

            if (divisor > dividend) {
                divisor.Divide(m_base);
                zeroCount--;
            }

            // Use school division techniques, make a guess for how many times
            // divisor goes into dividend, making adjustment if necessary.
            var a = 0;
            var b = 0;
            var c = 0;

            var hold = new BigInt();
            quotient.Clear();
            for (var index = 0; index <= zeroCount; index++) {
                a = dividend.Size == divisor.Size ? dividend.GetDigit(dividend.Size - 1) :
                                                    m_base * dividend.GetDigit(dividend.Size - 1) + dividend.GetDigit(dividend.Size - 2);
                b = divisor.GetDigit(divisor.Size - 1);
                c = a / b;

                if (c >= m_base) 
                    c = 0xFF;

                Multiply(divisor, c, ref hold);
                while (hold > dividend) {
                    c--;
                    Multiply(divisor, c, ref hold);
                }

                quotient.Multiply(m_base);
                Add(quotient, (Byte) c, ref quotient);
                Subtract(dividend, hold, ref dividend);
                divisor.Divide(m_base);
            }
            remainder.CopyFrom(dividend);
        }

        //
        // copies a BigInt value.
        //

        internal void CopyFrom (BigInt a) {
            Array.Copy(a.m_elements, m_elements, m_maxbytes);
            m_size = a.m_size;
        }

        //
        // This method returns true if the BigInt is equal to 0, false otherwise.
        //

        internal Boolean IsZero () {
            for (var index = 0; index < m_size; index++) {
                if (m_elements[index] != 0)
                    return false;
            }
            return true;
        }

        //
        // returns the array in machine format, i.e. little endian format (as an integer).
        //

        internal Byte[] ToByteArray() {
            var result = new Byte[Size];
            Array.Copy(m_elements, result, Size);
            return result;
        }

        //
        // zeroizes the content of the internal array.
        //

        internal void Clear () {
            m_size = 0;
        }

        //
        // Imports a hexadecimal string into a BigInt bit representation.
        //

        internal static Byte HexToByte (Char val) {
            if (val <= '9' && val >= '0')
                return (Byte) (val - '0');
            else if (val >= 'a' && val <= 'f')
                return (Byte) ((val - 'a') + 10);
            else if (val >= 'A' && val <= 'F')
                return (Byte) ((val - 'A') + 10);
            else
                return 0xFF;
        }

        internal static Byte[] DecodeHexString (String s) {
            var hexString = Utils.DiscardWhiteSpaces(s);
            var cbHex = (UInt32) hexString.Length / 2;
            var hex = new Byte[cbHex];
            var i = 0;
            for (var index = 0; index < cbHex; index++) {
                hex[index] = (Byte) ((HexToByte(hexString[i]) << 4) | HexToByte(hexString[i+1]));
                i += 2;
            }
            return hex;
        }

        internal void FromHexadecimal (String hexNum) {
            var hex = DecodeHexString(hexNum);
            Array.Reverse(hex);
            var size = Utils.GetHexArraySize(hex);
            Array.Copy(hex, m_elements, size);
            Size = size;
        }

        //
        // Imports a decimal string into a BigInt bit representation.
        //

        internal void FromDecimal (String decNum) {
            var c = new BigInt();
            var tmp = new BigInt();
            var length = decNum.Length;
            for (var index = 0; index < length; index++) {
                // just ignore invalid characters. No need to raise an exception.
                if (decNum[index] > '9' || decNum[index] < '0')
                    continue;
                Multiply(c, 10, ref tmp);
                Add(tmp, (Byte) (decNum[index] - '0'), ref c);
            }
            CopyFrom(c);
        }

        //
        // Exports the BigInt representation as a decimal string.
        //

        private static readonly Char[] decValues = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        internal String ToDecimal ()
        {
            if (IsZero())
                return "0";

            var ten = new BigInt(0xA);
            var numerator = new BigInt();
            var quotient = new BigInt();
            var remainder = new BigInt();

            numerator.CopyFrom(this);

            // Each hex digit can account for log(16) = 1.21 decimal digits. Times two hex digits in a byte
            // and m_size bytes used in this BigInt, yields the maximum number of characters for the decimal
            // representation of the BigInt.
            var dec = new Char[(Int32)Math.Ceiling(m_size * 2 * 1.21)];

            var index = 0;
            do
            {
                Divide(numerator, ten, ref quotient, ref remainder);
                dec[index++] = decValues[remainder.IsZero() ? 0 : (Int32)remainder.m_elements[0]];
                numerator.CopyFrom(quotient);
            } while (quotient.IsZero() == false);

            Array.Reverse(dec, 0, index);
            return new String(dec, 0, index);
        }
    }
}
