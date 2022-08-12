using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.PlatformUI.Views;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Controls
    {
    public class CertificateStoreManagementControl : Control
        {
        static CertificateStoreManagementControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CertificateStoreManagementControl), new FrameworkPropertyMetadata(typeof(CertificateStoreManagementControl)));
            }

        /// <summary>When overridden in a derived class, is invoked whenever
        /// application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            FrameHost = FrameHost ?? GetTemplateChild("FrameHost") as Frame;
            if (ItemsHost == null) {
                ItemsHost = GetTemplateChild("ItemsHost") as TreeView;
                if (ItemsHost != null) {
                    ItemsHost.SelectedItemChanged += OnSelectedItemChanged;
                    Refresh();
                    }
                }
            }

        private void OnSelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e) {
            if (FrameHost != null) {
                FrameHost.Navigate(e.NewValue);
                }
            }

        private void Refresh() {
            if (ItemsHost != null) {
                ItemsHost.ItemsSource = X509CertificateStorage.SystemStoreLocations.
                    Where(i => (i != X509StoreLocation.CurrentService) && (i != X509StoreLocation.Services) && (i != X509StoreLocation.Users)).
                    Select(i => new ECertificateStoreLocationPackage(i)).
                    ToArray();
                }
            }

        private TreeView ItemsHost;
        private Frame FrameHost;
        }
    }
