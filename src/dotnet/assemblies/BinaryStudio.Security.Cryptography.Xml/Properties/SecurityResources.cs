using System;

namespace BinaryStudio.Security.Cryptography.Xml.Properties
    {
    internal class SecurityResources : Resources
        {
        public static String GetResourceString(String key)
            {
            return ResourceManager.GetString(key);
            }
        }
    }