using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.Role
{

    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, ILogger<IndexModel> logger, AppDbcontext context) : base(roleManager, logger, context)
        {
        }

        public List<RoleAddClamis> roleClamis { get; set; }
        

        public class RoleAddClamis : IdentityRole
        {
            public string[] Clamis { get; set; }

        }

        public async Task OnGet()
        {
            roleClamis = await _roleManager.Roles.Select(r => new RoleAddClamis() { 
            
                Id = r.Id,
                Name = r.Name

            }).ToListAsync();

            foreach (var item in roleClamis)
            {
                var clamis = await _roleManager.GetClaimsAsync(item);
                var stringClamis = clamis.Select(c => c.Type + "=" + c.Value).ToArray();
                item.Clamis = stringClamis;
            }
        }
    }
}
