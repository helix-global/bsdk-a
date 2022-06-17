using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface ICommonBorderPaddingAndBackgroundProperties
        {
        String BackgroundAttachment {get;set;}
        String BackgroundColor {get;set;}
        String BackgroundImage {get;set;}
        String BackgroundPositionHorizontal {get;set;}
        String BackgroundPositionVertical {get;set;}
        String BackgroundRepeat {get;set;}
        String BorderAfterColor {get;set;}
        String BorderAfterStyle {get;set;}
        String BorderAfterWidth {get;set;}
        String BorderBeforeColor {get;set;}
        String BorderBeforeStyle {get;set;}
        String BorderBeforeWidth {get;set;}
        String BorderBottomColor {get;set;}
        String BorderBottomStyle {get;set;}
        String BorderBottomWidth {get;set;}
        String BorderEndColor {get;set;}
        String BorderEndStyle {get;set;}
        String BorderEndWidth {get;set;}
        String BorderLeftColor {get;set;}
        String BorderLeftStyle {get;set;}
        String BorderLeftWidth {get;set;}
        String BorderRightColor {get;set;}
        String BorderRightStyle {get;set;}
        String BorderRightWidth {get;set;}
        String BorderStartColor {get;set;}
        String BorderStartStyle {get;set;}
        String BorderStartWidth {get;set;}
        String BorderTopColor {get;set;}
        String BorderTopStyle {get;set;}
        String BorderTopWidth {get;set;}
        String PaddingAfter {get;set;}
        String PaddingBefore {get;set;}
        String PaddingBottom {get;set;}
        String PaddingEnd {get;set;}
        String PaddingLeft {get;set;}
        String PaddingRight {get;set;}
        String PaddingStart {get;set;}
        String PaddingTop {get;set;}
        }
    }
