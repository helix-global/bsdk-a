using BinaryStudio.PlatformUI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class XYViewportSelector : MultiSelector
        {
        static XYViewportSelector()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XYViewportSelector), new FrameworkPropertyMetadata(typeof(XYViewportSelector)));
            }

        /// <summary>Creates or identifies the element that is used to display the given item.</summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
            {
            return new XYViewportSelectorItem();
            }

        /// <summary>Prepares the specified element to display the specified item. </summary>
        /// <param name="element">The element that is used to display the specified item.</param>
        /// <param name="item">The specified item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            if (item is DependencyObject source) {
                if (element is XYViewportSelectorItem target) {
                    target.SetBinding(XYViewport.LeftProperty, source, XYViewport.LeftProperty, BindingMode.TwoWay);
                    target.SetBinding(XYViewport.RightProperty, source, XYViewport.RightProperty, BindingMode.TwoWay);
                    target.SetBinding(XYViewport.TopProperty, source, XYViewport.TopProperty, BindingMode.TwoWay);
                    target.SetBinding(XYViewport.BottomProperty, source, XYViewport.BottomProperty, BindingMode.TwoWay);
                    }
                }
            }

        /// <summary>Determines if the specified item is (or is eligible to be) its own container.</summary>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        /// <param name="item">The item to check.</param>
        protected override Boolean IsItemItsOwnContainerOverride(Object item)
            {
            return item is XYViewportSelectorItem;
            }

        internal void SelectItem(DependencyObject item) {
            if (!IsUpdatingSelectedItems) {
                BeginUpdateSelectedItems();
                SelectedItems.Add(item);
                EndUpdateSelectedItems();
                if (ItemsHost is XYViewportPanel panel) {
                    //panel.BringTop(item as UIElement);
                    }
                }
            }

        internal void UnselectItem(DependencyObject item) {
            if (!IsUpdatingSelectedItems) {
                BeginUpdateSelectedItems();
                SelectedItems.Remove(item);
                EndUpdateSelectedItems();
                }
            }

        public Panel ItemsHost { get {
            var pi = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);
            #if NET40
            return (Panel)pi.GetValue(this, null);
            #else
            return (Panel)pi.GetValue(this);
            #endif
            }}
        }
    }
