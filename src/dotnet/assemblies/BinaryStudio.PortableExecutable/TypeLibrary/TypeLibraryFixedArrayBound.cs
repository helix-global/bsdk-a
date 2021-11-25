using System;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    public class TypeLibraryFixedArrayBound
        {
        public Int64 Size { get; }
        public Int64 LowerBound { get; }
        public TypeLibraryFixedArrayBound(Int64 size, Int64 lowerbound) {
            Size = size;
            LowerBound = lowerbound;
            }

        //private class Converter : TypeLibraryJsonConverter
        //    {
        //    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        //        {
        //        base.WriteJson(writer, value, serializer);
        //        }
        //    }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"{LowerBound}..{LowerBound + Size - 1}";
            }
        }
    }