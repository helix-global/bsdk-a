using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class GroupControl : LayoutSynchronizedTabControl
        {
        #region P:ContentCornerRadius:CornerRadius
        public static readonly DependencyProperty ContentCornerRadiusProperty = DependencyProperty.Register("ContentCornerRadius", typeof(CornerRadius), typeof(GroupControl), new FrameworkPropertyMetadata(new CornerRadius(0.0)));
        public CornerRadius ContentCornerRadius
            {
            get { return (CornerRadius)GetValue(ContentCornerRadiusProperty); }
            set { SetValue(ContentCornerRadiusProperty, value); }
            }
        #endregion

        public GroupControl()
            {
            Loaded += (RoutedEventHandler)((param0, param1) => ClearValue(SelectedItemProperty));
            UtilityMethods.AddPresentationSourceCleanupAction(this, () =>
            {
                BindingOperations.SetBinding(this, SelectedItemProperty, new Binding
                    {
                    Mode = BindingMode.OneTime
                    });
                BindingOperations.SetBinding(this, ItemsSourceProperty, new Binding
                    {
                    Mode = BindingMode.OneTime
                    });
                DataContext = null;
            });
            }

        #region M:OnItemsChanged(NotifyCollectionChangedEventArgs)
        /// <summary>Called to update the current selection when items change.</summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ItemContainerGenerator.ItemsChanged" /> event.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
            {
            base.OnItemsChanged(e);
            OnApplyTemplate();
            }
        #endregion
        #region M:GetContainerForItemOverride:DependencyObject
        /// <summary>Creates or identifies the element used to display the specified item.</summary>
        /// <returns>The element used to display the specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
            {
            return new GroupControlTabItem();
            }
        #endregion

        internal DependencyObject GetHeaderPanel()
            {
            return GetTemplateChild("PART_TabPanel");
            }

        internal FrameworkElement GetContentPanel()
            {
            return GetTemplateChild("PART_ContentPanel") as FrameworkElement;
            }

        }
    }
