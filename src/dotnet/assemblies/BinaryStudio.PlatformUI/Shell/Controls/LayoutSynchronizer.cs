using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI
    {
    public static class LayoutSynchronizer
        {
        private class LayoutSynchronizationScope : DisposableObject {
            public LayoutSynchronizationScope() {
                @ref++;
                }

            protected override void DisposeManagedResources() {
                if (@ref > 0) {
                    if (--@ref == 0) {
                        Synchronize();
                        }
                    }
                }
            }

        #region M:Synchronize
        private static void Synchronize() {
            if (!IsUpdatingLayout) {
                IsUpdatingLayout = true;
                try
                    {
                    foreach (var e in ElementsToUpdate.
                        Select(i => i.RootVisual as UIElement).
                        Where(i => i != null)) {
                        e.UpdateLayout();
                        }
                    ElementsToUpdate.Clear();
                    }
                finally
                    {
                    IsUpdatingLayout = false;
                    }
                }
            }
        #endregion
        #region M:Update(Visual)
        public static void Update(Visual element) {
            if (IsSynchronizing && !IsUpdatingLayout) {
                var source = PresentationSource.FromVisual(element);
                if (source != null) {
                    ElementsToUpdate.Add(source);
                    }
                }
            }
        #endregion
        #region M:BeginLayoutSynchronization:IDisposable
        public static IDisposable BeginLayoutSynchronization() {
            return new LayoutSynchronizationScope();
            }
        #endregion

        private static Int32 @ref = 0;
        private static Boolean IsUpdatingLayout { get;set; }
        private static HashSet<PresentationSource> ElementsToUpdate {get;set;}
        #region P:IsSynchronizing:Boolean
        private static Boolean IsSynchronizing { get {
            return @ref > 0;
            }}
        #endregion

        static LayoutSynchronizer() {
            ElementsToUpdate = new HashSet<PresentationSource>();
            }
        }
    }