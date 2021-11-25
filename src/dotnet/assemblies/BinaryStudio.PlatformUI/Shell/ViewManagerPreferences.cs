using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class ViewManagerPreferences : DependencyObject
        {
        public static readonly DependencyProperty DocumentDockPreferenceProperty = DependencyProperty.Register("DocumentDockPreference", typeof(DockPreference), typeof(ViewManagerPreferences), new PropertyMetadata(DockPreference.DockAtBeginning));
        public static readonly DependencyProperty TabDockPreferenceProperty = DependencyProperty.Register("TabDockPreference", typeof(DockPreference), typeof(ViewManagerPreferences), new PropertyMetadata(DockPreference.DockAtBeginning));
        public static readonly DependencyProperty AllowDocumentTabAutoDockingProperty = DependencyProperty.Register("AllowDocumentTabAutoDocking", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty AllowTabGroupTabAutoDockingProperty = DependencyProperty.Register("AllowTabGroupTabAutoDocking", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty AutoHideHoverDelayProperty = DependencyProperty.Register("AutoHideHoverDelay", typeof(TimeSpan), typeof(ViewManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(SystemParameters.MenuShowDelay)));
        public static readonly DependencyProperty AutoHideMouseExitGracePeriodProperty = DependencyProperty.Register("AutoHideMouseExitGracePeriod", typeof(TimeSpan), typeof(ViewManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(500.0)));
        public static readonly DependencyProperty HideOnlyActiveViewProperty = DependencyProperty.Register("HideOnlyActiveView", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanTrue));
        public static readonly DependencyProperty AutoHideOnlyActiveViewProperty = DependencyProperty.Register("AutoHideOnlyActiveView", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty EnableIndependentFloatingDocumentGroupsProperty = DependencyProperty.Register("EnableIndependentFloatingDocumentGroups", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanTrue, OnEnableIndependentFloatingDocumentGroupsChanged));
        public static readonly DependencyProperty EnableIndependentFloatingToolwindowsProperty = DependencyProperty.Register("EnableIndependentFloatingToolwindows", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanTrue, OnEnableIndependentFloatingToolwindowsChanged, CoerceEnableIndependentFloatingToolwindows));
        public static readonly DependencyProperty MaintainPinStatusProperty = DependencyProperty.Register("MaintainPinStatus", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty IsPinnedTabPanelSeparateProperty = DependencyProperty.Register("IsPinnedTabPanelSeparate", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty ShowPinButtonInUnpinnedTabsProperty = DependencyProperty.Register("ShowPinButtonInUnpinnedTabs", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanTrue));
        public static readonly DependencyProperty AutoZOrderDelayProperty = DependencyProperty.Register("AutoZOrderDelay", typeof(TimeSpan), typeof(ViewManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(500.0)));
        public static readonly DependencyProperty ShowAutoHiddenWindowsOnHoverProperty = DependencyProperty.Register("ShowAutoHiddenWindowsOnHover", typeof(Boolean), typeof(ViewManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));

        public TimeSpan AutoHideMouseExitGracePeriod
            {
            get
                {
                return (TimeSpan)GetValue(AutoHideMouseExitGracePeriodProperty);
                }
            set
                {
                SetValue(AutoHideMouseExitGracePeriodProperty, value);
                }
            }

        public DockPreference DocumentDockPreference
            {
            get
                {
                return (DockPreference)GetValue(DocumentDockPreferenceProperty);
                }
            set
                {
                SetValue(DocumentDockPreferenceProperty, value);
                }
            }

        public DockPreference TabDockPreference
            {
            get
                {
                return (DockPreference)GetValue(TabDockPreferenceProperty);
                }
            set
                {
                SetValue(TabDockPreferenceProperty, value);
                }
            }

        public TimeSpan AutoHideHoverDelay
            {
            get
                {
                return (TimeSpan)GetValue(AutoHideHoverDelayProperty);
                }
            set
                {
                SetValue(AutoHideHoverDelayProperty, value);
                }
            }

        public Boolean AllowDocumentTabAutoDocking
            {
            get
                {
                return (Boolean)GetValue(AllowDocumentTabAutoDockingProperty);
                }
            set
                {
                SetValue(AllowDocumentTabAutoDockingProperty, Boxes.Box(value));
                }
            }

        public Boolean AllowTabGroupTabAutoDocking
            {
            get
                {
                return (Boolean)GetValue(AllowTabGroupTabAutoDockingProperty);
                }
            set
                {
                SetValue(AllowTabGroupTabAutoDockingProperty, Boxes.Box(value));
                }
            }

        public Boolean HideOnlyActiveView
            {
            get
                {
                return (Boolean)GetValue(HideOnlyActiveViewProperty);
                }
            set
                {
                SetValue(HideOnlyActiveViewProperty, Boxes.Box(value));
                }
            }

        public Boolean AutoHideOnlyActiveView
            {
            get
                {
                return (Boolean)GetValue(AutoHideOnlyActiveViewProperty);
                }
            set
                {
                SetValue(AutoHideOnlyActiveViewProperty, Boxes.Box(value));
                }
            }

        public Boolean EnableIndependentFloatingDocumentGroups
            {
            get
                {
                return (Boolean)GetValue(EnableIndependentFloatingDocumentGroupsProperty);
                }
            set
                {
                SetValue(EnableIndependentFloatingDocumentGroupsProperty, Boxes.Box(value));
                }
            }

        public Boolean EnableIndependentFloatingToolwindows
            {
            get
                {
                return (Boolean)GetValue(EnableIndependentFloatingToolwindowsProperty);
                }
            set
                {
                SetValue(EnableIndependentFloatingToolwindowsProperty, Boxes.Box(value));
                }
            }

        public Boolean MaintainPinStatus
            {
            get
                {
                return (Boolean)GetValue(MaintainPinStatusProperty);
                }
            set
                {
                SetValue(MaintainPinStatusProperty, Boxes.Box(value));
                }
            }

        public Boolean IsPinnedTabPanelSeparate
            {
            get
                {
                return (Boolean)GetValue(IsPinnedTabPanelSeparateProperty);
                }
            set
                {
                SetValue(IsPinnedTabPanelSeparateProperty, Boxes.Box(value));
                }
            }

        public Boolean ShowPinButtonInUnpinnedTabs
            {
            get
                {
                return (Boolean)GetValue(ShowPinButtonInUnpinnedTabsProperty);
                }
            set
                {
                SetValue(ShowPinButtonInUnpinnedTabsProperty, Boxes.Box(value));
                }
            }

        public TimeSpan AutoZOrderDelay
            {
            get
                {
                return (TimeSpan)GetValue(AutoZOrderDelayProperty);
                }
            set
                {
                SetValue(AutoZOrderDelayProperty, value);
                }
            }

        public Boolean ShowAutoHiddenWindowsOnHover
            {
            get
                {
                return (Boolean)GetValue(ShowAutoHiddenWindowsOnHoverProperty);
                }
            set
                {
                SetValue(ShowAutoHiddenWindowsOnHoverProperty, Boxes.Box(value));
                }
            }

        private static void OnEnableIndependentFloatingDocumentGroupsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
            {
            var managerPreferences = (ViewManagerPreferences)obj;
            if (!managerPreferences.EnableIndependentFloatingDocumentGroups)
                managerPreferences.EnableIndependentFloatingToolwindows = false;
            if (ViewManager.Instance.FloatingWindowManager == null)
                return;
            ViewManager.Instance.FloatingWindowManager.UpdateFloatingOwners();
            }

        private static void OnEnableIndependentFloatingToolwindowsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
            {
            if (ViewManager.Instance.FloatingWindowManager == null)
                return;
            ViewManager.Instance.FloatingWindowManager.UpdateFloatingOwners();
            }

        private static Object CoerceEnableIndependentFloatingToolwindows(DependencyObject obj, Object value)
            {
            if (!((ViewManagerPreferences)obj).EnableIndependentFloatingDocumentGroups)
                return Boxes.BooleanFalse;
            return value;
            }
        }
    }