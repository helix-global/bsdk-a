using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1Writer : IDisposable
        {
        private Stack<Stream> stack = new Stack<Stream>();
        public Asn1Writer(Stream stream)
            {
            stack.Push(stream);
            }

        #region M:WriteStartSequence
        public void WriteStartSequence()
            {
            stack.Push(new MemoryStream());
            }
        #endregion
        #region M:WriteEndSequence
        public void WriteEndSequence()
            {
            var buffer = ((MemoryStream)stack.Pop()).ToArray();
            var target = stack.Peek();
            WriteHeader(target, true, Asn1ObjectType.Sequence, buffer.Length);
            target.Write(buffer, 0, buffer.Length);
            }
        #endregion
        #region M:WriteObjectIdentifier(String)
        public void WriteObjectIdentifier(String value)
            {
            var target = stack.Peek();
            var sequence = value.Split('.').Select(Int64.Parse).ToArray();
            using (var writer = new MemoryStream()) {
                for (var i = 0; i < sequence.Length;) {
                    var n = sequence[i];
                    if (i == 0) {
                        switch (n)
                            {
                            case 0:
                                {
                                Debug.Assert(sequence.Length > i);
                                Debug.Assert(sequence[i + 1] < 40);
                                writer.WriteByte((Byte)sequence[i + 1]);
                                i++;
                                }
                                break;
                            case 1:
                                {
                                Debug.Assert(sequence.Length > i);
                                Debug.Assert(sequence[i + 1] < 40);
                                writer.WriteByte((Byte)(sequence[i + 1] + 40));
                                i++;
                                }
                                break;
                            case 2:
                                {
                                Debug.Assert(sequence.Length > i);
                                writer.WriteByte((Byte)(sequence[i + 1] + 80));
                                i++;
                                }
                                break;
                            }
                        }
                    else
                        {
                        var r = new LinkedList<Byte>();
                        var j = 0;
                        while (true) {
                            if (n < 128) {
                                r.AddFirst((j == 0) ? (Byte)(n) : (Byte)(n | 0x80));
                                break;
                                }
                            r.AddFirst((Byte)(n % 128));
                            n /= 128;
                            j++;
                            }

                        foreach (var v in r) { writer.WriteByte(v); }
                        }
                    i++;
                    }
                var buffer = writer.ToArray();
                WriteHeader(target, false, Asn1ObjectType.ObjectIdentifier, buffer.Length);
                target.Write(buffer, 0, buffer.Length);
                }
            }
        #endregion
        #region M:WriteInteger(Int32)
        public void WriteInteger(Int32 value)
            {
            WriteInteger((BigInteger)value);
            }
        #endregion
        #region M:WriteInteger(BigInteger)
        public void WriteInteger(BigInteger value)
            {
            var target = stack.Peek();
            var buffer = value.ToByteArray().Reverse().ToArray();
            WriteHeader(target, false, Asn1ObjectClass.Universal, (SByte)Asn1ObjectType.Integer,buffer.Length);
            target.Write(buffer, 0, buffer.Length);
            }
        #endregion

        public void WriteStartOctetString()
            {
            }

        public void WriteEndOctetString()
            {
            }

        #region M:WriteOctetString(Byte[])
        public void WriteOctetString(Byte[] value)
            {
            var target = stack.Peek();
            WriteHeader(target, false, Asn1ObjectType.OctetString, value.Length);
            target.Write(value, 0, value.Length);
            }
        #endregion
        #region M:WriteBitString(Byte[])
        public void WriteBitString(Byte[] value)
            {
            WriteBitString(0, value);
            }
        #endregion
        #region M:WriteBitString(Byte,Byte[])
        public void WriteBitString(Byte n, Byte[] value)
            {
            var target = stack.Peek();
            WriteHeader(target, false, Asn1ObjectType.BitString, value.Length + 1);
            target.WriteByte(n);
            target.Write(value, 0, value.Length);
            }
        #endregion
        #region M:WriteHeader(Stream,Boolean,Asn1ObjectType,Int64?)
        private static void WriteHeader(Stream target, Boolean constructed, Asn1ObjectType type, Int64? length)
            {
            WriteHeader(target, constructed, Asn1ObjectClass.Universal, (SByte)type, length);
            }
        #endregion
        #region M:WriteHeader(Stream,Boolean,Asn1ObjectClass,SByte,Int64?)
        private static void WriteHeader(Stream target, Boolean constructed, Asn1ObjectClass @class, SByte type, Int64? length)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            if (length < 0)     { return; }
            var r = constructed ? 0x20 : 0x00;
            r |= ((Byte)@class) << 6;
            r |= (Byte)type;
            target.WriteByte((Byte)r);
            if (length < 0x80) { target.WriteByte((Byte)length); }
            else
                {
                var n = new List<Byte>();
                while (length > 0) {
                    n.Add((Byte)(length & 0xFF));
                    length >>= 8;
                    }
                var c = n.Count;
                target.WriteByte((Byte)(c | 0x80));
                for (var i = c - 1;i >= 0; i--) {
                    target.WriteByte(n[i]);
                    }
                }
            }
        #endregion

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            while (stack.Count > 0) {
                var o = stack.Pop();
                o.Dispose();
                }
            }
        }
    }