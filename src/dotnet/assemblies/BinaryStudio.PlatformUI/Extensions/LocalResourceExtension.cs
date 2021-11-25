using System;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace BinaryStudio.PlatformUI
    {
    public class LocalResourceExtension : MarkupExtension
        {
        public ResourceManager Source { get;set; }
        public BindingBase Culture { get;set; }
        public Object Key { get;set; }

        private class LocalResource : IMultiValueConverter
            {
            public Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
                {
                var manager = (ResourceManager)values[0];
                var key     = values[1].ToString();
                culture     = (CultureInfo)values[2];
                return manager.GetString(key, culture);
                }

            public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture)
                {
                //throw new NotImplementedException();
                return null;
                }
            }

        public override Object ProvideValue(IServiceProvider serviceProvider)
            {
            var binding = new MultiBinding{
                Converter = new LocalResource()
                };
            binding.Bindings.Add(new Binding{ Source = Source});
            binding.Bindings.Add(new Binding{ Source = Key});
            binding.Bindings.Add(Culture);
            return binding.ProvideValue(serviceProvider);
            }
        }
    }