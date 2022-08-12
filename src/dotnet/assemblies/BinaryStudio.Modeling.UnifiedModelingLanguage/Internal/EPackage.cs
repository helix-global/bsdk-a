using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EPackage : ENamespace, Package, NamedElement
        {
        public TemplateParameter OwningTemplateParameter { get; }
        public TemplateParameter TemplateParameter { get; }
        public TemplateSignature OwnedTemplateSignature { get; }
        public TemplateBinding[] TemplateBinding { get; }
        public String URI { get; }
        public IList<Package> NestedPackage { get; }
        public Package NestingPackage { get; }

        public IList<Type> OwnedType { get {
            return new ReadOnlyCollection<Type>(
                OwnedElement.
                    OfType<Type>().
                    ToArray());
            }}

        public PackageMerge[] PackageMerge { get; }

        public IList<PackageableElement> PackagedElement { get {
            return new ReadOnlyCollection<PackageableElement>(
                OwnedElement.
                    OfType<PackageableElement>().
                    ToArray());
            }}

        public EPackage(String identifer)
            : base(identifer)
            {
            }

        #region P:Visibility:VisibilityKind
        public VisibilityKind Visibility { get;set; }
        VisibilityKind? NamedElement.Visibility {
            get { return Visibility; }
            set
                {
                if (value == null) { throw new ArgumentOutOfRangeException(nameof(value)); }
                Visibility = (VisibilityKind)value;
                }
            }
        #endregion
        }
    }