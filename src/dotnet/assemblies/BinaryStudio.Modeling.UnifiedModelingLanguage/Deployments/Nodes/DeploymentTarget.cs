namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DeploymentTarget : NamedElement
        {
        PackageableElement[] DeployedElement { get; }
        Deployment[] Deployment { get; }
        }
    }