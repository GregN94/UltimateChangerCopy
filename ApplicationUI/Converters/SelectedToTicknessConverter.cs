using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ApplicationUI.Converters
{
    public class SelectedToTicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool selected)
            {
                if (selected)
                {
                    ThicknessConverter converter = new ThicknessConverter();
                    return converter.ConvertFrom("2");
                }
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
