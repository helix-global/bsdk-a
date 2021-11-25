using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    [AttributeUsage(AttributeTargets.Class)]
    public class XamlSerializableAttribute : Attribute
        {
        public String FactoryMethodName { get; set; }
        }
    }