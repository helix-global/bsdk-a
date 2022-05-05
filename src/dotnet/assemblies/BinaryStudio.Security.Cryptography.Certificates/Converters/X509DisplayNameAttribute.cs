using System;
using System.ComponentModel;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.Certificates.Properties;

namespace BinaryStudio.Security.Cryptography.Certificates.Converters
    {
    internal class X509DisplayNameAttribute : DisplayNameAttribute
        {
        public String Key { get; }
        public X509DisplayNameAttribute(String key) {
            Key = key;
            }

        /**
         * <summary>Gets the display name for a property, event, or public void method that takes no arguments stored in this attribute.</summary>
         * <returns>The display name.</returns>
         * */
        public override String DisplayName { get {
            return Resources.ResourceManager.GetString(Key, PlatformContext.DefaultCulture);
            }}
        }
    }