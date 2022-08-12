using System.ComponentModel;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface PackageableElement : NamedElement
        {
        /// <summary>
        /// A <see cref="PackageableElement"/> must have a visibility specified
        /// if it is owned by a <see cref="Namespace"/>. The default visibility
        /// is <see cref="VisibilityKind.Public"/>.
        /// </summary>
        [DefaultValue(VisibilityKind.Public)]
        new VisibilityKind Visibility { get;set; }
        }
    }