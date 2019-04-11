
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using epubto.Models;

namespace epubto.Converters
{
    public class StateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is EpubItem.State item)
            {
                switch (item)
                {
                    case EpubItem.State.Processing:
                        {
                            return Visibility.Visible;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
