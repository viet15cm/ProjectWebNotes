
using System.ComponentModel.DataAnnotations;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectWebNotes.Areas.Identity.Pages.Account.Manager
{
    public class DescribeModel : PageModel
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;



        public DescribeModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager
           
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            
        }

        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGet()
        {

            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("~/");
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            Input = new InputModel()
            {
                Describe = user.Describe
            };

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("~/");
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                user.Describe = Input.Describe;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {

                    await _signInManager.RefreshSignInAsync(user);
                    StatusMessage = "Thông tin mô tả bản thân của bạn đã được cập nhật";

                    return RedirectToPage("ProFile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();

            }

            return Page();

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            public string UrlImage { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Mô tả bản thân")]
            public string Describe { get; set; }


        }

    }

   

    
}
