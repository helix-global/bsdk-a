using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface NamedElement : Element
        {
        String Name { get;set; }
        Namespace Namespace { get; }
        String QualifiedName { get; }
        /// <summary>
        /// Determines whether and how the <see cref="NamedElement"/> is visible outside its owning <see cref="Namespace"/>.
        /// </summary>
        VisibilityKind? Visibility { get;set; }
        }
    }