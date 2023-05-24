using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelValidation
{
    public class NumberCodeConfirmed : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string errorType;
            if (!IsValidCode(value))
            {
                errorType = "là số từ 0 tới 9";
            }         
            else
            {
                return ValidationResult.Success;
            }

            ErrorMessage = $"{validationContext.DisplayName} dữ liệu  {errorType}";

            return new ValidationResult(ErrorMessage);
        }

        bool IsValidCode(object value)
        {
            if (value != null)
            {
                var c = Regex.IsMatch(value.ToString(), @"^[0-9]+$");

                if (c)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

    }
}
