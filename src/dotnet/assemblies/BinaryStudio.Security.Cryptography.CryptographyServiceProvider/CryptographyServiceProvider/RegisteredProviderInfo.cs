using System;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public sealed class RegisteredProviderInfo
        {
        public CRYPT_PROVIDER_TYPE ProviderType { get; }
        public String ProviderName { get; }
        public Type HostType { get; }

        internal RegisteredProviderInfo(CRYPT_PROVIDER_TYPE providerType, String name, Type type) {
            ProviderType = providerType;
            ProviderName = name;
            HostType = type;
            }
        }
    }