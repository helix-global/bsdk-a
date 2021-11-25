using System;
using System.Windows;
using System.Windows.Controls;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Serialization;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable]
    public class AutoHideChannel : ViewGroup
        {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AutoHideChannel));

        public Dock Dock
            {
            get { return (Dock)GetValue(DockPanel.DockProperty); }
            set { SetValue(DockPanel.DockProperty, value); }
            }

        public Orientation Orientation
            {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
            }

        public override Boolean IsChildAllowed(ViewElement e)
            {
            return e is AutoHideGroup;
            }

        public static Boolean IsAutoHidden(ViewElement element)
            {
            return element.FindAncestor<AutoHideChannel, ViewElement>(e =>
            {
                if (e == null)
                    return (ViewElement)null;
                return (ViewElement)e.Parent;
            }) != null;
            }

        public static AutoHideChannel Create()
            {
            return ViewElementFactory.Current.CreateAutoHideChannel();
            }

        public override ICustomXmlSerializer CreateSerializer()
            {
            return new AutoHideChannelCustomSerializer(this);
            }
        }
    }