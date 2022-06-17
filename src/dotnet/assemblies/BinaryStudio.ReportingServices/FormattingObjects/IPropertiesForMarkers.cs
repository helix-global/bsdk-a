using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IPropertiesForMarkers
        {
        String MarkerClassName {get;set;}
        String RetrieveBoundary {get;set;}
        String RetrieveClassName {get;set;}
        String RetrievePosition {get;set;}
        }
    }
