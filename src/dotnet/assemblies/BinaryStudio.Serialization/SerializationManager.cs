using System;
using Newtonsoft.Json;

namespace BinaryStudio.Serialization
    {
    public class SerializationManager
        {
        public static event CanSerializeEventHandler CanSerialize;
        public static event SerializeEventHandler Serialize;

        internal static void ProcessObject(Object sender, JsonWriter writer, JsonSerializer serializer) {
            var e = new CanSerializeEventArgs();
            CanSerialize?.Invoke(sender, e);
            if (e.CanSerialize) {
                Serialize?.Invoke(sender, new SerializeEventArgs(writer, serializer));
                }
            }
        }
    }