using System;
using System.Text;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable
    {
    public class ResourceIdentifier
        {
        public Int32? Identifier { get; }
        public String Name { get; }
        internal IMAGE_RESOURCE_LEVEL? Level { get; set; }

        #region M:ResourceIdentifier(Int32)
        protected internal ResourceIdentifier(Int32 identifier)
            {
            Identifier = identifier;
            }
        #endregion
        #region M:ResourceIdentifier(String)
        protected internal ResourceIdentifier(String name)
            {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            Name = name;
            }
        #endregion
        #region M:ResourceIdentifier(IMAGE_RESOURCE_DIRECTORY*,IMAGE_DIRECTORY_ENTRY_RESOURCE*)
        internal unsafe ResourceIdentifier(Byte* directory, IMAGE_DIRECTORY_ENTRY_RESOURCE* source)
            {
            if ((source->NameOffset & 0x80000000) == 0x80000000) {
                var offset = (source->NameOffset & 0x7FFFFFFF);
                var r = ((Byte*)directory + offset);
                Name = Encoding.Unicode.GetString(r + 2, *(UInt16*)r * 2);
                }
            else
                {
                Identifier = (Int32)source->IntegerId;
                }
            }
        #endregion

        #region M:ToString:String
        public override String ToString()
            {
            return (Identifier != null)
                ? (Level == IMAGE_RESOURCE_LEVEL.LEVEL_TYPE)
                    ? ((IMAGE_RESOURCE_TYPE)Identifier).ToString()
                    : Identifier.ToString()
                : Name;
            }
        #endregion
        }
    }