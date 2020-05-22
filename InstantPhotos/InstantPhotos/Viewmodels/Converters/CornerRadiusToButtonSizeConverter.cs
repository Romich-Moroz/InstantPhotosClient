using System;
using System.Globalization;
using Xamarin.Forms;

namespace InstantPhotos
{
    class CornerRadiusToButtonSizeConverter : IValueConverter
    {
        /// <summary>
        /// Converts corner radius to width and height with formula width(height) = cornerRadius*2
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value * 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
