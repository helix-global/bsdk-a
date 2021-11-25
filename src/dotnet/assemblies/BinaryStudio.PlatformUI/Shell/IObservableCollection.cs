using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BinaryStudio.PlatformUI.Shell
    {
    public interface IObservableCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, INotifyPropertyChanged, INotifyCollectionChanged
        {
        new Int32 Count { get; }

        new T this[Int32 index] { get; set; }

        new void Clear();

        void Move(Int32 oldIndex, Int32 newIndex);
        }
    }