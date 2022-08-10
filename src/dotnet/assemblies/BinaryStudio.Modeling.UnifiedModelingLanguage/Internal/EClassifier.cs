using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EClassifier : ENamespace, Classifier, NamedElement
        {
        public EClassifier(String identifer)
            : base(identifer)
            {
            }

        public TemplateParameter OwningTemplateParameter { get; }
        public ClassifierTemplateParameter TemplateParameter { get; }
        public RedefinableTemplateSignature OwnedTemplateSignature { get; }

        TemplateParameter ParameterableElement.TemplateParameter
            {
            get { return TemplateParameter; }
            }

        public Package Package { get; }
        public Boolean IsLeaf { get; }
        public RedefinableElement[] RedefinedElement { get; }
        public Classifier[] RedefinitionContext { get; }
        public UseCase[] OwnedUseCase { get; }
        public UseCase[] UseCase { get; }
        public CollaborationUse[] CollaborationUse { get; }
        public CollaborationUse Representation { get; }
        public GeneralizationSet[] PowertypeExtent { get; }
        public Property[] Attribute { get; }
        public Feature[] Feature { get; }
        public Classifier[] General { get; }
        public Generalization[] Generalization { get; }
        public NamedElement[] InheritedMember { get; }
        public Boolean IsAbstract { get;set; }
        public Boolean IsFinalSpecialization { get; }
        public Classifier[] RedefinedClassifier { get; }
        public Substitution[] Substitution { get; }

        TemplateSignature TemplateableElement.OwnedTemplateSignature
            {
            get { return OwnedTemplateSignature; }
            }

        public TemplateBinding[] TemplateBinding { get; }
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