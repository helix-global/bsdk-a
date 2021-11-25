using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Cryptography;

    // abstract class providing symmetric key wrap implementation
    internal static class SymmetricKeyWrap {
        private readonly static Byte[] s_rgbTripleDES_KW_IV = {0x4a, 0xdd, 0xa2, 0x2c, 0x79, 0xe8, 0x21, 0x05};
        private readonly static Byte[] s_rgbAES_KW_IV = {0xa6, 0xa6, 0xa6, 0xa6, 0xa6, 0xa6, 0xa6, 0xa6};

        //
        // internal static methods
        //

        // CMS TripleDES KeyWrap as described in "http://www.w3.org/2001/04/xmlenc#kw-tripledes"
        [SuppressMessage("Microsoft.Security.Cryptography", "CA5354:SHA1CannotBeUsed", Justification="Part of the spec (CMSKeyChecksum): https://www.w3.org/TR/2002/REC-xmlenc-core-20021210/Overview.html#kw-tripledes")]
        [SuppressMessage("Microsoft.Security.Cryptography", "CA5353:TripleDESCannotBeUsed", Justification="Used for implementation of 3DES keywrap - only by user choice")]
        internal static Byte[] TripleDESKeyWrapEncrypt (Byte[] rgbKey, Byte[] rgbWrappedKeyData) {
            // checksum the key
            var sha = new SHA1CryptoServiceProvider();
            var rgbCKS = sha.ComputeHash(rgbWrappedKeyData);

            // generate a random IV
            var rng = new RNGCryptoServiceProvider();
            var rgbIV = new Byte[8];
            rng.GetBytes(rgbIV);

            // rgbWKCS = rgbWrappedKeyData | (first 8 bytes of the hash)
            var rgbWKCKS = new Byte[rgbWrappedKeyData.Length + 8];
            var tripleDES = new TripleDESCryptoServiceProvider();
            // Don't add padding, use CBC mode: for example, a 192 bits key will yield 40 bytes of encrypted data
            tripleDES.Padding = PaddingMode.None;
            var enc1 = tripleDES.CreateEncryptor(rgbKey, rgbIV);
            Buffer.BlockCopy(rgbWrappedKeyData, 0, rgbWKCKS, 0, rgbWrappedKeyData.Length);
            Buffer.BlockCopy(rgbCKS, 0, rgbWKCKS, rgbWrappedKeyData.Length, 8);
            var temp1 = enc1.TransformFinalBlock(rgbWKCKS, 0, rgbWKCKS.Length);
            var temp2 = new Byte[rgbIV.Length + temp1.Length];
            Buffer.BlockCopy(rgbIV, 0, temp2, 0, rgbIV.Length);
            Buffer.BlockCopy(temp1, 0, temp2, rgbIV.Length, temp1.Length);
            // temp2 = REV (rgbIV | E_k(rgbWrappedKeyData | rgbCKS))
            Array.Reverse(temp2);

            var enc2 = tripleDES.CreateEncryptor(rgbKey, s_rgbTripleDES_KW_IV);
            return enc2.TransformFinalBlock(temp2, 0, temp2.Length);
        }

        [SuppressMessage("Microsoft.Security.Cryptography", "CA5353:TripleDESCannotBeUsed", Justification="Used for implementation of 3DES keywrap - only by user choice")]
        [SuppressMessage("Microsoft.Security.Cryptography", "CA5354:SHA1CannotBeUsed", Justification="Part of the spec (CMSKeyChecksum): https://www.w3.org/TR/2002/REC-xmlenc-core-20021210/Overview.html#kw-tripledes")]
        internal static Byte[] TripleDESKeyWrapDecrypt (Byte[] rgbKey, Byte[] rgbEncryptedWrappedKeyData) {
            // Check to see whether the length of the encrypted key is reasonable
            if (rgbEncryptedWrappedKeyData.Length != 32 && rgbEncryptedWrappedKeyData.Length != 40 
                && rgbEncryptedWrappedKeyData.Length != 48) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_KW_BadKeySize"));

            var tripleDES = new TripleDESCryptoServiceProvider();
            // Assume no padding, use CBC mode
            tripleDES.Padding = PaddingMode.None;
            var dec1 = tripleDES.CreateDecryptor(rgbKey, s_rgbTripleDES_KW_IV);
            var temp2 = dec1.TransformFinalBlock(rgbEncryptedWrappedKeyData, 0, rgbEncryptedWrappedKeyData.Length);
            Array.Reverse(temp2);
            // Get the IV and temp1
            var rgbIV = new Byte[8];
            Buffer.BlockCopy(temp2, 0, rgbIV, 0, 8);
            var temp1 = new Byte[temp2.Length - rgbIV.Length];
            Buffer.BlockCopy(temp2, 8, temp1, 0, temp1.Length);

            var dec2 = tripleDES.CreateDecryptor(rgbKey, rgbIV);
            var rgbWKCKS = dec2.TransformFinalBlock(temp1, 0, temp1.Length);

            // checksum the key
            var rgbWrappedKeyData = new Byte[rgbWKCKS.Length - 8];
            Buffer.BlockCopy(rgbWKCKS, 0, rgbWrappedKeyData, 0, rgbWrappedKeyData.Length);
            var sha = new SHA1CryptoServiceProvider();
            var rgbCKS = sha.ComputeHash(rgbWrappedKeyData);
            for (Int32 index = rgbWrappedKeyData.Length, index1 = 0; index < rgbWKCKS.Length; index++, index1++)
                if (rgbWKCKS[index] != rgbCKS[index1])
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_BadWrappedKeySize"));
            return rgbWrappedKeyData;
        }

        // AES KeyWrap described in "http://www.w3.org/2001/04/xmlenc#kw-aes***", as suggested by NIST
        internal static Byte[] AESKeyWrapEncrypt (Byte[] rgbKey, Byte[] rgbWrappedKeyData) {
            var N = rgbWrappedKeyData.Length >> 3;
            // The information wrapped need not actually be a key, but it needs to be a multiple of 64 bits
            if ((rgbWrappedKeyData.Length % 8 != 0) || N <= 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_KW_BadKeySize"));

            var aes = Aes.Create();
            aes.Key = rgbKey;
            // Use ECB mode, no padding
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using (aes)
            using (var enc = aes.CreateEncryptor()) {
                // special case: only 1 block -- 8 bytes
                if (N == 1) {
                    // temp = 0xa6a6a6a6a6a6a6a6 | P(1)
                    var temp = new Byte[s_rgbAES_KW_IV.Length + rgbWrappedKeyData.Length];
                    Buffer.BlockCopy(s_rgbAES_KW_IV, 0, temp, 0, s_rgbAES_KW_IV.Length);
                    Buffer.BlockCopy(rgbWrappedKeyData, 0, temp, s_rgbAES_KW_IV.Length, rgbWrappedKeyData.Length);
                    return enc.TransformFinalBlock(temp, 0, temp.Length);
                }
                // second case: more than 1 block
                Int64 t = 0;
                var rgbOutput = new Byte[(N + 1) << 3];
                // initialize the R_i's
                Buffer.BlockCopy(rgbWrappedKeyData, 0, rgbOutput, 8, rgbWrappedKeyData.Length);
                var rgbA = new Byte[8];
                var rgbBlock = new Byte[16];
                Buffer.BlockCopy(s_rgbAES_KW_IV, 0, rgbA, 0, 8);
                for (var j = 0; j <= 5; j++) {
                    for (var i = 1; i <= N; i++) {
                        t = i + j * N;
                        Buffer.BlockCopy(rgbA, 0, rgbBlock, 0, 8);
                        Buffer.BlockCopy(rgbOutput, 8 * i, rgbBlock, 8, 8);
                        var rgbB = enc.TransformFinalBlock(rgbBlock, 0, 16);
                        for (var k = 0; k < 8; k++) {
                            var tmp = (Byte)((t >> (8 * (7 - k))) & 0xFF);
                            rgbA[k] = (Byte)(tmp ^ rgbB[k]);
                        }
                        Buffer.BlockCopy(rgbB, 8, rgbOutput, 8 * i, 8);
                    }
                }
                // Set the first block of rgbOutput to rgbA
                Buffer.BlockCopy(rgbA, 0, rgbOutput, 0, 8);
                return rgbOutput;
            }
        }

        internal static Byte[] AESKeyWrapDecrypt(Byte[] rgbKey, Byte[] rgbEncryptedWrappedKeyData) {
            var N = (rgbEncryptedWrappedKeyData.Length >> 3) - 1;
            // The information wrapped need not actually be a key, but it needs to be a multiple of 64 bits
            if ((rgbEncryptedWrappedKeyData.Length % 8 != 0) || N <= 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_KW_BadKeySize"));

            var rgbOutput = new Byte[N << 3];

            var aes = Aes.Create();
            aes.Key = rgbKey;
            // Use ECB mode, no padding
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using (aes)
            using (var dec = aes.CreateDecryptor()) {
                // special case: only 1 block -- 8 bytes
                if (N == 1) {
                    var temp = dec.TransformFinalBlock(rgbEncryptedWrappedKeyData, 0, rgbEncryptedWrappedKeyData.Length);
                    // checksum the key
                    for (var index = 0; index < 8; index++)
                        if (temp[index] != s_rgbAES_KW_IV[index])
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_BadWrappedKeySize"));
                    // rgbOutput is LSB(temp)
                    Buffer.BlockCopy(temp, 8, rgbOutput, 0, 8);
                    return rgbOutput;
                }
                // second case: more than 1 block
                Int64 t = 0;
                // initialize the C_i's
                Buffer.BlockCopy(rgbEncryptedWrappedKeyData, 8, rgbOutput, 0, rgbOutput.Length);
                var rgbA = new Byte[8];
                var rgbBlock = new Byte[16];
                Buffer.BlockCopy(rgbEncryptedWrappedKeyData, 0, rgbA, 0, 8);
                for (var j = 5; j >= 0; j--) {
                    for (var i = N; i >= 1; i--) {
                        t = i + j * N;
                        for (var k = 0; k < 8; k++) {
                            var tmp = (Byte)((t >> (8 * (7 - k))) & 0xFF);
                            rgbA[k] ^= tmp;
                        }
                        Buffer.BlockCopy(rgbA, 0, rgbBlock, 0, 8);
                        Buffer.BlockCopy(rgbOutput, 8 * (i - 1), rgbBlock, 8, 8);
                        var rgbB = dec.TransformFinalBlock(rgbBlock, 0, 16);
                        Buffer.BlockCopy(rgbB, 8, rgbOutput, 8 * (i - 1), 8);
                        Buffer.BlockCopy(rgbB, 0, rgbA, 0, 8);
                    }
                }

                // checksum the key
                for (var index = 0; index < 8; index++) {
                    if (rgbA[index] != s_rgbAES_KW_IV[index]) {
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_BadWrappedKeySize"));
                    }
                }

                return rgbOutput;
            }
        }
    }
}
