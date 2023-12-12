using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mypple_Music.Extensions.Validators
{
    public class UserNameValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                var userName = value.ToString();
                if (userName.Count() > 10)
                {
                    return new ValidationResult(false, "用户名长度不能超过10");
                }
                if (userName.Contains("@"))
                {
                    return new ValidationResult(false, "用户名中不能包含'@'");
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}
