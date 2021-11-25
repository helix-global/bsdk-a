using System;
using System.Collections.Generic;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class MultiSelectionManager
        {
        private readonly Suspender _clearSuspender = new Suspender(null);
        private static MultiSelectionManager instance;
        private MultiSelectionCollection multiSelection;

        public static MultiSelectionManager Instance
            {
            get
                {
                return instance ?? (instance = new MultiSelectionManager());
                }
            }

        public ViewGroup ContainingGroup
            {
            get
                {
                if (multiSelection.Count == 0)
                    return null;
                return multiSelection[0].Parent;
                }
            }

        public IEnumerable<ViewElement> MultiSelectedElements
            {
            get
                {
                return multiSelection;
                }
            }

        public Int32 SelectedElementCount
            {
            get
                {
                return multiSelection.Count;
                }
            }

        protected MultiSelectionManager()
            {
            multiSelection = new MultiSelectionCollection();
            }

        public void Add(ViewElement element)
            {
            Validate.IsNotNull(element, "element");
            if (!((ViewElement)element.Parent is DocumentGroup))
                throw new ArgumentException("element must be a child of a DocumentGroup");
            if (multiSelection.Contains(element))
                return;
            multiSelection.Add(element);
            }

        public Boolean Remove(ViewElement element)
            {
            Validate.IsNotNull(element, "element");
            return multiSelection.Remove(element);
            }

        public Boolean Contains(ViewElement element)
            {
            return multiSelection.Contains(element);
            }

        public void Clear()
            {
            if (_clearSuspender.IsSuspended)
                return;
            multiSelection.Clear();
            }

        public IDisposable PreventClear()
            {
            return _clearSuspender.Suspend();
            }

        private class MultiSelectionCollection : OwnershipCollection<ViewElement>
            {
            protected override void LoseOwnership(ViewElement item)
                {
                DetachEvents(item);
                DocumentGroup.SetIsMultiSelected(item, false);
                if (Count != 1)
                    return;
                Clear();
                }

            protected override void TakeOwnership(ViewElement item)
                {
                foreach (var viewElement in new List<ViewElement>(this))
                    {
                    if (viewElement.Parent != item.Parent)
                        Remove(viewElement);
                    }
                AttachEvents(item);
                DocumentGroup.SetIsMultiSelected(item, true);
                }

            private void AttachEvents(ViewElement item)
                {
                item.ParentChanged += OnParentChanged;
                var view = item as View;
                if (view == null)
                    return;
                view.Hidden += OnViewHidden;
                }

            private void DetachEvents(ViewElement item)
                {
                item.ParentChanged -= OnParentChanged;
                var view = item as View;
                if (view == null)
                    return;
                view.Hidden -= OnViewHidden;
                }

            private void OnParentChanged(Object sender, EventArgs e)
                {
                var viewElement = (ViewElement)sender;
                if (!((ViewElement)viewElement.Parent is TabGroup))
                    return;
                Remove(viewElement);
                }

            private void OnViewHidden(Object sender, EventArgs e)
                {
                Remove((ViewElement)sender);
                }

            protected override void OnMaximumItemsExceeded(ViewElement item)
                {
                }
            }
        }
    }