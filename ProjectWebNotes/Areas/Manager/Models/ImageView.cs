using Dto;
using ModelValidation;

namespace ProjectWebNotes.Areas.Manager.Models
{
    public class ImageView
    {
        
        [FileImagesVallidations(new string[] { ".jpg", ".jpeg", ".jfif", ".png", ".gif" })]
        public IFormFile[] FormFiles { get; set; }

        [FileImgValidations(new string[] { ".jpg", ".jpeg", ".png", ".jfif", ".gif" })]
        public IFormFile FormFile { get; set; }

        public IList<string> SelectImages { get; set; }

        public IList<ImageSelectList> AvailableImages { get; set; }

        public ImageView()
        {
            SelectImages = new List<string>();
            AvailableImages = new List<ImageSelectList>();
        }
    }
}
