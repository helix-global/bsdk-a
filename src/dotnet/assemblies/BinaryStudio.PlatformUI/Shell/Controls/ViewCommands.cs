using System.Windows.Input;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public static class ViewCommands
        {
        public static readonly RoutedCommand HideViewCommand = new RoutedCommand("HideView", typeof(ViewCommands));
        public static readonly RoutedCommand AutoHideViewCommand = new RoutedCommand("AutoHideView", typeof(ViewCommands));
        public static readonly RoutedCommand ShowAutoHiddenView = new RoutedCommand("ShowAutoHiddenView", typeof(ViewCommands));
        public static readonly RoutedCommand ShowAndActivateAutoHiddenView = new RoutedCommand("ShowAndActivateAutoHiddenView", typeof(ViewCommands));
        public static readonly RoutedCommand HideAutoHiddenView = new RoutedCommand("HideAutoHiddenView", typeof(ViewCommands));
        public static readonly RoutedCommand ToggleDocked = new RoutedCommand("ToggleDocked", typeof(ViewCommands));
        public static readonly RoutedCommand NewHorizontalTabGroupCommand = new RoutedCommand("NewHorizontalTabGroup", typeof(ViewCommands));
        public static readonly RoutedCommand NewVerticalTabGroupCommand = new RoutedCommand("NewVerticalTabGroup", typeof(ViewCommands));
        public static readonly RoutedCommand MoveToNextTabGroupCommand = new RoutedCommand("MoveToNextTabGroup", typeof(ViewCommands));
        public static readonly RoutedCommand MoveToPreviousTabGroupCommand = new RoutedCommand("MoveToPreviousTabGroup", typeof(ViewCommands));
        public static readonly RoutedCommand MoveAllToNextTabGroupCommand = new RoutedCommand("MoveAllToNextTabGroup", typeof(ViewCommands));
        public static readonly RoutedCommand MoveAllToPreviousTabGroupCommand = new RoutedCommand("MoveAllToPreviousTabGroup", typeof(ViewCommands));
        public static readonly RoutedCommand ActivateDocumentViewCommand = new RoutedCommand("ActivateDocumentView", typeof(ViewCommands));
        public static readonly RoutedCommand PromoteCommand = new RoutedCommand("Promote", typeof(ViewCommands));
        public static readonly RoutedCommand MultiSelectCommand = new RoutedCommand("MultiSelect", typeof(ViewCommands));
        public static readonly RoutedCommand RightSelectCommand = new RoutedCommand("RightSelect", typeof(ViewCommands));
        public static readonly RoutedCommand CancelMultiSelectionCommand = new RoutedCommand("CancelMultiSelection", typeof(ViewCommands));
        public static readonly RoutedCommand TogglePinStatusCommand = new RoutedCommand("TogglePinStatus", typeof(ViewCommands));
        public static RoutedCommand ToggleMaximizeRestoreWindow = new RoutedCommand("ToggleMaximizeRestoreWindow", typeof(ViewCommands));
        public static RoutedCommand MinimizeWindow = new RoutedCommand("MinimizeWindow", typeof(ViewCommands));
        public static RoutedCommand CloseWindow = new RoutedCommand("CloseWindow", typeof(ViewCommands));
        }
    }