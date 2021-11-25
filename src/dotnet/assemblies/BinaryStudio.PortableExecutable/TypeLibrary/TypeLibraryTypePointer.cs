using System;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    //[JsonConverter(typeof(Converter))]
    internal class TypeLibraryTypePointer : TypeLibraryTypeReference
        {
        public override Boolean IsPointer { get { return true; }}

        //private class Converter : TypeLibraryJsonConverter
        //    {
        //    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        //        {
        //        base.WriteJson(writer, value, serializer);
        //        }
        //    }


        public TypeLibraryTypePointer(ITypeLibraryTypeDescriptor typeref)
            :base(typeref)
            {
            if (typeref == null)
                {
                throw new ArgumentNullException(nameof(typeref));
                }
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return Name;
            }

        [JsonIgnore] public override String Name { get { return $"{UnderlyingType.Name}*"; }}
        [JsonIgnore] public override Guid? UniqueIdentifier { get { return null; }}
        }
    }