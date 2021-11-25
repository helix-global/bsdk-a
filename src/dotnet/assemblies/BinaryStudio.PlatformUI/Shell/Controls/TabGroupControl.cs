using System;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
{
    [TemplatePart(Name = "PART_TabPanel", Type = typeof(ReorderTabPanel)), TemplatePart(Name = "PART_ContentPanel", Type = typeof(FrameworkElement))]
    public class TabGroupControl : GroupControl
        {
        private static ResourceKey tabItemStyleKey;

        internal ViewHeader Header
            {
            get
                {
                return GetTemplateChild("PART_Header") as ViewHeader;
                }
            }

        public static ResourceKey TabItemStyleKey
            {
            get
                {
                ResourceKey arg_14_0;
                if ((arg_14_0 = tabItemStyleKey) == null)
                    {
                    arg_14_0 = (tabItemStyleKey = new StyleKey<TabGroupControl>());
                    }
                return arg_14_0;
                }
            }

        static TabGroupControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabGroupControl), new FrameworkPropertyMetadata(typeof(TabGroupControl)));
            }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item)
            {
            base.PrepareContainerForItemOverride(element, item);
            var view = item as View;
            if (view != null)
                {
                AutomationProperties.SetAutomationId(element, "TAB_" + view.Name);
                }
            var tabItem = (TabItem)element;
            tabItem.SetResourceReference(StyleProperty, TabItemStyleKey);
            }

        /// <summary>Called to update the current selection when items change.</summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ItemContainerGenerator.ItemsChanged" /> event.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
            {
            base.OnItemsChanged(e);
            var stringBuilder = new StringBuilder("TabGroup");
            foreach (var current in Items)
                {
                stringBuilder.Append("|");
                var view = current as View;
                if (view != null)
                    {
                    stringBuilder.Append(view.Name);
                    }
                else
                    {
                    stringBuilder.Append("Unknown");
                    }
                }
            AutomationProperties.SetAutomationId(this, stringBuilder.ToString());
            }

        /// <summary>Provides an appropriate <see cref="T:System.Windows.Automation.Peers.TabControlAutomationPeer" /> implementation for this control, as part of the WPF automation infrastructure.</summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new TabGroupContainerAutomationPeer(this);
            }
        }
    }
