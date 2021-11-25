using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    public interface ICustomXmlSerializable
        {
        WindowProfileSerializationVariants SerializationVariants { get; }

        ICustomXmlSerializer CreateSerializer();

        Type GetSerializedType();
        }
    }