using System.Text;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using ProjectWebNotes.Services.MailServices;

namespace ProjectWebNotes.Areas.Identity.Pages.Account.Manager
{
    public class ConfirmedChangeEmailModel : PageModel
    {
        private readonly ISendMailServices _sendMail;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<ConfirmedChangeEmailModel> _logger;

        public ConfirmedChangeEmailModel(ILogger<ConfirmedChangeEmailModel> logger,
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
            _logger.LogInformation("Đã vào");
            returUrl = returUrl ?? Url.Content("~/");

            if (!_signInManager.IsSignedIn(User))
            {
                _logger.LogInformation("Lỗi null");
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

            if (! await _userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectToPage();
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, token);

            _logger.LogInformation(result.Succeeded.ToString());

            if (result.Succeeded)
            {
                user.EmailConfirmed = false;

                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                return RedirectToPage("./ChangeEmail");
            }

            StatusMessage = result.Succeeded ? $"Kích hoặt ghông thành công." : $"kích hoặt thành công.";
            
            return Page();

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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(InputModel input, string returUrl)
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

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var callbackUrl = Url.Page(
                "/Account/Manager/ConfirmedChangeEmail",
                pageHandler: "Confirmed",
                values: new { area = "Identity", token = token, returnUrl = returUrl },
                protocol: Request.Scheme);
            var mailContent = new MailContent()
            {
                To = user.Email,
                Subject = "Kích hoặt đổi Email",
                Body = $"Nhấn vào <a href='{callbackUrl}'>đây</a> để kích hoặt đổi email "
            };

            var isSendEmail = await _sendMail.SendMailAsync(mailContent);

            StatusMessage = isSendEmail ? "Gửi email thành công !!" : "Gửi Email thất bại !!";

            return RedirectToPage("/Account/Manager/ConfirmedChangeEmail", new { area = "Identity"});

        }


        [BindProperty]
        public InputModel input { get; set; }

        public string ReturnURL { get; set; }

        [TempData]
        public string StatusMessage { get; set; }


        public class InputModel
        {
            public string Email { get; set; }

        }
    }
}
