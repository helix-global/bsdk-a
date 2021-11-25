using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell
    {
    public interface INonClientArea
        {
        Int32 HitTest(Point point);
        }
    }