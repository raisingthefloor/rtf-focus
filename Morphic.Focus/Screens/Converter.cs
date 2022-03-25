using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Morphic.Focus.Screens
{
    public class WidthMinusThirtyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Double)value) - 30;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    public class ComboItemSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MaximumValueVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int valueAsInt;
            try
            {
                valueAsInt = System.Convert.ToInt32(value);
            }
            catch
            {
                // if the 'value' argument was not convertible to an int, return collapsed
                System.Diagnostics.Debug.Assert(false, "Argument 'value' must be convertible to Int32");
                return Visibility.Collapsed;
            }

            int maximumValue;
            try
            {
                maximumValue = System.Convert.ToInt32(parameter);
            }
            catch
            {
                // if the 'parameter' argument was not convertible to an int, return collapsed
                System.Diagnostics.Debug.Assert(false, "Argument 'parameter' must be convertible to Int32");
                return Visibility.Collapsed;
            }

            return valueAsInt <= maximumValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MinimumValueVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int valueAsInt;
            try
            {
                valueAsInt = System.Convert.ToInt32(value);
            }
            catch
            {
                // if the 'value' argument was not convertible to an int, return collapsed
                System.Diagnostics.Debug.Assert(false, "Argument 'value' must be convertible to Int32");
                return Visibility.Collapsed;
            }

            int minimumValue;
            try
            {
                minimumValue = System.Convert.ToInt32(parameter);
            }
            catch
            {
                // if the 'parameter' argument was not convertible to an int, return collapsed
                System.Diagnostics.Debug.Assert(false, "Argument 'parameter' must be convertible to Int32");
                return Visibility.Collapsed;
            }

            return valueAsInt >= minimumValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                Visibility vis = (Visibility)value;
                return vis == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new InvalidOperationException("Converter can only convert to value of type Visibility.");
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
