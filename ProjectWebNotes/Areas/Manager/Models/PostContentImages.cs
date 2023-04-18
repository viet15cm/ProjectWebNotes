using Dto;
using System.Reflection.Metadata.Ecma335;

namespace ProjectWebNotes.Areas.Manager.Models
{
    public class PostContentImages
    {
        public string TitlePost { get; set; }
        public PostForUpdateContentDto PostForUpdateContent { get; set; }

        public ICollection<ImageDto> ImageDtos { get; set; }
    }
}
