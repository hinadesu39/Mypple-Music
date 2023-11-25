using SixLabors.Fonts.Unicode;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mypple_Music.Extensions.Converters
{
    public class VolumeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((double)value)
            {
                case double n when (n == 0):
                    return 0;
                case double n when (n > 0 && n < 0.34):
                    return 1;
                case double n when (n >= 0.34 && n < 0.8):
                    return 2;
                case double n when (n >= 0.8):
                    return 3;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
