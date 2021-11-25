using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BinaryStudio.PlatformUI.Shell
    {
    internal class ImmutableObservableCollection<T> : ReadOnlyObservableCollection<T>, IObservableCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, INotifyPropertyChanged, INotifyCollectionChanged
    {
        T IObservableCollection<T>.this[Int32 index]
        {
            get
            {
                return base[index];
            }
            set
            {
                ((IList<T>)this)[index] = value;
            }
        }

        public ImmutableObservableCollection(ObservableCollection<T> observedCollection) : base(observedCollection)
        {
        }

        void IObservableCollection<T>.Clear()
        {
            ((ICollection<T>)this).Clear();
        }

        void IObservableCollection<T>.Move(Int32 oldIndex, Int32 newIndex)
        {
            throw new NotSupportedException();
        }
    }
}