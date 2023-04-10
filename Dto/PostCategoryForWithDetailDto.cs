
using System.ComponentModel;


namespace Dto
{
    public class PostCategoryForWithDetailDto
    {
        public string PostID { set; get; }

        public string CategoryID { set; get; }

        public PostForWithDetailDto Post { set; get; }

        public CategoryForWithDetailDto Category { set; get; }

        [DisplayName("Thứ tự")]
        public int Serial { get; set; }
    }
}
