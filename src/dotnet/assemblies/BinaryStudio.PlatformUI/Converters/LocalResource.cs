using System;
using System.Dynamic;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Properties
    {
    internal class LocalResource : DynamicObject, IValueConverter
        {
        public ResourceManager ResourceManager { get;set; }
        public LocalResource() {
            ResourceManager = Resources.ResourceManager;
            }

        public static LocalResource Instance { get; }
        static LocalResource()
            {
            Instance = new LocalResource();
            }

        public override Boolean TryGetMember(GetMemberBinder binder, out Object result)
            {
            result = Resources.ResourceManager.GetString(binder.Name, Theme.Culture);
            return true;
            }

        #region M:IValueConverter.Convert(Object,Type,Object,CultureInfo):Object
        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            parameter = parameter ?? String.Empty;
            return ResourceManager.GetString(parameter.ToString(), Theme.Culture);
            }
        #endregion
        #region M:IValueConverter.ConvertBack(Object,Type,Object,CultureInfo):Object
        Object IValueConverter.ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            //throw new NotImplementedException();
            return value;
            }
        #endregion
        }
    }