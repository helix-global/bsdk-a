using System;
using System.Collections.Generic;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Serialization
    {
    public class ViewCustomSerializer : ViewElementCustomSerializer
        {
        protected override IEnumerable<DependencyProperty> SerializableProperties
            {
            get
                {
                yield return View.NameProperty;
                yield return View.IsPinnedProperty;
                foreach (var serializableProperty in base.SerializableProperties)
                    yield return serializableProperty;
                }
            }

        public ViewCustomSerializer(View view)
          : base(view)
            {
            }

        public override IEnumerable<KeyValuePair<String, Object>> GetNonContentPropertyValues()
            {
            var obj = (Object)null;
            if (!ExcludeLocalizable && ShouldSerializeProperty(View.TitleProperty, ref obj))
                yield return new KeyValuePair<String, Object>(View.TitleProperty.Name, obj);
            foreach (var contentPropertyValue in base.GetNonContentPropertyValues())
                yield return contentPropertyValue;
            }
        }
    }