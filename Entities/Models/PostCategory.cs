using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class PostCategory
    {
        public string PostID { set; get; }

        public string CategoryID { set; get; }
        
        [ForeignKey("PostID")]
        public Post Post { set; get; }

        
        [ForeignKey("CategoryID")]
        public Category Category { set; get; }

        
        [DisplayName("Thứ tự")]
        public int Serial { get; set; }

    }
}
