using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EProperty : EStructuralFeature, Property
        {
        public EProperty(String identifer)
            : base(identifer)
            {
            }

        public PackageableElement[] DeployedElement { get; }
        public Deployment[] Deployment { get; }
        public TemplateParameter OwningTemplateParameter { get; }
        public ConnectableElementTemplateParameter TemplateParameter { get; }
        public ConnectorEnd[] End { get; }

        TemplateParameter ParameterableElement.TemplateParameter
            {
            get { return TemplateParameter; }
            }

        public AggregationKind Aggregation { get; }
        public Association Association { get; }
        public Class Class { get; }
        public DataType Datatype { get; }
        public String Default { get; }
        public ValueSpecification DefaultValue { get; }
        public Boolean IsComposite { get; }
        public Boolean IsDerived { get; }
        public Boolean IsDerivedUnion { get; }
        public Boolean IsID { get; }
        public Property Opposite { get; }
        public Association OwningAssociation { get; }
        public Property[] RedefinedProperty { get; }
        public Property[] SubsettedProperty { get; }
        public Interface Interface { get; }
        public Property AssociationEnd { get; }
        public Property[] Qualifier { get; }
        }
    }