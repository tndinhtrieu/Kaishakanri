using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Data;

namespace Kaishakanri.Helpers
{
    public class CustomValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value==null||string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Không được để rỗng trường này");
            }

            return ValidationResult.ValidResult;
        }
    }


    [ValueConversion(typeof(DateTime), typeof(DateTime))]
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime result = DateTime.Parse(((DateTime)value).ToShortDateString());

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime result;

            if (DateTime.TryParse((string)value, out result) == false)
                return null;

            return result;
        }

    }  
 
}
