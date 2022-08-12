
using System;
using System.Xml;
using BinaryStudio.Modeling.UnifiedModelingLanguage;
using shell;

internal class EMXDocumentSource : DocumentSource
    {
    public override Object Source { get; }
    public EMXDocumentSource(XmlDocument source)
        : base(source, DesiredDocumentType.EMX)
        {
        Source = (new Module()).LoadModel(source);
        }
    }
