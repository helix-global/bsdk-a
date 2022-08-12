namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Deployment : Dependency
        {
        DeployedArtifact[] DeployedArtifact { get; }
        DeploymentTarget Location { get; }
        }
    }