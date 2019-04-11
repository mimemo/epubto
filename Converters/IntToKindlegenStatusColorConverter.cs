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
    public class IntToKindlegenStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is int item)
            {
                switch (item)
                {
                    case 0:
                        {
                            return new SolidColorBrush(Colors.LightGreen);
                        }
                    case 1:
                        {
                            return new SolidColorBrush(Colors.Yellow);
                        }
                    case 2:
                        {
                            return new SolidColorBrush(Colors.LightPink);
                        }
                    case -1:
                        {
                            return new SolidColorBrush(Colors.White);
                        }

                    default:
                        {
                            return new SolidColorBrush(Colors.Transparent);
                        }
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
