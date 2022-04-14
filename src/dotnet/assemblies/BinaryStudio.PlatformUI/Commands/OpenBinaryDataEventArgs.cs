using System;
using System.Windows;

namespace BinaryStudio.PlatformUI
    {
    public class OpenBinaryDataEventArgs : RoutedEventArgs
        {
        public Byte[] Data { get;set; }
        public OpenBinaryDataEventArgs(RoutedEvent routedEvent)
            :base(routedEvent)
            {
            }
        }
    }