using System;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    internal interface ITypeLibraryTypeReferenceSerializer
        {
        void WriteJson(JsonWriter writer, JsonSerializer serializer);
        }
    }