using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider.Internal.Fintech
    {
    public class FintechCryptographicMessageProvider : ICustomCryptographicMessageProvider
        {
        private const UInt32 FT_SIGNATURE_CONTAINER_HEADER_MAGIC  = 0x00035446;
        private const UInt32 FT_SIGNATURE_CONTAINER_BARRIER_MAGIC = 0xFF00EE10;
        private const Int32 DEFAULT_BUFFER_SIZE = (1024 * 1024 * 5);

        public Boolean VerifyMessage(CryptographicContext context, Stream input, Stream output, CryptographicMessageFlags type,
            out IList<IX509Certificate> certificates, out Exception e)
            {
            if (input == null)  { throw new ArgumentNullException(nameof(input)); }
            if (!input.CanSeek) { throw new ArgumentOutOfRangeException(nameof(input)); }
            e = null;
            certificates = new List<IX509Certificate>();
            try
                {
                try
                    {
                    var flag = false;
                    var position = input.Position;
                    if (ReadUInt32(input, out var magic)) {
                        if (magic == FT_SIGNATURE_CONTAINER_HEADER_MAGIC) {
                            if (ReadPascalString(input, out var hashalgid)) {
                                if (ReadBlock(input, out var blob)) {
                                    if (ReadUInt32(input, out magic)) {
                                        if (magic == FT_SIGNATURE_CONTAINER_BARRIER_MAGIC) {
                                            var certificate = new X509Certificate(new Asn1Certificate(Asn1Object.Load(new ReadOnlyMemoryMappingStream(blob)).FirstOrDefault()));
                                            certificates.Add(certificate);
                                            using (var hashalg = new CryptHashAlgorithm(context, CryptographicObject.OidToAlgId(certificate.HashAlgorithm))) {
                                                using (var key = context.ImportPublicKey(certificate.Handle)) {
                                                    var currp = input.Position;
                                                    var datasize = input.Seek(-sizeof(UInt32), SeekOrigin.End);
                                                    if (ReadInt32(input, out var signsz)) {
                                                        if (signsz*8 != key.KeyLength)
                                                            {
                                                            throw new InvalidDataException("Размер сигнатуры не соответствует алгоритму подписи") {
                                                                Data =
                                                                    {
                                                                    {"KeyLength", key.KeyLength},
                                                                    {"SignatureLength", signsz}
                                                                    }
                                                                };
                                                            }
                                                        input.Seek(-(sizeof(UInt32) + signsz), SeekOrigin.End);
                                                        if (ReadBlock(input, signsz, out var signature)) {
                                                            input.Seek(currp, SeekOrigin.Begin);
                                                            datasize -= signsz;
                                                            datasize -= currp;
                                                            var buffer = new Byte[DEFAULT_BUFFER_SIZE];
                                                            for (;datasize > 0;) {
                                                                var sz = input.Read(buffer, 0, (Int32)Math.Min(buffer.Length, datasize));
                                                                ((IHashOperation)hashalg).HashCore(buffer, 0, sz);
                                                                output?.Write(buffer, 0, sz);
                                                                datasize -= sz;
                                                                }
                                                            hashalg.VerifySignature(signature, key);
                                                            flag = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    if (!flag)
                        {
                        input.Seek(position, SeekOrigin.Begin);
                        certificates.Clear();
                        return false;
                        }
                    return true;
                    }
                catch (Exception exception)
                    {
                    e = exception;
                    return false;
                    }
                }
            finally
                {
                certificates = certificates.Distinct().ToArray();
                }
            }

        #region M:ReadUInt32(Stream,[Out]UInt32):Boolean
        private static unsafe Boolean ReadUInt32(Stream stream, out UInt32 r)
            {
            r = 0;
            var buffer = new Byte[sizeof(UInt32)];
            if (stream.Read(buffer,0, sizeof(UInt32)) == sizeof(UInt32)) {
                fixed (Byte* i = buffer) {
                    r = *(UInt32*)i;
                    return true;
                    }
                }
            return false;
            }
        #endregion
        #region M:ReadInt32(Stream,[Out]Int32):Boolean
        private static unsafe Boolean ReadInt32(Stream stream, out Int32 r)
            {
            r = 0;
            var buffer = new Byte[sizeof(Int32)];
            if (stream.Read(buffer,0, sizeof(Int32)) == sizeof(Int32)) {
                fixed (Byte* i = buffer) {
                    r = *(Int32*)i;
                    return true;
                    }
                }
            return false;
            }
        #endregion
        #region M:ReadByte(Stream,[Out]Byte):Boolean
        private static Boolean ReadByte(Stream stream, out Byte r)
            {
            r = 0;
            var buffer = new Byte[sizeof(Byte)];
            if (stream.Read(buffer,0, sizeof(Byte)) == sizeof(Byte)) {
                r = buffer[0];
                return true;
                }
            return false;
            }
        #endregion
        #region M:ReadPascalString(Stream,[Out]String):Boolean
        private static Boolean ReadPascalString(Stream stream, out String r) {
            return ReadPascalString(stream, Encoding.UTF8, out r);
            }
        #endregion
        #region M:ReadPascalString(Stream,Encoding,[Out]String):Boolean
        private static Boolean ReadPascalString(Stream stream, Encoding encoding, out String r) {
            r = null;
            if (ReadByte(stream, out var length)) {
                var buffer = new Byte[length];
                if (stream.Read(buffer, 0, length) == length) {
                    r = encoding.GetString(buffer);
                    return true;
                    }
                }
            return false;
            }
        #endregion
        #region M:ReadBlock(Stream,[Out]Byte[]):Boolean
        private static Boolean ReadBlock(Stream stream,out Byte[] r) {
            r = null;
            if (ReadInt32(stream, out var size)) {
                r = new Byte[size];
                if (stream.Read(r, 0, size) == size) { return true; }
                r = null;
                }
            return false;
            }
        #endregion
        #region M:ReadBlock(Stream,Int32,[Out]Byte[]):Boolean
        private static Boolean ReadBlock(Stream stream,Int32 size, out Byte[] r) {
            r = new Byte[size];
            if (stream.Read(r, 0, size) == size) { return true; }
            r = null;
            return false;
            }
        #endregion
        }
    }