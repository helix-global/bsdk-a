using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using BinaryStudio.PlatformUI.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class SplitterMeasureData {
        public SplitterLength AttachedLength{get;set;}
        public UIElement Source {get;}
        public Boolean IsMaximumReached {get;set;}
        public Boolean IsMinimumReached {get;set;}
        public Rect MeasuredBounds {get;set;}
        public SplitterMeasureData(UIElement source) {
            Source = source;
            AttachedLength = SplitterPanel.GetSplitterLength(Source);
            }

        public static IList<SplitterMeasureData> FromElements(IList elements) {
            var list = new List<SplitterMeasureData>(elements.Count);
            foreach (UIElement uIElement in elements) {
                if (uIElement != null) {
                    list.Add(new SplitterMeasureData(uIElement));
                    }
                }
            return list;
            }
        }
    }