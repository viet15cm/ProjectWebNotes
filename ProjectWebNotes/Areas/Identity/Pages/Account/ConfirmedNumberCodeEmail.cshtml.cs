using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ModelValidation;
using ProjectWebNotes.Services.MailServices;

namespace ProjectWebNotes.Areas.Identity.Pages.Account
{
    public class ConfirmedNumberCodeEmailModel : PageModel
    {

        private readonly ISendMailServices _sendMail;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<ConfirmedEmailModel> _logger;

        public ConfirmedNumberCodeEmailModel(ILogger<ConfirmedEmailModel> logger,
                                    UserManager<AppUser> userManager,
                                    SignInManager<AppUser> signInManager,
                                    ISendMailServices sendMailServices)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _sendMail = sendMailServices;
        }


        public async Task<IActionResult> OnGet()
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return NotFound();
            }

            Input = new InputModel();
            return Page();
        }

        public async Task<IActionResult> OnGetSendMail()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.ResetAuthenticatorKeyAsync(user);

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var mailContent = new MailContent()
            {
                To = user.Email,
                Subject = "Xác thực người dùng",
                Body = code
            };

            var isSendEmail = await _sendMail.SendMailAsync(mailContent);

            StatusMessage = isSendEmail ? $"gửi mã xác thực tới {user.Email} thành công." : $"gửi mã xác thực tới {user.Email} thất bại.";

            if (true)
            {
                Input = new InputModel()
                {
                    Code = code
                };
                
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPost(string retrunUrl)
        {
            retrunUrl = retrunUrl ?? Url.Content("~/");
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            
            if (user == null)
            {
                return NotFound();
            }
           
            var code = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
         
          
           
            var result = await _signInManager.TwoFactorSignInAsync("Email", code, true, false);
            
            if (result.Succeeded)
            {  
                return LocalRedirect(retrunUrl);
            }

            ModelState.AddModelError(string.Empty, $"Lỗi sai mã code");

            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        public class InputModel
        {
            [StringLength(6 , ErrorMessage =("Độ dài tối đa là 7 chữ số"))]
            [Required(ErrorMessage = "{0} không được bỏ trống.")]
            [NumberCodeConfirmed]
            public string Code { get; set; }
            
        }
    }
}
