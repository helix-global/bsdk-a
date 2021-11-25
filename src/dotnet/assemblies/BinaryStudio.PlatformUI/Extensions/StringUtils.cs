using System;
using System.Windows;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Controls.Primitives;

namespace BinaryStudio.PlatformUI.Extensions
    {
    public class StringUtils
        {
        public static String Format(String source, FrameworkElement target) {
            if (String.IsNullOrEmpty(source)) { return String.Empty; }
            var g = new GlyphRunBlock(source, new CustomTextRunProperties(target));
            return source;
            }
        }
    }