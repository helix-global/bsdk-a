using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class XYViewportSelectorItem : Control
        {
        static XYViewportSelectorItem()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XYViewportSelectorItem), new FrameworkPropertyMetadata(typeof(XYViewportSelectorItem)));
            }
        }
    }
