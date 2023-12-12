using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mypple_Music.Extensions.Validators
{
    public class PhoneNumValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                var phoneNum = value.ToString();
                Regex regex = new Regex(@"^\d{11}$");
                if (!regex.IsMatch(phoneNum))
                {
                    return new ValidationResult(false, "请输入正确的手机号格式");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
