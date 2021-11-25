using System;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    public sealed class TypeLibraryCustomAttribute
        {
        public Guid UniqueIdentifier { get; }
        public Object Value {  get; }
        public TypeLibraryCustomAttribute(Guid g, Object o) {
            UniqueIdentifier = g;
            Value = o;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            var r = new StringBuilder("custom");
            r.Append('(');
            r.Append(UniqueIdentifier.ToString("D"));
            r.Append(", ");
            if (Value is String) {
                r.Append('"');
                r.Append(Value);
                r.Append('"');
                }
            else
                {
                r.Append(Value);
                }
            r.Append(')');
            return r.ToString();
            }
        }
    }