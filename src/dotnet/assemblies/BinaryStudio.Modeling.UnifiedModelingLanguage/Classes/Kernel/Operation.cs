using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    using Integer = System.Int64;
    public partial interface Operation : BehavioralFeature
        {
        Constraint BodyCondition { get; }
        Class Class { get; }
        DataType Datatype { get; }
        Boolean IsOrdered { get; }
        Boolean IsQuery { get; }
        Boolean IsUnique { get; }
        Integer Lower { get; }
        new Parameter[] OwnedParameter { get; }
        Constraint[] Postcondition { get; }
        Constraint[] Precondition { get; }
        new Type[] RaisedException { get; }
        Operation[] RedefinedOperation { get; }
        Type Type { get; }
        UnlimitedNatural Upper { get; }
        }
    }