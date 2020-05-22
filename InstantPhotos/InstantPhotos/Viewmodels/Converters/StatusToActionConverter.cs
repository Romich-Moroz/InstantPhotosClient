using System;
using System.Globalization;
using Xamarin.Forms;

namespace InstantPhotos
{
    /// <summary>
    /// Converts server status to button text for connect button
    /// </summary>
    class StatusToActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == false ? "Connect to server" : "Disconnect from server";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
