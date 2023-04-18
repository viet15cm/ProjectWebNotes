
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Dto
{
    public class ContentFWDPost : ContentDto
    {
        [ForeignKey("PostId")]
        [Display(Name = "Bài viết")]
        public virtual PostsFWDImagesDto Post { get; set; }
    }
}
