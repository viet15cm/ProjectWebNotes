using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ProjectWebNotes.Services.MailServices;

namespace ProjectWebNotes.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ISendMailServices _sendMail;
        private ILogger<ForgotPasswordModel> _logger;
        public ForgotPasswordModel(
            UserManager<AppUser> userManager,
            ISendMailServices emailSender,
            ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _sendMail = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage ="{0} không được bỏ trống.")]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng đăng kí Email.");
                    return Page();
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: "Comfirmed",
                    values: new { area = "Identity", token = token },
                    protocol: Request.Scheme);

                var mailContent = new MailContent()
                {
                    To = user.Email,
                    Subject = "Đặt lại mật khẩu",
                    Body = $"Nhấn vào <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>đây</a> đặt lại mật khẩu."
                };

                var isSendEmail = await _sendMail.SendMailAsync(mailContent);

                StatusMessage = isSendEmail ? "Gửi Email thành công , vào email để lấy lại mật khẩu." : "Gửi Email thất bại.";
                
                if (isSendEmail)
                {
                    _logger.LogInformation("Gửi mail thanh cong");
                }
                return RedirectToPage("./ForgotPassword");
            }

            return Page();
        }
    }
}
