using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable
    {
    public class ResourceDescriptor
        {
        [DebuggerDisplay("{ToString(Identifier),nq}")]
        public ResourceIdentifier Identifier { get; }
        public ResourceDescriptor Owner { get; }
        internal IMAGE_RESOURCE_LEVEL Level { get; set; }
        public UInt32? CodePage { get; internal set; }
        public IList<Byte[]> Source { get; protected set; }
        public IList<ResourceDescriptor> Resources { get; private set; }

        #region M:ResourceDescriptor(ResourceDescriptor,ResourceIdentifier)
        protected internal ResourceDescriptor(ResourceDescriptor owner, ResourceIdentifier identifier)
            {
            Identifier = identifier;
            Owner = owner;
            Level = (owner != null)
                ? owner.Level + 1
                : 0;
            }
        #endregion
        #region M:ResourceDescriptor(ResourceDescriptor,ResourceIdentifier,Byte[])
        protected internal ResourceDescriptor(ResourceDescriptor owner, ResourceIdentifier identifier, Byte[] source)
            : this(owner, identifier)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Source = new[] { source };
            }
        #endregion
        #region M:AddRange(IEnumerable<ResourceDescriptor>)
        internal void AddRange(IEnumerable<ResourceDescriptor> descriptors)
            {
            if (descriptors == null) { throw new ArgumentNullException(nameof(descriptors)); }
            if (Resources == null) { Resources = new List<ResourceDescriptor>(); }
            foreach (var descriptor in descriptors)
                {
                Resources.Add(descriptor);
                }
            }
        #endregion
        #region M:ToString(ResourceIdentifier):String
        //[UsedImplicitly]
        private String ToString(ResourceIdentifier source)
            {
            if ((Level == IMAGE_RESOURCE_LEVEL.LEVEL_TYPE) && (source.Identifier.HasValue)) { return ((IMAGE_RESOURCE_TYPE)source.Identifier).ToString(); }
            if ((Level == IMAGE_RESOURCE_LEVEL.LEVEL_LANGUAGE) && (source.Identifier.HasValue)) {
                if (source.Identifier == 0) { return "(neutral)"; }
                return CultureInfo.GetCultureInfo(source.Identifier.Value).IetfLanguageTag;
                }
            return source.ToString();
            }
        #endregion

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return ToString(Identifier);
            }
        }
    }