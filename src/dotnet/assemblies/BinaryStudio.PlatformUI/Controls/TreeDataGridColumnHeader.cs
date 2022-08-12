using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridColumnHeader : Control
        {
        static TreeDataGridColumnHeader()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridColumnHeader), new FrameworkPropertyMetadata(typeof(TreeDataGridColumnHeader)));
            }

        #region P:Column:TreeDataGridColumn
        private static readonly DependencyPropertyKey ColumnPropertyKey = DependencyProperty.RegisterReadOnly("Column", typeof(TreeDataGridColumn), typeof(TreeDataGridColumnHeader), new PropertyMetadata(default(TreeDataGridColumn)));
        public static readonly DependencyProperty ColumnProperty = ColumnPropertyKey.DependencyProperty;
        public TreeDataGridColumn Column
            {
            get { return (TreeDataGridColumn)GetValue(ColumnProperty); }
            internal set { SetValue(ColumnPropertyKey, value); }
            }
        #endregion

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            InstallHandlers();
            }

        private void InstallHandlers(Thumb source) {
            if (source != null) {
                source.DragStarted   += OnDragStarted;
                source.DragDelta     += OnDragDelta;
                source.DragCompleted += OnDragCompleted;
                }
            }

        private void OnDragCompleted(Object sender, DragCompletedEventArgs e)
            {
            IsDragging = false;
            }

        private void OnDragDelta(Object sender, DragDeltaEventArgs e) {
            if (IsDragging) {
                var column = Column;
                if (column != null) {
                    if (column.Width.IsStar)
                        {

                        }
                    else
                        {
                        var value = Math.Max(column.MinWidth,Math.Min(column.MaxWidth, column.ActualWidth + e.HorizontalChange));
                        column.Width = value;
                        }
                    }
                }
            }

        private void OnDragStarted(Object sender, DragStartedEventArgs e)
            {
            IsDragging = true;
            }

        private void UninstallHandlers(Thumb source) {
            if (source != null) {
                source.DragStarted   -= OnDragStarted;
                source.DragDelta     -= OnDragDelta;
                source.DragCompleted -= OnDragCompleted;
                }
            }

        private void InstallHandlers()
            {
            _leftGripper  = (GetTemplateChild("PART_LeftHeaderGripper")  as Thumb);
            _rightGripper = (GetTemplateChild("PART_RightHeaderGripper") as Thumb);
            UninstallHandlers(_leftGripper);
            UninstallHandlers(_rightGripper);
            InstallHandlers(_leftGripper);
            InstallHandlers(_rightGripper);
            }

        private Thumb _leftGripper;
        private Thumb _rightGripper;
        private Boolean IsDragging;
        }
    }