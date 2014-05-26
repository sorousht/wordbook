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
            {2,"This week"},
            {3,"This month"},
            {4,"Last 6 Months"},
            {5,"This year"},
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