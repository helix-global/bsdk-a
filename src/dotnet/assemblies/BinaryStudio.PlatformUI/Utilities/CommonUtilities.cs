using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using BinaryStudio.PlatformUI.Shell;

namespace BinaryStudio.PlatformUI {
    internal static class CommonUtilities {
        public static Int32 RoundToInt(Double d) {
            return (Int32)Math.Round(d);
            }

        public static System.Windows.Interop.MSG MSGFromMessageParam(IntPtr hwnd, UInt32 message, UInt32 wParam, Int32 lParam) {
            var msg = new System.Windows.Interop.MSG();
            msg.hwnd = hwnd;
            msg.message = (Int32)message;
            msg.wParam = new IntPtr((Int32)wParam);
            msg.lParam = new IntPtr(lParam);
            msg.time = NativeMethods.GetMessageTime();
            var messagePos = NativeMethods.GetMessagePos();
            msg.pt_x = NativeMethods.GetXLParam(messagePos);
            msg.pt_y = NativeMethods.GetYLParam(messagePos);
            return msg;
            }

        //public static System.Windows.Interop.MSG MSGFromOleMSG(ref Microsoft.VisualStudio.OLE.Interop.MSG oleMsg)
        //{
        //  return new System.Windows.Interop.MSG()
        //  {
        //    hwnd = oleMsg.hwnd,
        //    message = (int) oleMsg.message,
        //    wParam = oleMsg.wParam,
        //    lParam = oleMsg.lParam,
        //    time = (int) oleMsg.time,
        //    pt_x = oleMsg.pt.x,
        //    pt_y = oleMsg.pt.y
        //  };
        //}

        //public static Microsoft.VisualStudio.OLE.Interop.MSG OleMSGFromMSG(ref System.Windows.Interop.MSG msg)
        //{
        //  Microsoft.VisualStudio.OLE.Interop.MSG msg1;
        //  msg1.hwnd = msg.hwnd;
        //  msg1.message = (uint) msg.message;
        //  msg1.wParam = msg.wParam;
        //  msg1.lParam = msg.lParam;
        //  msg1.time = (uint) msg.time;
        //  msg1.pt.x = msg.pt_x;
        //  msg1.pt.y = msg.pt_y;
        //  return msg1;
        //}

        internal static void InvokeOnUIThread(Action action) {
            ThreadHelper.Generic.Invoke(action);
            }

        internal static T InvokeOnUIThread<T>(Func<T> func) {
            return ThreadHelper.Generic.Invoke(func);
            }

        public static Rect Resize(this Rect rect, Vector positionChangeDelta, Vector sizeChangeDelta, Size minSize, Size maxSize) {
            var width = Math.Min(Math.Max(minSize.Width, rect.Width + sizeChangeDelta.X), maxSize.Width);
            var height = Math.Min(Math.Max(minSize.Height, rect.Height + sizeChangeDelta.Y), maxSize.Height);
            var right = rect.Right;
            var bottom = rect.Bottom;
            return new Rect(Math.Min(right - minSize.Width, Math.Max(right - maxSize.Width, rect.Left + positionChangeDelta.X)), Math.Min(bottom - minSize.Height, Math.Max(bottom - maxSize.Height, rect.Top + positionChangeDelta.Y)), width, height);
            }

        public static Uri MakePackUri(Assembly assembly, String path) {
            var r = String.Format("pack://application:,,,/{0};component/{1}", assembly.GetName().Name, path);
            //var r = String.Format("/{1}", assembly.GetName().Name, path);
            var uri = new Uri(r, UriKind.Absolute);
            //return new Uri(string.Format("/Helix.PlatformUI;component/{0}", (object)path), UriKind.Relative);
            return uri;
            }

        public static String Escape(String text, Char[] charsToEscape, Char escapeChar) {
            if (String.IsNullOrEmpty(text) || charsToEscape == null || charsToEscape.Length == 0)
                return text;
            var stringBuilder = new StringBuilder(text.Length);
            foreach (var ch in text) {
                if (charsToEscape.Contains(ch)) {
                    stringBuilder.Append(escapeChar);
                    stringBuilder.Append(((Int32)ch).ToString("x4"));
                    }
                else {
                    stringBuilder.Append(ch);
                    if (ch == escapeChar)
                        stringBuilder.Append(ch);
                    }
                }
            return stringBuilder.ToString();
            }

        public static String Unescape(String text, Char escapeChar) {
            if (String.IsNullOrEmpty(text))
                return text;
            var stringBuilder = new StringBuilder(text.Length);
            for (var index = 0; index < text.Length; ++index) {
                if (text[index] != escapeChar) {
                    stringBuilder.Append(text[index]);
                    }
                else {
                    if (index + 1 >= text.Length)
                        return stringBuilder.ToString();
                    if (text[index + 1] == escapeChar) {
                        stringBuilder.Append(escapeChar);
                        ++index;
                        }
                    else {
                        if (index + 5 > text.Length)
                            return stringBuilder.ToString();
                        Int32 result;
                        if (!Int32.TryParse(text.Substring(index + 1, 4), NumberStyles.HexNumber, null, out result))
                            return stringBuilder.ToString();
                        stringBuilder.Append((Char)result);
                        index += 4;
                        }
                    }
                }
            return stringBuilder.ToString();
            }
        }
    }