using System.ComponentModel.DataAnnotations;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelValidation;
using ProjectWebNotes.Services.MailServices;

namespace ProjectWebNotes.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ISendMailServices _sendMail;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;


        public RegisterModel(
            ISendMailServices sendMail,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger
            )

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sendMail = sendMail;
            _logger = logger;

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var appUser = new AppUser() { UserName = Input.UserName , Email=Input.Email};

                var result = await _userManager.CreateAsync(appUser, Input.Password);
                if (result.Succeeded)
                {
                     StatusMessage = $"Đăng kí thành công tài khoản {appUser.UserName} ";
                    _logger.LogInformation("Thên thành công");

                    //if (_signInManager.Options.SignIn.RequireConfirmedEmail)
                    //{

                    //    return RedirectToPage("/Account/ConfirmedEmail", new { area = "Identity", id = appUser.Id });
                    //}
      
                    await _signInManager.SignInAsync(appUser, isPersistent: false);
                    return LocalRedirect(returnUrl);

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
         
            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} không được bỏ trống.")]
            [EmailAddress]
            [EmailValidations]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(20, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
            [NotEmailValidtions]
            [Display(Name = "Tên tài khoản")]
            public string UserName { set; get; }

            [StringLength(100, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            [Required(ErrorMessage = "{0} không được bỏ trống.")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Nhập lại mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu không giống nhau")]
            public string ConfirmPassword { get; set; }


        }
    }
}
