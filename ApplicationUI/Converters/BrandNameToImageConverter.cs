using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FittingSoftwareEnums;

namespace ApplicationUI.Converters
{
    public class BrandNameToImageConverter : IValueConverter
    {
        private Dictionary<FittingSoftwares, string> imagePaths = new Dictionary<FittingSoftwares, string>
        {
            {FittingSoftwares.Genie, "Views/Images/genie.bmp"},
            {FittingSoftwares.Oasis, "Views/Images/Oasis.ico"},
            {FittingSoftwares.ExpressFit, "Views/Images/EXPRESSfitPRO.ico"},
            {FittingSoftwares.HearSuite, "Views/Images/hearSuite.ico"},
            {FittingSoftwares.GenieMedical, "Views/Images/medical.ico"},
            {FittingSoftwares.Noah4, "Views/Images/noah4.jpg"},
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            var path = (FittingSoftwares)value;
            return imagePaths[path];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
