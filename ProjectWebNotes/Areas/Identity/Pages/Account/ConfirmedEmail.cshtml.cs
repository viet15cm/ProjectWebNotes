using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ProjectWebNotes.Services.MailServices;

namespace ProjectWebNotes.Areas.Identity.Pages.Account
{
    public class ConfirmedEmailModel : PageModel
    {

        private readonly ISendMailServices _sendMail;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<ConfirmedEmailModel> _logger;

        public ConfirmedEmailModel(ILogger<ConfirmedEmailModel> logger,
                                    UserManager<AppUser> userManager,
                                    SignInManager<AppUser> signInManager,
                                    ISendMailServices sendMailServices)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _sendMail = sendMailServices;
        }

        public async Task<IActionResult> OnGetConfirmed(string token, string returUrl)
        {
            returUrl = returUrl ?? Url.Content("~/");

           

            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect(returUrl);
            }

          

            if (string.IsNullOrEmpty(token))
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectToPage();
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            StatusMessage = result.Succeeded ? $"xác nhận email {user.Email} thành công." : $"xác nhận email {user.Email} không thành công.";
            return RedirectToPage("./Manager/SuccessConfirmEmail");

        }
        public async Task<IActionResult> OnGetAsync(string returUrl)
        {
            ReturnURL = returUrl ?? Url.Content("~/");

            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect(ReturnURL);
            }


            var user = await _userManager.GetUserAsync(User);


            if (user == null)
            {
                return NotFound($"Lỗi !!!!");
            }

            var email = user.Email;
            
            input = new InputModel()
            {
                Email = email
                
            };

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                StatusMessage = $"Đã xác nhận email {user.Email} thành công.";
                return RedirectToPage("./Manager/SuccessConfirmEmail");

            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(InputModel input , string returUrl)
        {

            ReturnURL = returUrl ?? Url.Content("~/");

            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect(ReturnURL);
            }


            var user = await _userManager.GetUserAsync(User);


            if (user == null)
            {
                return NotFound($"Lỗi!!!!");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                StatusMessage = $"xác nhận email {user.Email} thành công.";
                return RedirectToPage();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var callbackUrl = Url.Page(
                "/Account/ConfirmedEmail",
                pageHandler: "Confirmed",
                values: new { area = "Identity", token = token, returnUrl = returUrl },
                protocol: Request.Scheme);
            var mailContent = new MailContent()
            {
                To = user.Email,
                Subject = "Xác nhận địa chỉ email",
                Body = $"Nhấn vào <a href='{callbackUrl}'>đây</a> để xác nhận Email "
            };

            var isSendEmail = await _sendMail.SendMailAsync(mailContent);

            StatusMessage = isSendEmail ? "Gửi email thành công !!" : "Gửi Email thất bại !!";

            return RedirectToPage("/Account/ConfirmedEmail", new { area = "Identity", id = user.Id }); 

        }


        [BindProperty]
        public InputModel input { get; set; }

        public string ReturnURL { get; set; }
        
        [TempData]
        public string StatusMessage { get; set; }


        public class InputModel
        {       
            public string Email { get; set; }

            public bool IsConfirmed {get; set;}
        }

    }

}
