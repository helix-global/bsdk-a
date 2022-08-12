using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EDependency : ENamedElement, Dependency
        {
        public EDependency(String identifer)
            : base(identifer)
            {
            }

        public Element[] RelatedElement { get; }
        public Element[] Source { get; }
        public Element[] Target { get; }
        public TemplateParameter OwningTemplateParameter { get; }
        public TemplateParameter TemplateParameter { get; }
        #region P:Visibility:VisibilityKind
        public VisibilityKind Visibility { get; set; }
        VisibilityKind? NamedElement.Visibility
            {
            get { return Visibility; }
            set
                {
                if (value == null) { throw new ArgumentOutOfRangeException(nameof(value)); }
                Visibility = (VisibilityKind)value;
                }
            }
        #endregion
        public NamedElement[] Client { get; }
        public NamedElement[] Supplier { get; }
        }
    }