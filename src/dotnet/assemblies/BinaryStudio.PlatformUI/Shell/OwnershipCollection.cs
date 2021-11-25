using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinaryStudio.PlatformUI.Shell
    {
    public abstract class OwnershipCollection<T> : ObservableCollection<T>, IObservableCollection<T>
        {
        internal const Int32 ItemCapacity = 20000;
        private Boolean exceededCapacity;

        protected abstract void TakeOwnership(T item);
        protected abstract void LoseOwnership(T item);
        protected abstract void OnMaximumItemsExceeded(T item);

        /// <summary>Removes all items from the collection.</summary>
        protected override void ClearItems() {
            var objList = new List<T>(this);
            base.ClearItems();
            foreach (var obj in objList)
                LoseOwnership(obj);
            }

        /// <summary>Inserts an item into the collection at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(Int32 index, T item) {
            if (item == null) { throw new InvalidOperationException("Collection item cannot be null"); }
            BeforeBaseInsertItem(index, item);
            base.InsertItem(index, item);
            AfterBaseInsertItem(index, item);
            CheckCapacity(item);
            }

        protected virtual void BeforeBaseInsertItem(Int32 index, T item) {
            }

        protected virtual void AfterBaseInsertItem(Int32 index, T item) {
            TakeOwnership(item);
            }

        /// <summary>Replaces the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(Int32 index, T item) {
            if (item == null) { throw new InvalidOperationException("Collection item cannot be null"); }
            var obj = this[index];
            base.SetItem(index, item);
            LoseOwnership(obj);
            TakeOwnership(item);
            }

        /// <summary>Removes the item at the specified index of the collection.</summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(Int32 index) {
            var obj = this[index];
            base.RemoveItem(index);
            LoseOwnership(obj);
            }

        private void CheckCapacity(T item) {
            if (exceededCapacity || Count < 20000)
                return;
            exceededCapacity = true;
            OnMaximumItemsExceeded(item);
            }

        void IObservableCollection<T>.Move(Int32 oldIndex, Int32 newIndex) {
            Move(oldIndex, newIndex);
            }
        }
    }