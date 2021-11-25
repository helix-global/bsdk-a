using System;
using System.Collections.Generic;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Serialization
    {
    internal class WindowProfileCustomSerializer : DependencyObjectCustomSerializer
        {
        protected override IEnumerable<DependencyProperty> SerializableProperties
            {
            get
                {
                yield return WindowProfile.NameProperty;
                }
            }

        public override Object Content
            {
            get
                {
                return ((WindowProfile)Owner).Children;
                }
            }

        public WindowProfileCustomSerializer(WindowProfile profile)
          : base(profile)
            {
            }
        }
    }