using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using ProjectWebNotes.DbContextLayer;
using Domain.IdentityModel;
using Entities.Models;

namespace ProjectWebNotes.Security.Requirements
{
    public class CanOptionPostUserHandler : AuthorizationHandler<CanOptionPostUserRequirements, Post>
    {
        
        private readonly UserManager<AppUser> _userManager;

        private readonly AppDbcontext _context;

        public CanOptionPostUserHandler(UserManager<AppUser> userManager, AppDbcontext appDbcontext)
        {         
            _userManager = userManager;
            _context = appDbcontext;
        }

        protected override async  Task HandleRequirementAsync(AuthorizationHandlerContext context, CanOptionPostUserRequirements requirement, Post post)
        {
            var user = _userManager.GetUserAsync(context.User).Result;

            if (context.User.IsInRole("Administrator"))
            {
                context.Succeed(requirement);
                await Task.CompletedTask;
            }


            if (user.Id == post.AuthorId)
            {
                context.Succeed(requirement);
                await Task.CompletedTask;
             
            }

            await Task.CompletedTask;
        }
    }
}
