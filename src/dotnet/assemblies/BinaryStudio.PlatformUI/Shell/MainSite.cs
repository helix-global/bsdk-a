using System;
using System.ComponentModel;
using BinaryStudio.PlatformUI.Shell.Serialization;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable]
    public class MainSite : ViewSite
        {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override WindowProfileSerializationVariants SerializationVariants
            {
            get
                {
                return WindowProfileSerializationVariants.Default | WindowProfileSerializationVariants.Restricted;
                }
            }

        public override ICustomXmlSerializer CreateSerializer()
            {
            return new MainSiteCustomSerializer(this);
            }

        public override Boolean IsChildAllowed(ViewElement e)
            {
            return e is AutoHideRoot;
            }

        public static MainSite Create()
            {
            return ViewElementFactory.Current.CreateMainSite();
            }

        public static MainSite Create(AutoHideRoot e) {
            var r = Create();
            r.Child = e;
            return r;
            }
        }
    }