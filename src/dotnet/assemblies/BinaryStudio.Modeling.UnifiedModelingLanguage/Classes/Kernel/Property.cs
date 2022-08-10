using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Property : StructuralFeature
        {
        AggregationKind Aggregation { get; }
        Association Association { get; }
        Class Class { get; }
        DataType Datatype { get; }
        String Default { get; }
        ValueSpecification DefaultValue { get; }
        Boolean IsComposite { get; }
        Boolean IsDerived { get; }
        Boolean IsDerivedUnion { get; }
        Boolean IsID { get; }
        new Boolean IsReadOnly { get; }
        Property Opposite { get; }
        Association OwningAssociation { get; }
        Property[] RedefinedProperty { get; }
        Property[] SubsettedProperty { get; }
        }
    }