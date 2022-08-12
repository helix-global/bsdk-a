using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ActivityPartition : ActivityGroup
        {
        ActivityEdge[] Edge { get; }
        Boolean IsDimension { get; }
        Boolean IsExternal { get; }
        ActivityNode[] Node { get; }
        Element Represents { get; }
        ActivityPartition[] Subpartition { get; }
        ActivityPartition SuperPartition { get; }
        }
    }