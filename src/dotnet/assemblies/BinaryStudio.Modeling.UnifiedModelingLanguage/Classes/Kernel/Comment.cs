using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Comment : Element
        {
        Element[] AnnotatedElement { get; }
        String Body { get; }
        }
    }