namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Deployment : Dependency
        {
        DeploymentSpecification[] Configuration { get; }
        }
    }