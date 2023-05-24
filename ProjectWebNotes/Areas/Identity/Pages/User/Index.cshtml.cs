
using Domain.IdentityModel;
using Dto;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Paging;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.User
{

    public class IndexModel : UserPageModel
    {
      
        [BindProperty]
        public string SearchUserName { get; set; }
        
      
        
        public IndexModel(UserManager<AppUser> userManager, ILogger<UserPageModel> logger, AppDbcontext dbContext , RoleManager<IdentityRole> roleManager) : base(userManager, logger, dbContext, roleManager)
        {

        }

        public PagedList<UserAndRole> users { get; set; }

       public class UserAndRole : AppUser
        {
            public string UserRoles { get; set; }
        }

        public async Task<IActionResult> OnGet([FromQuery] UserParameters userParameters)
        {
           
            PagedList<AppUser> pageUsers = PagedList<AppUser>.ToPagedList(_userManager.Users.OrderBy(u => u.UserName),
                        userParameters.PageNumber,
                        userParameters.PageSize);

            List<UserAndRole> derivedList = pageUsers.ToList().Select(x => new UserAndRole
            {

                Id = x.Id,
                UserName = x.UserName

            }).ToList();

            users = new PagedList<UserAndRole>(derivedList, pageUsers.TotalCount, userParameters.PageNumber, userParameters.PageSize);

            foreach (var item in users)
            {
                var roles = await _userManager.GetRolesAsync(item);
                item.UserRoles = string.Join(",", roles);
                
            }

            return Page();

        }

    }
}
