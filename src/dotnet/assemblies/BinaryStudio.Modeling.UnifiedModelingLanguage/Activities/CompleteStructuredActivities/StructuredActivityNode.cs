using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface StructuredActivityNode : Action, ActivityGroup
        {
        ActivityEdge[] Edge { get; }
        Boolean MustIsolate { get; }
        InputPin[] StructuredNodeInput { get; }
        OutputPin[] StructuredNodeOutput { get; }
        }
    }