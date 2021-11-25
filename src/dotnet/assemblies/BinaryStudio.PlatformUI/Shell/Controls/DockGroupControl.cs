using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BinaryStudio.PlatformUI.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockGroupControl : SplitterItemsControl
        {
        private class EffectiveMinimumSizeConverter : MultiValueConverter<Double, Double, Boolean, Double>
            {
            protected override Double Convert(Double contentSize, Double splitterSize, Boolean isLast, Object parameter, CultureInfo culture) {
                var r = contentSize;
                if (!isLast) { r += splitterSize; }
                return r;
                }
            }

        static DockGroupControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockGroupControl), new FrameworkPropertyMetadata(typeof(DockGroupControl)));
            }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            SetBindings(element, item);
            }

        protected override void OnOrientationChanged() {
            base.OnOrientationChanged();
            foreach (var item in Items) {
                var container = ItemContainerGenerator.ContainerFromItem(item);
                if (container != null) {
                    SetBindings(container, item);
                    }
                }
            }

        private void SetBindings(DependencyObject element, Object item) {
            var d = item as DependencyObject;
            if (d != null) {
                ViewManager.BindViewManager(d, element);
                }
            if (Orientation == Orientation.Horizontal) {
                BindingOperations.SetBinding(element, SplitterPanel.SplitterLengthProperty, new Binding {
                    Source = item,
                    Path = new PropertyPath(ViewElement.DockedWidthProperty),
                    Mode = BindingMode.TwoWay
                    });
                BindingOperations.SetBinding(element, SplitterPanel.MinimumLengthProperty, new MultiBinding {
                    Converter = new EffectiveMinimumSizeConverter(),
                    Bindings = {
                        new Binding {
                            Source = item,
                            Path = new PropertyPath(nameof(ViewElement.MinimumWidth)),
                            Mode = BindingMode.OneWay
                            },
                        new Binding {
                            Source = element,
                            Path = new PropertyPath(SplitterGripSizeProperty),
                            Mode = BindingMode.OneWay
                            },
                        new Binding {
                            Source = element,
                            Path = new PropertyPath(SplitterPanel.IsLastProperty),
                            Mode = BindingMode.OneWay
                            }
                        }
                    });
                return;
                }
            BindingOperations.SetBinding(element, SplitterPanel.SplitterLengthProperty, new Binding {
                Source = item,
                Path = new PropertyPath(nameof(ViewElement.DockedHeight)),
                Mode = BindingMode.TwoWay
                });
            BindingOperations.SetBinding(element, SplitterPanel.MinimumLengthProperty, new MultiBinding {
                Converter = new EffectiveMinimumSizeConverter(),
                Bindings = {
                    new Binding {
                        Source = item,
                        Path = new PropertyPath(nameof(ViewElement.MinimumHeight)),
                        Mode = BindingMode.OneWay
                        },
                    new Binding {
                        Source = element,
                        Path = new PropertyPath(SplitterGripSizeProperty),
                        Mode = BindingMode.OneWay
                        },
                    new Binding {
                        Source = element,
                        Path = new PropertyPath(SplitterPanel.IsLastProperty),
                        Mode = BindingMode.OneWay
                        }
                    }
                });
            }
        }
    }
