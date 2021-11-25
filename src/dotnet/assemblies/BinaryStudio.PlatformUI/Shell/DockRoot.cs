using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable]
    public class DockRoot : ViewGroup
        {
        public override Boolean IsChildAllowed(ViewElement e)
            {
            return e is DockGroup;
            }

        public static DockRoot Create()
            {
            return ViewElementFactory.Current.CreateDockRoot();
            }

        protected override void TryCollapseCore()
            {
            if (Children.Count != 0)
                return;
            Detach();
            }
        }
    }