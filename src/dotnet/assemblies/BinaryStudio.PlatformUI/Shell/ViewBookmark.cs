using System;
using System.ComponentModel;
using System.Windows;
using BinaryStudio.PlatformUI.Shell.Serialization;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable]
    public class ViewBookmark : ViewElement
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override WindowProfileSerializationVariants SerializationVariants {
            get {
                var serializationVariants = WindowProfileSerializationVariants.Default;
                if (this.FindAncestor<DocumentGroup>() != null)
                    serializationVariants |= WindowProfileSerializationVariants.Restricted;
                return serializationVariants;
                }
            }

        #region P:Name:String
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(String), typeof(ViewBookmark));
        public String Name {
            get { return (String)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
            }
        #endregion
        #region P:AccessOrder:Int32
        public static readonly DependencyProperty AccessOrderProperty = DependencyProperty.Register("AccessOrder", typeof(Int32), typeof(ViewBookmark), new FrameworkPropertyMetadata(Boxes.Int32Zero));
        [DefaultValue(0)]
        public Int32 AccessOrder {
            get { return (Int32)GetValue(AccessOrderProperty); }
            set { SetValue(AccessOrderProperty, value); }
            }
        #endregion
        #region P:ViewBookmarkType:ViewBookmarkType
        public static readonly DependencyProperty ViewBookmarkTypeProperty = DependencyProperty.Register("ViewBookmarkType", typeof(ViewBookmarkType), typeof(ViewBookmark), new FrameworkPropertyMetadata(ViewBookmarkType.Default, OnViewBookmarkTypeChanged));
        [DefaultValue(ViewBookmarkType.Default)]
        public ViewBookmarkType ViewBookmarkType {
            get { return (ViewBookmarkType)GetValue(ViewBookmarkTypeProperty); }
            set { SetValue(ViewBookmarkTypeProperty, value); }
            }
        #endregion

        public override ICustomXmlSerializer CreateSerializer() {
            return new ViewBookmarkCustomSerializer(this);
            }

        private static void OnViewBookmarkTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            if (ViewBookmarkType.All == (ViewBookmarkType)args.NewValue)
                throw new InvalidOperationException("All is not a valid type for a bookmark instance");
            }

        public static ViewBookmark Create() {
            return ViewElementFactory.Current.CreateViewBookmark();
            }

        public static ViewBookmark Create(String name, ViewBookmarkType type) {
            var viewBookmark = Create();
            viewBookmark.Name = name;
            viewBookmark.AccessOrder = 0;
            viewBookmark.ViewBookmarkType = type;
            return viewBookmark;
            }

        public static ViewBookmark Create(View source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var r = Create(source.Name, source.GetBookmarkType());
            ViewManager.BindViewManager(r, source);
            return r;
            }

        public override String ToString() {
            return String.Format("{0}, Type = {1}, Name = {2}", GetType().Name, ViewBookmarkType, Name);
            }
        }
    }