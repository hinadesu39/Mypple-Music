using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Mypple_Music.Extensions.Converters
{
    public class DoubleToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && double.TryParse(value.ToString(), out double result))
            {
                TimeSpan ts = TimeSpan.FromSeconds(result);
                string str = ts.ToString(@"mm\:ss");
                return str;
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
