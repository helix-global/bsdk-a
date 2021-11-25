using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
{
    public class AutoHideChannelContextMenuEventArgs : RoutedEventArgs
  {
    public Point ChannelPoint { get; }

    public AutoHideChannelContextMenuEventArgs(RoutedEvent evt, Point channelPoint)
      : base(evt)
    {
      ChannelPoint = channelPoint;
    }
  }
}