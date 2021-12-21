using System;
using System.ComponentModel;
using System.Diagnostics;
using BinaryStudio.DataProcessing;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters
    {
    public class Asn1DisplayNameAttribute : DisplayNameAttribute
        {
        public String Key { get; }
        public Asn1DisplayNameAttribute(String key) {
            Key = key;
            }

        /**
         * <summary>Gets the display name for a property, event, or public void method that takes no arguments stored in this attribute.</summary>
         * <returns>The display name.</returns>
         * */
        public override String DisplayName { get {
            #if DEBUG
            var r = Resources.ResourceManager.GetString(Key, PlatformContext.DefaultCulture);
            Debug.Assert(!String.IsNullOrWhiteSpace(r));
            return r;
            #else
            return Resources.ResourceManager.GetString(Key, LocalizationManager.DefaultCulture);
            #endif
            }
        }
        }
    }