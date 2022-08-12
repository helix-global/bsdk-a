using System.Collections.Generic;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Element
        {
        /// <summary>
        /// The Comments owned by this <see cref="Element"/>.
        /// {subsets <see cref="OwnedElement"/>}
        /// </summary>
        IList<Comment> OwnedComment { get; }
        /// <summary>
        /// The Elements owned by this <see cref="Element"/>.
        /// </summary>
        IList<Element> OwnedElement { get; }
        Element Owner { get; }
        }
    }