using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    #if A
    internal class MSFTPropertyInfo : PortablePropertyInfo
        {
        public PortableMemberTypes MemberType { get; }
        public IList<Attribute> CustomAttributes { get; }
        public String Name { get; }
        public String HelpString { get; }
        public PortableTypeInfo DeclaringType { get; }
        public Int32? Id { get; }
        public PortablePropertyAttributes Attributes { get; }
        public PortableMethodInfo GetMethod { get; }
        public PortableMethodInfo SetMethod { get; }
        public PortableTypeInfo PropertyType { get; }
        public Boolean CanRead { get; }
        public Boolean CanWrite { get; }
        public IList<PortableParameterInfo> Parameters { get; }

        public MSFTPropertyInfo(PortableTypeInfo declaringtype, PortableMethodInfo getmethod, PortableMethodInfo setmethod) {
            Attributes = PortablePropertyAttributes.None;
            MemberType = PortableMemberTypes.Property;
            DeclaringType = declaringtype;
            GetMethod = getmethod;
            SetMethod = setmethod;
            var mi = (getmethod != null) ? getmethod : setmethod;
            Id   = mi.Id;
            Name = mi.Name;
            var helpstring = (DescriptionAttribute)mi.GetCustomAttributes(typeof(DescriptionAttribute)).FirstOrDefault();
            if (helpstring != null) { HelpString = helpstring.Description; }
            CustomAttributes =
                ExcludeAttribute(mi.CustomAttributes,
                    new []{
                        typeof(DescriptionAttribute),
                        typeof(PortableTypeLibraryPropertyGetAttribute),
                        typeof(PortableTypeLibraryPropertyPutAttribute),
                        typeof(PortableTypeLibraryPropertyPutRefAttribute)
                        }).ToList();
            if (HelpString != null) {
                if (HelpString.StartsWith("get ", StringComparison.OrdinalIgnoreCase)) { HelpString = HelpString.Substring(3).Trim(); }
                if (!String.IsNullOrWhiteSpace(HelpString)) { CustomAttributes.Add(new DescriptionAttribute(HelpString)); }
                }
            CustomAttributes = new ReadOnlyCollection<Attribute>(CustomAttributes);
            PropertyType = (getmethod != null)
                ? Dereference(getmethod.Parameters[getmethod.Parameters.Count - 1].ParameterType)
                : setmethod.Parameters[setmethod.Parameters.Count - 1].ParameterType;
            Parameters = new PortableParameterInfo[mi.Parameters.Count - 1];
            for (var i = 0; i < Parameters.Count; ++i) {
                Parameters[i] = mi.Parameters[i];
                }
            CanRead  = (GetMethod != null);
            CanWrite = (SetMethod != null);
            }

        private static PortableTypeInfo Dereference(PortableTypeInfo source)
            {
            if (!source.IsPointer) { throw new ArgumentOutOfRangeException(nameof(source)); }
            return source.UnderlyingType;
            }

        Boolean IPortableCustomAttributeProvider.IsDefined(Type attribute) { return CustomAttributes.Any(i => (i.GetType() == attribute) || i.GetType().IsSubclassOf(attribute)); }
        Attribute[] IPortableCustomAttributeProvider.GetCustomAttributes(Type attribute) { return CustomAttributes.Where(i => (i.GetType() == attribute) || i.GetType().IsSubclassOf(attribute)).ToArray(); }

        private static IEnumerable<Attribute> ExcludeAttribute(IEnumerable<Attribute> attributes, Type[] types) {
            foreach (var attribute in attributes) {
                var r = attribute.GetType();
                var flags = true;
                for (var i = 0; i < types.Length; i++) {
                    if (r == types[i])            { flags = false; break; }
                    if (r.IsSubclassOf(types[i])) { flags = false; break; }
                    }
                if (flags) { yield return attribute; }
                }
            }
        }
    #endif
    }