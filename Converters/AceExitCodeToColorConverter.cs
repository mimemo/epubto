using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace epubto.Converters
{
    public class AceExitCodeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is int item)
            {
                switch (item)
                {
                    case -1:
                        {
                            // non
                            return new SolidColorBrush(Colors.Transparent);
                        }
                    case 0:
                        {
                            // valid(Processing is over, but the result is not always correct)
                            return new SolidColorBrush(Colors.Transparent);
                        }
                    case 1:
                        {
                            // invalid
                            return new SolidColorBrush(Colors.LightPink);
                        }

                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.Transparent);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
