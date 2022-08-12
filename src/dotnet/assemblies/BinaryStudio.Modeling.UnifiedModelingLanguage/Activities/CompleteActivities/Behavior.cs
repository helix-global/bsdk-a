namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Behavior : Class
        {
        ParameterSet[] OwnedParameterSet { get; }
        }
    }