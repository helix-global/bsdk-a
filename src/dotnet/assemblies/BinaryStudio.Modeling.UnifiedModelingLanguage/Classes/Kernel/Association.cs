using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Association : Relationship, Classifier
        {
        Type[] EndType { get; }
        Boolean IsDerived { get; }
        Property[] MemberEnd { get; }
        Property[] NavigableOwnedEnd { get; }
        Property[] OwnedEnd { get; }
        }
    }