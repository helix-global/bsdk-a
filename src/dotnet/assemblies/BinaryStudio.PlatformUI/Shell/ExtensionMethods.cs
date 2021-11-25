using System;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Shell
    {
    public static class ExtensionMethods
        {
        public static TAncestorType FindAncestor<TAncestorType>(this ViewElement obj) where TAncestorType : ViewElement
            {
            return obj.FindAncestor<TAncestorType, ViewElement>(e => (ViewElement)e.Parent);
            }

        public static TAncestorType FindAncestorOrSelf<TAncestorType>(this ViewElement obj) where TAncestorType : ViewElement
            {
            var ancestorType = obj as TAncestorType;
            if (ancestorType != null)
                return ancestorType;
            return obj.FindAncestor<TAncestorType>();
            }

        internal static Boolean IsAncestorOf(this ViewElement viewElement, ViewElement otherElement)
            {
            return viewElement.IsAncestorOf(otherElement, e => (ViewElement)e.Parent);
            }

        internal static Boolean IsPinned(this ViewElement element)
            {
            var view = element as View;
            if (view != null)
                return view.IsPinned;
            return false;
            }

        internal static ViewBookmarkType GetBookmarkType(this ViewElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var viewBookmarkType = ViewBookmarkType.Default;
            if (element is DocumentGroup || element is DocumentGroupContainer || element.Parent is DocumentGroup)
                viewBookmarkType = ViewBookmarkType.DocumentWell;
            else if (ViewElement.FindRootElement(element) is FloatSite)
                viewBookmarkType = ViewBookmarkType.Raft;
            return viewBookmarkType;
            }

        internal static void InsertNewParent(this ViewElement oldView, ViewGroup parent)
            {
            var parent1 = oldView.Parent;
            if (parent1 == null)
                {
                parent.Children.Add(oldView);
                }
            else
                {
                using (parent1.DeferRebuildVisibleChildren())
                    {
                    var index = parent1.Children.IndexOf(oldView);
                    parent.IsVisible = parent.IsVisible || oldView.IsVisible;
                    parent1.Children[index] = parent;
                    parent.Children.Add(oldView);
                    }
                }
            }

        internal static String GetAutomationPeerCaption(this ViewElement viewElement)
            {
            var str = String.Empty;
            var view1 = viewElement as View;
            if (view1 != null)
                {
                if (view1.Title != null)
                    str = view1.Title.ToString();
                }
            else
                {
                var tabGroup = viewElement as TabGroup;
                if (tabGroup != null)
                    {
                    var view2 = tabGroup.SelectedElement as View;
                    if (view2 == null && tabGroup.VisibleChildren.Count > 0)
                        view2 = tabGroup.VisibleChildren[0] as View;
                    if (view2 != null && view2.Title != null)
                        str = view2.Title.ToString();
                    }
                else
                    {
                    var dockGroup = viewElement as DockGroup;
                    if (dockGroup != null)
                        {
                        if (dockGroup.VisibleChildren.Count == 1)
                            str = dockGroup.VisibleChildren[0].GetAutomationPeerCaption();
                        }
                    else if (viewElement is DockRoot)
                        str = DockSiteAdornerAutomationPeer.MainAutomationName;
                    }
                }
            return str ?? String.Empty;
            }

        public static void SetDeviceFloatingLeft(this ViewElement element, Double deviceFloatingLeft)
            {
            element.FloatingLeft = deviceFloatingLeft * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
            }

        public static Double GetDeviceFloatingLeft(this ViewElement element)
            {
            return element.FloatingLeft * DpiHelper.LogicalToDeviceUnitsScalingFactorX;
            }

        public static void SetDeviceFloatingTop(this ViewElement element, Double deviceFloatingTop)
            {
            element.FloatingTop = deviceFloatingTop * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
            }

        public static Double GetDeviceFloatingTop(this ViewElement element)
            {
            return element.FloatingTop * DpiHelper.LogicalToDeviceUnitsScalingFactorY;
            }

        public static void SetDeviceFloatingWidth(this ViewElement element, Double deviceFloatingWidth)
            {
            element.FloatingWidth = deviceFloatingWidth * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
            }

        public static Double GetDeviceFloatingWidth(this ViewElement element)
            {
            return element.FloatingWidth * DpiHelper.LogicalToDeviceUnitsScalingFactorX;
            }

        public static void SetDeviceFloatingHeight(this ViewElement element, Double deviceFloatingHeight)
            {
            element.FloatingHeight = deviceFloatingHeight * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
            }

        public static Double GetDeviceFloatingHeight(this ViewElement element)
            {
            return element.FloatingHeight * DpiHelper.LogicalToDeviceUnitsScalingFactorY;
            }
        }
    }