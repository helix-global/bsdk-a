using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BinaryStudio.PortableExecutable
    {
    public class TypeLibraryFixedArrayBoundCollection : ReadOnlyCollection<TypeLibraryFixedArrayBound>
        {
        public TypeLibraryFixedArrayBoundCollection(IList<TypeLibraryFixedArrayBound> source)
            :base(source)
            {
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return String.Join(String.Empty, Items.Select(i => $"[{i.Size}]"));
            }
        }
    }