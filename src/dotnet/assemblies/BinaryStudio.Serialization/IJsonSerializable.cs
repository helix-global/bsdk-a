using Newtonsoft.Json;

namespace BinaryStudio.Serialization
    {
    public interface IJsonSerializable
        {
        void WriteJson(JsonWriter writer, JsonSerializer serializer);
        }
    }
