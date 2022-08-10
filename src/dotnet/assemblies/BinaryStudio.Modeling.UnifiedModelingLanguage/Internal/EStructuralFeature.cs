using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EStructuralFeature : ENamedElement, StructuralFeature
        {
        public EStructuralFeature(String identifer)
            : base(identifer)
            {
            }

        public Boolean IsLeaf { get; }
        public RedefinableElement[] RedefinedElement { get; }
        public Classifier[] RedefinitionContext { get; }
        public Classifier[] FeaturingClassifier { get; }
        public Boolean IsStatic { get; }
        public Type Type { get; }
        public Boolean IsOrdered { get; }
        public Boolean IsUnique { get; }
        public Int64 Lower { get; }
        public ValueSpecification LowerValue { get; }
        public UnlimitedNatural Upper { get; }
        public ValueSpecification UpperValue { get; }
        public Boolean IsReadOnly { get; }
        }
    }