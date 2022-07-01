using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Port : Property
        {
        Boolean IsBehavior { get; }
        Boolean IsConjugated { get; }
        Boolean IsService { get; }
        Interface[] Provided { get; }
        Port[] RedefinedPort { get; }
        Interface[] Required { get; }
        }
    }