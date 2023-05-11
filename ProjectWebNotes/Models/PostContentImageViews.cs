using Dto;

namespace ProjectWebNotes.Models
{
    public class PostContentImageViews
    {       
        public PostForUpdateContentDto PostForUpdateContent { get; set; }

        public ICollection<ImageDto> ImageDtos { get; set; }
    }
}
