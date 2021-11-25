using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell
    {
    public interface IDependencyObjectCustomSerializerAccess
        {
        Boolean ShouldSerializeProperty(DependencyProperty dp);

        Object GetValue(DependencyProperty dp);
        }
    }