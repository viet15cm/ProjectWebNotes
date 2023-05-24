using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModelValidation
{
    public class NotEmailValidtions : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string errorType;
            if (!IsValidEmail(value))
            {
                errorType = "!";
            }
            else
            {
                return ValidationResult.Success;
            }
            ErrorMessage = $"{validationContext.DisplayName} không được là Email  {errorType}";
            return new ValidationResult(ErrorMessage);
        }

        bool IsValidEmail(object value)
        {
            if (value != null)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(value.ToString());
                    return !(addr.Address == (string)value);
                }
                catch
                {
                    return true;
                }
            }

            return true;
        }
    }
}
