using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Diagnostics
    {
    [Serializable]
    public class ExceptionObjectDecorator : ISerializable, IExceptionSerializable
        {
        public Object Source { get; }

        /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
            }

        /// <summary>The special constructor is used to deserialize values.</summary>
        /// <param name="info">The data needed to deserialize an object.</param>
        /// <param name="context">Describes the source of a given serialized stream, and provides an additional caller-defined context.</param>
        protected ExceptionObjectDecorator(SerializationInfo info, StreamingContext context)
            {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }
            }

        public ExceptionObjectDecorator(Object source)
            {
            Source = source;
            }

        /// <inheritdoc/>
        public void WriteTo(TextWriter target)
            {
            using (var writer = new JsonTextWriter(target){
                    Formatting = Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                    }) {
                var serializer = new JsonSerializer();
                ((IExceptionSerializable)this).WriteTo(writer, serializer);
                writer.Flush();
                }
            }

        /// <inheritdoc/>
        public void WriteTo(JsonWriter writer, JsonSerializer serializer) {
            if (Source is IExceptionSerializable r) {
                r.WriteTo(writer, serializer);
                }
            else
                {
                using (writer.ObjectScope(serializer)) {
                    foreach (var descriptor in TypeDescriptor.GetProperties(Source).OfType<PropertyDescriptor>()) {
                        writer.WriteValue(serializer, descriptor.Name, descriptor.GetValue(Source));
                        }
                    }
                }
            }
        }
    }