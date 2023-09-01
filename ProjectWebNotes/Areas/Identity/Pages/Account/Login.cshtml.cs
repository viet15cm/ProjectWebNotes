using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ProjectWebNotes.Services.MailServices;

namespace ProjectWebNotes.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ISendMailServices _sendMail;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<AppUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<AppUser> userManager,
            ISendMailServices sendMailServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _sendMail = sendMailServices;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {

            //[Required(ErrorMessage = "{0} không được bỏ trống")]
            //[EmailAddress]
            //public string Email { get; set; }

            [Required(ErrorMessage = "{0} Không được để trống")]
            [Display(Name = "Tên hoặc Email")]
            [StringLength(100, MinimumLength = 1, ErrorMessage = "Nhập đúng thông tin")]
            public string UserNameOrEmail { set; get; }

            [Required(ErrorMessage = "{0} không được bỏ trống")]
            [Display(Name ="Mật khẩu")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Nhớ mật khẩu ?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGet(string returnUrl = null)
        {


            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

           //danh sách  nhà cung cấp thông tin đăng nhập bên ngoài đã có.
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (_signInManager.IsSignedIn(User))
            {
                return LocalRedirect(returnUrl);
            }

            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByEmailAsync(Input.Email);

                //if (user == null)
                //{
                //    ModelState.AddModelError(string.Empty, "Email không tồn tại.");
                //    return Page();
                //}

                //var isCheck = ValidateUser(Input.Password , user);

                //if (isCheck == false)
                //{
                //    ModelState.AddModelError(string.Empty, "Sai mật khẩu.");
                
                //}
                var result = await _signInManager.PasswordSignInAsync(Input.UserNameOrEmail, Input.Password, Input.RememberMe, true);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }

                //if (_userManager.Options.SignIn.RequireConfirmedEmail)
                //{
                //    var user = await _userManager.FindByEmailAsync(Input.Email);

                //    if (!user.EmailConfirmed)
                //    {
                //        return RedirectToPage("/Account/ConfirmedEmail", new { area = "Identity", id = user.Id });
                //    }
                //}
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToPage("/Account/ConfirmedNumberCodeEmail", new { area = "Identity"});
                //}

                if (result.IsLockedOut)
                {
                    _logger.LogInformation("khóa");
                    return RedirectToPage("./Lockout");
                }

                ModelState.AddModelError(string.Empty, "Nổ lực đăng nhập không hợp lệ.");
                
                return Page();
            }

            return Page();
        }

        public bool ValidateUser(string Password , AppUser user)
        {

            if (user == null)
            {
               return false;
            }

            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);
            if (result != PasswordVerificationResult.Success)
            {
                return false;
            }

            return true;
        }


        
    }

}
