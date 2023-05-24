using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace ProjectWebNotes.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public ResetPasswordModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required (ErrorMessage ="{0} không được bỏ trống")]
            [EmailAddress(ErrorMessage ="{0} không đúng")]
            public string Email { get; set; }

            [Required(ErrorMessage = "{0} không được bỏ trống")]
            [Display(Name ="Mật khẩu")]
            [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Nhập lại mật khẩu")]
            [Compare("Password", ErrorMessage = "Nhập lại mật khẩu không trùng nhau.")]
            public string ConfirmPassword { get; set; }

            public string token { get; set; }
        }

        public IActionResult OnGetComfirmed(string token = null)
        {
            if (token == null)
            {
                return BadRequest("Không có mã.");
            }
            else
            {
                Input = new InputModel
                {
                    token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token))
                };
                return Page();
            }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Nổ lực thay đổi không hợp lệ.");
                return Page();
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.token, Input.Password);
            if (result.Succeeded)
            {
                StatusMessage = $"{user.Email} thay đổi mật khẩu thành công.";
                return RedirectToPage();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
