using System;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Shell
{
  public class FloatingWindowEventArgs : EventArgs
  {
    public FloatingWindow Window { get; }

    public FloatingWindowEventArgs(FloatingWindow window)
    {
      if (window == null)
        throw new ArgumentNullException(nameof(window));
      Window = window;
    }
  }
}