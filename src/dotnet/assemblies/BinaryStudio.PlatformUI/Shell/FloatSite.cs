using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using BinaryStudio.PlatformUI.Shell.Controls;
using BinaryStudio.PlatformUI.Shell.Serialization;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable(FactoryMethodName = "Create")]
    public class FloatSite : ViewSite
        {
        private static IsIndependentConverter isIndependentConverter = new IsIndependentConverter();
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(Guid), typeof(FloatSite));
        public static readonly DependencyProperty IsIndependentProperty = DependencyProperty.Register("IsIndependent", typeof(Boolean), typeof(FloatSite), new PropertyMetadata(Boxes.BooleanFalse));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override WindowProfileSerializationVariants SerializationVariants
            {
            get
                {
                var serializationVariants = WindowProfileSerializationVariants.Default;
                if (HasDocumentGroupContainer)
                    serializationVariants |= WindowProfileSerializationVariants.Restricted;
                return serializationVariants;
                }
            }

        public Guid Id
            {
            get
                {
                return (Guid)GetValue(IdProperty);
                }
            set
                {
                SetValue(IdProperty, value);
                }
            }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Boolean IsIndependent
            {
            get
                {
                return (Boolean)GetValue(IsIndependentProperty);
                }
            set
                {
                SetValue(IsIndependentProperty, Boxes.Box(value));
                }
            }

        public FloatSite()
            {
            Id = Guid.NewGuid();
            BindingOperations.SetBinding(this, IsIndependentProperty, new MultiBinding
                {
                Bindings = {
              new Binding
              {
                  Source = ViewManager.Instance.Preferences,
                  Path = new PropertyPath("EnableIndependentFloatingDocumentGroups", new Object[0]),
                  Mode = BindingMode.OneWay
              },
              new Binding
              {
                  Source = ViewManager.Instance.Preferences,
                  Path = new PropertyPath("EnableIndependentFloatingToolwindows", new Object[0]),
                  Mode = BindingMode.OneWay
              },
              new Binding
              {
                  Source = this,
                  Path = new PropertyPath("HasDocumentGroupContainer", new Object[0]),
                  Mode = BindingMode.OneWay
              }
          },
                Converter = isIndependentConverter
                });
            }

        public override ICustomXmlSerializer CreateSerializer()
            {
            return new FloatSiteCustomSerializer(this);
            }

        public static Boolean IsFloating(ViewElement element)
            {
            return FindRootElement(element) is FloatSite;
            }

        public override Boolean IsChildAllowed(ViewElement e)
            {
            if (!(e is View) && !(e is DockGroup) && (!(e is TabGroup) && !(e is ViewBookmark)))
                return e is AutoHideRoot;
            return true;
            }

        public static FloatSite Create()
            {
            return ViewElementFactory.Current.CreateFloatSite();
            }

        protected override void TryCollapseCore()
            {
            if (Children.Count != 0)
                return;
            WindowProfile.Children.Remove(this);
            }

        internal void RemoveDocumentGroupContainer(WindowProfile profile)
            {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));
            var documentGroupContainer = Find<DocumentGroupContainer>(false);
            if (documentGroupContainer == null)
                return;
            var child = Find<DockRoot>(false).Children[0];
            using (child.PreventCollapse())
                {
                foreach (var view in new List<View>(FindAll<View>(v => AutoHideChannel.IsAutoHidden(v))))
                    {
                    if (AutoHideChannel.IsAutoHidden(view))
                        DockOperations.DockViewElementOrGroup(view);
                    }
                documentGroupContainer.Detach();
                foreach (ViewElement element in new List<View>(documentGroupContainer.FindAll<View>()))
                    element.Float(profile);
                child.Detach();
                child.Display = Display;
                child.FloatingLeft = FloatingLeft;
                child.FloatingTop = FloatingTop;
                child.FloatingWidth = FloatingWidth;
                child.FloatingHeight = FloatingHeight;
                child.Float(profile);
                }
            }
        }
    }