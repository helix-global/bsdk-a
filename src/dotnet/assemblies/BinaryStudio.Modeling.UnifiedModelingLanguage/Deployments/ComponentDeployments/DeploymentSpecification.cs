using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DeploymentSpecification : Artifact
        {
        Deployment Deployment { get; }
        String DeploymentLocation { get; }
        String ExecutionLocation { get; }
        }
    }