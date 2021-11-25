using System;
using System.Diagnostics;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal static class Contracts
        {
        public static void Requires(Boolean condition)
            {
            #if DEBUG
            Debug.Assert(condition);
            #else
            if (!condition)
                {
                throw new Exception(condition);
                }
            #endif
            }

        public static void Requires(Boolean condition, String userMessage)
            {
            #if DEBUG
            Debug.Assert(condition, userMessage);
            #else
            if (!condition)
                {
                throw new Exception(condition, userMessage);
                }
            #endif
            }
        }
    }