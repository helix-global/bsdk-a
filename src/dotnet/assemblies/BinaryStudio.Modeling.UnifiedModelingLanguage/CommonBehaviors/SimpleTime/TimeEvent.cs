using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface TimeEvent : Event
        {
        Boolean IsRelative { get; }
        TimeExpression When { get; }
        }
    }