using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class AutoHideGroupControl : LayoutSynchronizedItemsControl
        {
        static AutoHideGroupControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideGroupControl), new FrameworkPropertyMetadata(typeof(AutoHideGroupControl)));
            }

        /// <summary>Determines if the specified item is (or is eligible to be) its own container.</summary>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        /// <param name="item">The item to check.</param>
        protected override Boolean IsItemItsOwnContainerOverride(Object item) {
            return item is AutoHideTabItem;
            }

        /// <summary>Creates or identifies the element that is used to display the given item.</summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride() {
            return new AutoHideTabItem();
            }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            AutoHideChannelControl.SetOrientation(element, AutoHideChannelControl.GetOrientation(this));
            }
        }
    }
