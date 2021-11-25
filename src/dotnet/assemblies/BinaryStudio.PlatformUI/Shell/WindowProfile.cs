using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;
using BinaryStudio.PlatformUI.Shell.Serialization;

namespace BinaryStudio.PlatformUI.Shell
    {
    [ContentProperty("Children")]
    [DefaultProperty("Children")]
    [XamlSerializable]
    public class WindowProfile : DependencyObject, ICustomXmlSerializable, IDependencyObjectCustomSerializerAccess
        {
        #region P:Name:String
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(String), typeof(WindowProfile));
        public String Name {
            get { return (String)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
            }
        #endregion
        #region P:ICustomXmlSerializable.SerializationVariants:WindowProfileSerializationVariants
        WindowProfileSerializationVariants ICustomXmlSerializable.SerializationVariants { get {
            return WindowProfileSerializationVariants.Default |
                   WindowProfileSerializationVariants.Restricted;
            }}
        #endregion

        public MainSite MainSite {get; private set; }
        public AutoHideRoot AutoHideRoot {get; private set; }
        public AutoHideChannel LeftAutoHideChannel {get; private set; }
        public AutoHideChannel RightAutoHideChannel {get; private set; }
        public AutoHideChannel TopAutoHideChannel {get; private set; }
        public AutoHideChannel BottomAutoHideChannel {get; private set; }
        public DockRoot DockRoot {get; private set; }
        public ViewManager ViewManager { get;internal set; }

        public IObservableCollection<ViewElement> Children { get; }


        public WindowProfile()
            {
            Children = new WindowProfileElementCollection(this);
            }

        #region M:ICustomXmlSerializable.CreateSerializer:ICustomXmlSerializer
        ICustomXmlSerializer ICustomXmlSerializable.CreateSerializer() {
            return new WindowProfileCustomSerializer(this);
            }
        #endregion
        #region M:ICustomXmlSerializable.GetSerializedType:Type
        Type ICustomXmlSerializable.GetSerializedType() {
            return GetType();
            }
        #endregion

        Boolean IDependencyObjectCustomSerializerAccess.ShouldSerializeProperty(DependencyProperty dp) {
            return ShouldSerializeProperty(dp);
            }

        Object IDependencyObjectCustomSerializerAccess.GetValue(DependencyProperty dp) {
            return GetValue(dp);
            }

        #region M:Find(Predicate<ViewElement>):ViewElement
        public ViewElement Find(Predicate<ViewElement> predicate) {
            if (predicate == null)  { return null; }
            foreach (var child in Children) {
                var viewElement = child.Find(predicate, false);
                if (viewElement != null)
                    return viewElement;
                }
            return null;
            }
        #endregion
        #region M:Find<T>:T
        public T Find<T>() where T : ViewElement {
            return (T)Find(element => element is T);
            }
        #endregion
        #region M:Find<T>(Predicate<T>)>:T
        public T Find<T>(Predicate<T> predicate) where T : ViewElement {
            return (T)Find(element => {
                if (element is T) {
                    return predicate((T)element);
                    }
                return false;
                });
            }
        #endregion
        #region M:FindAll(Predicate<ViewElement>):IEnumerable<ViewElement>
        public IEnumerable<ViewElement> FindAll(Predicate<ViewElement> predicate) {
            if (predicate != null) {
                foreach (var child in Children) {
                    foreach (var viewElement in child.FindAll(predicate)) {
                        yield return viewElement;
                        }
                    }
                }
            }
        #endregion
        #region M:FindAll<T>:IEnumerable<T>
        public IEnumerable<T> FindAll<T>() where T : ViewElement {
            return FindAll(element => element is T).Cast<T>();
            }
        #endregion
        #region M:FindAll<T>(Predicate<T>):IEnumerable<T>
        public IEnumerable<T> FindAll<T>(Predicate<T> predicate) where T : ViewElement {
            return FindAll<T>().Where(t => predicate(t));
            }
        #endregion
        #region M:FindWindowProfile(ViewElement):WindowProfile
        public static WindowProfile FindWindowProfile(ViewElement view) {
            return (WindowProfile)ViewElement.FindRootElement(view).GetValue(ViewElement.WindowProfileProperty);
            }
        #endregion
        #region M:ToString:String
        public override String ToString() {
            return String.Format("{0}, Name = {1}, Children = {2}", GetType().Name, Name, Children.Count);
            }
        #endregion

        public static WindowProfile Create(String profileName)
            {
            var profile = new WindowProfile {Name = profileName };
            profile.InitializeOverride();
            return profile;
            }

        protected void InitializeOverride() {
            var site = MainSite.Create();
            ViewManager.BindViewManager(site, this);
            site.Child = CreateDefaultViewSiteContent();
            Children.Add(site);
            MainSite = site;
            AutoHideRoot = (AutoHideRoot)site.Children[0];
            LeftAutoHideChannel   = (AutoHideChannel)AutoHideRoot.Children[0];
            RightAutoHideChannel  = (AutoHideChannel)AutoHideRoot.Children[1];
            TopAutoHideChannel    = (AutoHideChannel)AutoHideRoot.Children[2];
            BottomAutoHideChannel = (AutoHideChannel)AutoHideRoot.Children[3];
            DockRoot = (DockRoot)AutoHideRoot.Children[4];
            }

        public static ViewElement CreateDefaultViewSiteContent() {
            var autoHideChannel1 = AutoHideChannel.Create();
            autoHideChannel1.Dock = Dock.Left;
            autoHideChannel1.Orientation = Orientation.Vertical;
            var autoHideChannel2 = AutoHideChannel.Create();
            autoHideChannel2.Dock = Dock.Right;
            autoHideChannel2.Orientation = Orientation.Vertical;
            var autoHideChannel3 = AutoHideChannel.Create();
            autoHideChannel3.Dock = Dock.Top;
            autoHideChannel3.Orientation = Orientation.Horizontal;
            var autoHideChannel4 = AutoHideChannel.Create();
            autoHideChannel4.Dock = Dock.Bottom;
            autoHideChannel4.Orientation = Orientation.Horizontal;
            var documentGroupContainer = DocumentGroupContainer.Create();
            documentGroupContainer.DockedWidth = new SplitterLength(1.0, 0);
            documentGroupContainer.DockedHeight = new SplitterLength(1.0, 0);
            documentGroupContainer.Children.Add(DocumentGroup.Create());
            var dockRoot = DockRoot.Create();
            dockRoot.DockedWidth = new SplitterLength(1.0, 0);
            dockRoot.DockedHeight = new SplitterLength(1.0, 0);
            dockRoot.Children.Add(documentGroupContainer);
            var autoHideRoot = AutoHideRoot.Create();
            autoHideRoot.DockedWidth = new SplitterLength(1.0, 0);
            autoHideRoot.DockedHeight = new SplitterLength(1.0, 0);
            autoHideRoot.Children.Add(autoHideChannel1);
            autoHideRoot.Children.Add(autoHideChannel2);
            autoHideRoot.Children.Add(autoHideChannel3);
            autoHideRoot.Children.Add(autoHideChannel4);
            autoHideRoot.Children.Add(dockRoot);
            return autoHideRoot;
            }

        #region M:Load(String,IEnumerable<Assembly>):WindowProfile
        public static WindowProfile Load(String profileXml, IEnumerable<Assembly> safeAssemblies) {
            using (var stringReader = new StringReader(profileXml)) {
                var settings = new XmlReaderSettings {
                    CheckCharacters = false,
                    CloseInput = false
                    };
                using (var reader = XmlReader.Create(stringReader, settings)) {
                    return LoadInternal(reader, safeAssemblies);
                    }
                }
            }
        #endregion
        #region M:Load(Stream,IEnumerable<Assembly>):WindowProfile
        public static WindowProfile Load(Stream stream, IEnumerable<Assembly> safeAssemblies) {
            var settings = new XmlReaderSettings {
                CheckCharacters = false,
                CloseInput = false
                };
            using (var reader = XmlReader.Create(stream, settings)) {
                return LoadInternal(reader, safeAssemblies);
                }
            }
        #endregion
        #region M:LoadInternal(XmlReader,IEnumerable<Assembly>):WindowProfile
        private static WindowProfile LoadInternal(XmlReader reader, IEnumerable<Assembly> safeAssemblies) {
            try {
                var xamlXmlReader = new XamlXmlReader(reader, new XamlSchemaContext(safeAssemblies));
                WindowProfile profile;
                using (ViewElementFactory.Current.AllowConstruction())
                    profile = (WindowProfile)XamlServices.Load(xamlXmlReader);
                new WindowProfileValidator().Validate(profile);
                return profile;
                }
            catch (XmlException e)  { throw new FileFormatException("Window profile contains malformed XML.", e); }
            catch (XamlException e) { throw new FileFormatException("Window profile contains malformed XAML.", e); }
            }
        #endregion
        #region M:Save(Stream)
        public void Save(Stream stream) {
            Save(stream, new WindowProfileSerializer());
            }
        #endregion
        #region M:Save(Stream,WindowProfileSerializer)
        public void Save(Stream stream, WindowProfileSerializer serializer) {
            serializer.Serialize(this, stream);
            }
        #endregion
        #region M:Copy(String):WindowProfile
        public WindowProfile Copy(String newName) {
            return Copy(newName, new WindowProfileSerializer());
            }
        #endregion
        #region M:Copy(String,WindowProfileSerializer):WindowProfile
        public WindowProfile Copy(String newName, WindowProfileSerializer serializer) {
            using (var memoryStream = new MemoryStream()) {
                var settings = new XmlReaderSettings
                {
                    CheckCharacters = false
                    };
                Save(memoryStream, serializer);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                using (var xmlReader = XmlReader.Create(memoryStream, settings)) {
                    WindowProfile windowProfile;
                    using (ViewElementFactory.Current.AllowConstruction())
                        windowProfile = (WindowProfile)XamlServices.Load(xmlReader);
                    windowProfile.Name = newName;
                    return windowProfile;
                    }
                }
            }
        #endregion
        #region M:Validate
        public void Validate() {
            new WindowProfileValidator().Validate(this);
            }
        #endregion
        #region M:Initialize(ContentControl)
        public void Initialize(ContentControl content)
            {
            var M = new ViewManager("Default");
            M.Initialize(content);
            M.WindowProfile = this;
            }
        #endregion

        private class WindowProfileElementCollection : OwnershipCollection<ViewElement>
            {
            public WindowProfile WindowProfile { get; }

            public WindowProfileElementCollection(WindowProfile windowProfile) {
                WindowProfile = windowProfile;
                }

            protected override void LoseOwnership(ViewElement element) {
                element.SetValue(ViewElement.WindowProfilePropertyKey, null);
                ViewManager.ClearViewManager(element);
                }

            protected override void TakeOwnership(ViewElement element) {
                if (element.WindowProfile != null)
                    element.WindowProfile.Children.Remove(element);
                element.SetValue(ViewElement.WindowProfilePropertyKey, WindowProfile);
                ViewManager.BindViewManager(element, WindowProfile);
                }

            protected override void BeforeBaseInsertItem(Int32 index, ViewElement item) {
                TakeOwnership(item);
                }

            protected override void AfterBaseInsertItem(Int32 index, ViewElement item) {
                }

            protected override void OnMaximumItemsExceeded(ViewElement element) {
                }
            }
        }
    }