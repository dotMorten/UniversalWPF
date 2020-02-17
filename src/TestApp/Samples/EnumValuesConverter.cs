using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TestApp.Samples
{
    public class EnumValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type enumType = null;
            if(parameter is string typename)
            {
                enumType = Type.GetType(typename);
            }
            else if(parameter is Type t)
            {
                enumType = t;
            }
            if(enumType.IsEnum)
            {
                return Enum.GetValues(enumType);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
