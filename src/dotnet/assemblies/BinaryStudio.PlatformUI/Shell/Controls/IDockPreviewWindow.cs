using System;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public interface IDockPreviewWindow
        {
        Int32 InsertPosition { get; }

        void Show(IntPtr owner);

        void Hide();

        void Close();

        void SetupDockPreview(SetupDockPreviewArgs args);
        }
    }