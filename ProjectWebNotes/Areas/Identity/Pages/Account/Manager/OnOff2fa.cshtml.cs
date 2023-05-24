using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ProjectWebNotes.Areas.Identity.Pages.Account.Manager
{
    public class OnOff2faModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        //private readonly SignInManager<AppUser> _signInMamager;
        private readonly ILogger<OnOff2faModel> _logger;

        public OnOff2faModel(
            UserManager<AppUser> userManager,
            ILogger<OnOff2faModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public string UserName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel input { get; set; }
        //[BindProperty]
        //public InputText inputText { get; set; }

        //public class InputText
        //{
        //    [Required(ErrorMessage = "Ten Khong duoc bo trong")]
        //    public string Text { get; set; }
        //}


        public class InputModel
        {
            [Display(Name = "Xác thực hai yếu tố")]
            public bool Check2fa { get; set; }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Ensure the user has gone through the username & password screen first

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Phiên đã hết hạn");
            }

            if (!user.EmailConfirmed)
            {
                return RedirectToPage("/Account/ConfirmedEmail", new { area = "Identity"});
            }

            input = new InputModel();

            input.Check2fa = await _userManager.GetTwoFactorEnabledAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng có ID'{_userManager.GetUserId(User)}'.");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!input.Check2fa)
            {
                var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);

                if (!disable2faResult.Succeeded)
                {
                    throw new InvalidOperationException($"Đã xảy ra lỗi không mong muốn khi tắt 2FA cho người dùng có ID'{_userManager.GetUserId(User)}'.");
                }

                _logger.LogInformation($"Người dùng đã tắt 2fa.");

                StatusMessage = "Error Xác Thực hai yếu tố đã bị tắt.";
            }
            else
            {
                var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, true);

                if (!disable2faResult.Succeeded)
                {
                    throw new InvalidOperationException($"Đã xảy ra lỗi không mong muốn khi tắt 2FA cho người dùng có ID'{_userManager.GetUserId(User)}'.");
                }

                StatusMessage = "Xác Thực hai yếu tố đã bật.";
            }
            return RedirectToPage();
            //return new JsonResult(input.Check2fa);
        }


        // co the dung ajax hiện tại không liên quan 
        public IActionResult OnGetPartial([FromQuery] bool Message = true)
        {

            var message = Message ? "Xác Thực hai yếu tố đã bật" : "Error Xác Thực hai yếu tố đã bị tắt";

            return new PartialViewResult
            {
                ViewName = "_StatusMessage",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary
                <string>(ViewData, message)
            };

        }
    }
}
