
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
    public class StateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is EpubItem.State item)
            {
                switch (item)
                {
                    case EpubItem.State.Completed:
                        {
                            return false;
                        }
                    case EpubItem.State.Processing:
                        {
                            return false;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            return true;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
