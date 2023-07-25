using Microsoft.AspNetCore.Mvc;
using ModelValidation;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebNotes.Areas.Manager.Models
{
    public class InputFormFile
    {
        [BindProperty]
        [FileImgValidations(new string[] { ".jpg", ".jpeg", ".png", ".jfif" })]
        [Display(Name = "Icon")]
        public IFormFile FormFile { get; set; }
    }
}
