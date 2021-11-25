using System;
using System.Globalization;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class TabGroupBorderThicknessConverter : ValueConverter<Thickness, Thickness>
        {
        protected override Thickness Convert(Thickness value, Object parameter, CultureInfo culture) {
            var tabGroupBorderType = (TabGroupBorderType)parameter;
            var result = default(Thickness);
            if (tabGroupBorderType != TabGroupBorderType.HeaderBorder) {
                if (tabGroupBorderType == TabGroupBorderType.ContentBorder) {
                    result = new Thickness {
                        Left = value.Left,
                        Right = value.Right,
                        Top = 0.0,
                        Bottom = value.Bottom
                        };
                    }
                }
            else {
                result = new Thickness {
                    Left = value.Left,
                    Right = value.Right,
                    Top = value.Top,
                    Bottom = 0.0
                    };
                }
            return result;
            }
        }
    }