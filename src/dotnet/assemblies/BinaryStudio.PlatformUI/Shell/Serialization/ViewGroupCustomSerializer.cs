using System;

namespace BinaryStudio.PlatformUI.Shell.Serialization
    {
    internal class ViewGroupCustomSerializer : ViewElementCustomSerializer
        {
        public override Object Content
            {
            get
                {
                return ((ViewGroup)Owner).Children;
                }
            }

        public ViewGroupCustomSerializer(ViewGroup group)
          : base(@group)
            {
            }
        }
    }