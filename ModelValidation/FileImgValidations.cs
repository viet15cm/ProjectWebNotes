using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelValidation
{
    public class FileImgValidations : ValidationAttribute
    {
        private readonly string[] _extensions;

        public FileImgValidations(string[] extensions)
        {
            _extensions = extensions;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string errorType;
            
            if (!IsValidAllowedExtensions(value))
            {
                errorType = "đuôi mở rộng phải là (.jpg .jpeg .png .jfif) !";
            }
            else
            {
                return ValidationResult.Success;
            }

            ErrorMessage = $"{validationContext.DisplayName} dữ liệu  {errorType}";

            return new ValidationResult(ErrorMessage);
        }

        bool IsValidAllowedExtensions(object value)
        {
            var file = value as IFormFile;            
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return false;
                }
            }
            return true;
        }

       

        /*[FileImgValidation(new string[] { ".jpg", ".jpeg", ".png" })]*/


    }
}
