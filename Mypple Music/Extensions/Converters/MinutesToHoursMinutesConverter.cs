using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mypple_Music.Extensions.Converters
{
    public class MinutesToHoursMinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double second)
            {
                int hours = (int)(second / 3600);
                int minutes = (int)((second % 3600) / 60);
                second = (int)((second % 3600) % 60);
                return $"{hours}小时{minutes}分{second}秒";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
