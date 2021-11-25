namespace BinaryStudio.PlatformUI.Shell
    {
    public interface IBookmarkResolver
        {
        ViewElement SelectBookmarkLocation(View view, WindowProfile windowProfile, ViewBookmarkType bookmarkType);

        void DockToBookmarkLocation(ViewElement bookmarkLocation, View view, WindowProfile windowProfile, ViewBookmarkType bookmarkType);
        }
    }