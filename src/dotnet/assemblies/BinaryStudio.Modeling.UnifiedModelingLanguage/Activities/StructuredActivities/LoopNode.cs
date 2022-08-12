using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface LoopNode : StructuredActivityNode
        {
        ExecutableNode[] BodyPart { get; }
        OutputPin Decider { get; }
        Boolean IsTestedFirst { get; }
        ExecutableNode[] SetupPart { get; }
        ExecutableNode[] Test { get; }
        }
    }