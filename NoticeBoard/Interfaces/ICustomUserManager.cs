using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NoticeBoard.Interfaces
{
    public interface ICustomUserManager
    {
        string GetUserId(ClaimsPrincipal principal);
        Task<IdentityResult> CreateAsync(IdentityUser user, string password);
        Task<IdentityResult> CreateAsync(IdentityUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user);
        //add property _userManager.Options.SignIn.RequireConfirmedAccount
        Task<IdentityUser> FindByEmailAsync(string email);
        Task<string> GetUserIdAsync(IdentityUser user);
    }
}