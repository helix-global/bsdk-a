using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    [TemplatePart(Name = "PART_TabPanel", Type = typeof(ReorderTabPanel))]
    [TemplatePart(Name = "PART_ContentPanel", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_TabPopupButton", Type = typeof(GlyphDropDownButton))]
    public class DocumentGroupControl : GroupControl
        {
        private UInt32 _lastAccessOrder = 1;
        private static Object _itemTemplateKey = "DocumentGroupControlItemTemplate";
        private static readonly DependencyPropertyKey AccessOrderPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AccessOrder", typeof(UInt32), typeof(DocumentGroupControl), new PropertyMetadata(Boxes.UInt32Zero));
        public static readonly DependencyProperty AccessOrderProperty = AccessOrderPropertyKey.DependencyProperty;
        private static ResourceKey tabItemStyleKey;

        public DocumentGroupControl()
            {
            return;
            }

        public static ResourceKey TabItemStyleKey
            {
            get
                {
                return tabItemStyleKey ?? (tabItemStyleKey = new StyleKey<DocumentGroupControl>());
                }
            }

        public static Object ItemTemplateKey
            {
            get
                {
                return _itemTemplateKey;
                }
            set
                {
                _itemTemplateKey = value;
                }
            }

        static DocumentGroupControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentGroupControl), new FrameworkPropertyMetadata(typeof(DocumentGroupControl)));
            }

        public static UInt32 GetAccessOrder(TabItem tab)
            {
            Validate.IsNotNull(tab, "tab");
            return (UInt32)tab.GetValue(AccessOrderProperty);
            }

        private static void SetAccessOrder(TabItem tab, UInt32 value)
            {
            Validate.IsNotNull(tab, "tab");
            tab.SetValue(AccessOrderPropertyKey, value);
            }

        public void TouchAccessOrder(TabItem tab)
            {
            var tab1 = tab;
            var num1 = _lastAccessOrder + 1U;
            _lastAccessOrder = num1;
            var num2 = (Int32)num1;
            SetAccessOrder(tab1, (UInt32)num2);
            }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            var tab = (TabItem)element;
            tab.SetResourceReference(StyleProperty, TabItemStyleKey);
            SetAccessOrder(tab, _lastAccessOrder - 1U);
            }

        /// <summary>Creates or identifies the element used to display the specified item.</summary>
        /// <returns>The element used to display the specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
            {
            return new DocumentTabItem();
            }

        /// <summary>Provides an appropriate <see cref="T:System.Windows.Automation.Peers.TabControlAutomationPeer" /> implementation for this control, as part of the WPF automation infrastructure.</summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new DocumentGroupControlAutomationPeer(this);
            }

        public void ShowTabListPopup()
            {
            var templateChild = GetTemplateChild("PART_TabPopupButton") as GlyphDropDownButton;
            if (templateChild == null)
                return;
            templateChild.ShowDropDown();
            }
        }
    }
