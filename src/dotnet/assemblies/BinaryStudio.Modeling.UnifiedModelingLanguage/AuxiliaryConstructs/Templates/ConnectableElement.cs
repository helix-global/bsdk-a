﻿namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ConnectableElement : ParameterableElement
        {
        ConnectableElementTemplateParameter TemplateParameter { get; }
        }
    }