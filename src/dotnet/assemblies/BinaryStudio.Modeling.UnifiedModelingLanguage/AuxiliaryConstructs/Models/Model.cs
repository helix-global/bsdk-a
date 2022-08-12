using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Model : Package
        {
        String Viewpoint { get; }
        }
    }