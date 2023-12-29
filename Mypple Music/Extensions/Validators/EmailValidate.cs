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
    public class EmailValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((value ?? "").ToString())) return new ValidationResult(false, "E-mail can not be empty !");
            
            var mail = value!.ToString()!.Trim();
            
            string pattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            bool isMatch = Regex.IsMatch(mail, pattern);
            string tips = isMatch ? "" : "Email format error !";
            return new ValidationResult(isMatch, tips);
        }
    }
}
