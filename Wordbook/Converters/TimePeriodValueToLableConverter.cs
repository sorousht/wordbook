using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Wordbook.Converters
{
    public class TimePeriodValueToLableConverter:IValueConverter
    {
        private readonly static IDictionary<int,string> TimePeriods=new Dictionary<int, string>
        {
            {0,"Today"},
            {1,"Yesterday"},
            {2,"Last week"},
            {3,"Last month"},
            {4,"6 Months ago"},
            {5,"Last year"},
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = System.Convert.ToInt32(value);
            return TimePeriods.ContainsKey(key) ? TimePeriods[key] : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}