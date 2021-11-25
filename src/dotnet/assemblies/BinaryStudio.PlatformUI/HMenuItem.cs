using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI
    {
    public class HMenuItem : MenuItem, INotifyPropertyChanged
        {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        protected override DependencyObject GetContainerForItemOverride()
            {
            var r = new HMenuItem();
            return r;
            }
        }
    }