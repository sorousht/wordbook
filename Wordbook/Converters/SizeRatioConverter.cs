using System;
using System.Globalization;
using System.Windows.Data;

namespace Wordbook.Converters
{
    public class SizeRatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ratio = System.Convert.ToDouble(parameter);

            if (Math.Abs(ratio) <= 0)
            {
                ratio = 1d;
            }

            var size = System.Convert.ToDouble(value);

            return size * ratio;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}