using System;
using System.Windows;

namespace BinaryStudio.PlatformUI
{
    public interface IResizable
  {
    Size MinSize { get; }

    Size MaxSize { get; }

    Rect CurrentScreenBounds { get; }

    Rect CurrentBounds { get; }

    void UpdateBounds(Double leftDelta, Double topDelta, Double widthDelta, Double heightDelta);
  }
}