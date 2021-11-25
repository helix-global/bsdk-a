using System;
using Newtonsoft.Json;

namespace BinaryStudio.Serialization
    {
    public class SerializeEventArgs : EventArgs
        {
        public JsonWriter Writer { get; }
        public JsonSerializer Serializer { get; }
        public SerializeEventArgs(JsonWriter writer, JsonSerializer serializer) {
            Writer = writer;
            Serializer = serializer;
            }
        }

    public delegate void SerializeEventHandler(Object sender, SerializeEventArgs e);
    }