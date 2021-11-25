using System;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class ViewElementFactory
        {
        public static ViewElementFactory Current { get; set; }

        internal Boolean IsConstructionAllowed
            {
            get
                {
                return AllowConstructionReferences > 0;
                }
            }

        private Int32 AllowConstructionReferences { get; set; }

        static ViewElementFactory()
            {
            Current = new ViewElementFactory();
            }

        public TabGroup CreateTabGroup()
            {
            using (AllowConstruction())
                return CreateTabGroupCore();
            }

        protected virtual TabGroup CreateTabGroupCore()
            {
            return new TabGroup();
            }

        public DockGroup CreateDockGroup()
            {
            using (AllowConstruction())
                return CreateDockGroupCore();
            }

        protected virtual DockGroup CreateDockGroupCore()
            {
            return new DockGroup();
            }

        public DocumentGroup CreateDocumentGroup()
            {
            using (AllowConstruction())
                return CreateDocumentGroupCore();
            }

        protected virtual DocumentGroup CreateDocumentGroupCore()
            {
            return new DocumentGroup();
            }

        public DocumentGroupContainer CreateDocumentGroupContainer()
            {
            using (AllowConstruction())
                return CreateDocumentGroupContainerCore();
            }

        protected virtual DocumentGroupContainer CreateDocumentGroupContainerCore()
            {
            return new DocumentGroupContainer();
            }

        public AutoHideGroup CreateAutoHideGroup()
            {
            using (AllowConstruction())
                return CreateAutoHideGroupCore();
            }

        protected virtual AutoHideGroup CreateAutoHideGroupCore()
            {
            return new AutoHideGroup();
            }

        public AutoHideChannel CreateAutoHideChannel()
            {
            using (AllowConstruction())
                return CreateAutoHideChannelCore();
            }

        protected virtual AutoHideChannel CreateAutoHideChannelCore()
            {
            return new AutoHideChannel();
            }

        public AutoHideRoot CreateAutoHideRoot()
            {
            using (AllowConstruction())
                return CreateAutoHideRootCore();
            }

        protected virtual AutoHideRoot CreateAutoHideRootCore()
            {
            return new AutoHideRoot();
            }

        public DockRoot CreateDockRoot()
            {
            using (AllowConstruction())
                return CreateDockRootCore();
            }

        protected virtual DockRoot CreateDockRootCore()
            {
            return new DockRoot();
            }

        public FloatSite CreateFloatSite()
            {
            using (AllowConstruction())
                return CreateFloatSiteCore();
            }

        protected virtual FloatSite CreateFloatSiteCore()
            {
            return new FloatSite();
            }

        #region M:CreateMainSite:MainSite
        public MainSite CreateMainSite()
            {
            using (AllowConstruction())
                {
                return CreateMainSiteCore();
                }
            }
        #endregion

        protected virtual MainSite CreateMainSiteCore()
            {
            return new MainSite();
            }

        public View CreateView()
            {
            using (AllowConstruction())
                return CreateViewCore(typeof(View));
            }

        public View CreateView(Type viewType)
            {
            if (!typeof(View).IsAssignableFrom(viewType))
                throw new InvalidOperationException("viewType must derive from View");
            using (AllowConstruction())
                return CreateViewCore(viewType);
            }

        protected virtual View CreateViewCore(Type viewType)
            {
            return (View)Activator.CreateInstance(viewType);
            }

        public ViewBookmark CreateViewBookmark()
            {
            using (AllowConstruction())
                return CreateViewBookmarkCore();
            }

        protected virtual ViewBookmark CreateViewBookmarkCore()
            {
            return new ViewBookmark();
            }

        public IDisposable AllowConstruction()
            {
            return new AllowPublicConstructionScope(this);
            }

        private class AllowPublicConstructionScope : DisposableObject
            {
            private ViewElementFactory Factory { get; }

            public AllowPublicConstructionScope(ViewElementFactory factory)
                {
                Factory = factory;
                ++Factory.AllowConstructionReferences;
                }

            protected override void DisposeManagedResources()
                {
                --Factory.AllowConstructionReferences;
                }
            }
        }
    }