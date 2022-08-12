using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class ENamespace : ENamedElement, Namespace
        {
        public ElementImport[] ElementImport { get; }
        public PackageableElement[] ImportedMember { get; }
        public NamedElement[] Member { get; }
        public NamedElement[] OwnedMember { get; }
        public Constraint[] OwnedRule { get; }
        public PackageImport[] PackageImport { get; }

        public ENamespace(String identifer)
            : base(identifer)
            {
            }
        }
    }