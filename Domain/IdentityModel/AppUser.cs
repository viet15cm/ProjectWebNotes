using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Domain.IdentityModel
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Column(TypeName = "Char")]
        [StringLength(100)]
        public string UrlImage { get; set; }

        [Display(Name = "Họ")]
        [StringLength(20, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string LastName { get; set; }

        [Display(Name = "Tên")]
        [StringLength(20, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? BirthDate { set; get; }

        [Display(Name = "Công ty")]
        [StringLength(50, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 5)]
        public string Company { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Mô tả bản thân")]
        public string Describe { get; set; }

        [Display(Name = "Quê quán")]
        [StringLength(100, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 5)]
        public string NativePlace { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

    }
}
