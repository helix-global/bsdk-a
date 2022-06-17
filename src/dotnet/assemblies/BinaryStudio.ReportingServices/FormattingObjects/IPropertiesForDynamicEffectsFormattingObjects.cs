using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IPropertiesForDynamicEffectsFormattingObjects
        {
        String ActiveState {get;set;}
        String AutoRestore {get;set;}
        String CaseName {get;set;}
        String CaseTitle {get;set;}
        String DestinationPlacementOffset {get;set;}
        String ExternalDestination {get;set;}
        String IndicateDestination {get;set;}
        String InternalDestination {get;set;}
        String ShowDestination {get;set;}
        String StartingState {get;set;}
        String SwitchTo {get;set;}
        String TargetPresentationContext {get;set;}
        String TargetProcessingContext {get;set;}
        String TargetStylesheet {get;set;}
        }
    }
