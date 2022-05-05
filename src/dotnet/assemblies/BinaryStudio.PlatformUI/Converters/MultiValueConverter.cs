using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using BinaryStudio.PlatformUI.Properties;

namespace BinaryStudio.PlatformUI
    {
    public abstract class MultiValueConverter<T> : IMultiValueConverter
        {
        protected static readonly Object DisconnectedSource = typeof(BindingExpressionBase).GetField("DisconnectedItem", BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);

        #region P:ExpectedSourceValueCount:Int32
        private Int32 _expectedSourceValueCount = -1;
		private Int32 ExpectedSourceValueCount { get {
			if (_expectedSourceValueCount == -1) {
				var typeFromHandle = typeof(MultiValueConverter<T>);
				var type = GetType();
				var baseType = type.BaseType;
				while (baseType != typeFromHandle) {
					type = baseType;
					baseType = baseType.BaseType;
				    }
				var genericArguments = type.GetGenericArguments();
				_expectedSourceValueCount = genericArguments.Length - 1;
			    }
			return _expectedSourceValueCount;
			}}
        #endregion

        /// <summary>Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.</summary>
        /// <returns>A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public abstract Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture);

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public abstract Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture);

        #region M:ValidateConvertParameters(Object[],Type):Boolean
        protected Boolean ValidateConvertParameters(Object[] values, Type targetType) {
            if (values.Length != ExpectedSourceValueCount) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.Error_InsufficientSourceParameters, ExpectedSourceValueCount)); }
            for (var i = 0; i < values.Length; i++) {
                var value = values[i];
                if ((value == DependencyProperty.UnsetValue) || (value == DisconnectedSource)) { return false; }
                }
            if (!targetType.IsAssignableFrom(typeof(T))) { throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.Error_TargetNotExtendingType, typeof(T).FullName)); }
            return true;
            }
        #endregion
        #region M:ValidateConvertBackParameters(Object,Type[])
        protected void ValidateConvertBackParameters(Object value, Type[] targetTypes) {
            if (targetTypes.Length != ExpectedSourceValueCount) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.Error_InsufficientTypeParameters, ExpectedSourceValueCount)); }
            if (!(value is T) && (value != null || typeof(T).IsValueType)) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ValueNotOfType, typeof(T).FullName)); }
            }
        #endregion
        #region M:MakeConverterFunctionNotDefinedException(String):Exception
        protected Exception MakeConverterFunctionNotDefinedException([CallerMemberName] String functionName = "") {
            return new NotSupportedException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, functionName));
            }
        #endregion
        }

    public class MultiValueConverter<T1, T2, TTarget> : MultiValueConverter<TTarget>
        {
        /// <summary>Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.</summary>
        /// <returns>A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public sealed override Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
            {
            if (!ValidateConvertParameters(values, targetType))
                {
                return default(TTarget);
                }
            return Convert(MultiValueHelper.CheckValue<T1>(values, 0), MultiValueHelper.CheckValue<T2>(values, 1), parameter, culture);
            }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public sealed override Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture)
            {
            ValidateConvertBackParameters(value, targetTypes);
            MultiValueHelper.CheckType<T1>(targetTypes, 0);
            MultiValueHelper.CheckType<T2>(targetTypes, 1);
            T1 t1;
            T2 t2;
            ConvertBack((TTarget)value, out t1, out t2, parameter, culture);
            return new Object[]
                {
                t1,
                t2
                };
            }

        protected virtual TTarget Convert(T1 value1, T2 value2, Object parameter, CultureInfo culture)
            {
            throw MakeConverterFunctionNotDefinedException("Convert");
            }

        protected virtual void ConvertBack(TTarget value, out T1 value1, out T2 value2, Object parameter, CultureInfo culture)
            {
            throw MakeConverterFunctionNotDefinedException("ConvertBack");
            }
        }

    public class MultiValueConverter<T1,T2,T3,T> : MultiValueConverter<T>
        {
        /// <summary>Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.</summary>
        /// <returns>A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public sealed override Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture) {
            if (!ValidateConvertParameters(values, targetType)) { return default(T); }
            return Convert(MultiValueHelper.CheckValue<T1>(values, 0), MultiValueHelper.CheckValue<T2>(values, 1), MultiValueHelper.CheckValue<T3>(values, 2), parameter, culture);
            }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public sealed override Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture) {
            ValidateConvertBackParameters(value, targetTypes);
            MultiValueHelper.CheckType<T1>(targetTypes, 0);
            MultiValueHelper.CheckType<T2>(targetTypes, 1);
            MultiValueHelper.CheckType<T3>(targetTypes, 2);
            T1 value1;
            T2 value2;
            T3 value3;
            ConvertBack((T)value, out value1, out value2, out value3, parameter, culture);
            return new Object[] {
                value1,
                value2,
                value3
                };
            }

        protected virtual T Convert(T1 value1, T2 value2, T3 value3, Object parameter, CultureInfo culture) {
            throw MakeConverterFunctionNotDefinedException("Convert");
            }

        protected virtual void ConvertBack(T value, out T1 value1, out T2 value2, out T3 value3, Object parameter, CultureInfo culture) {
            throw MakeConverterFunctionNotDefinedException("ConvertBack");
            }
        }
    }