using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NoticeBoard.Models;

namespace NoticeBoard.Interfaces
{
    public interface ICustomUserManager
    {
        string GetUserId(ClaimsPrincipal principal);
        Task<IdentityResult> CreateAsync(CustomUser user, string password);
        Task<IdentityResult> CreateAsync(CustomUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(CustomUser user);
        //add property _userManager.Options.SignIn.RequireConfirmedAccount
        Task<CustomUser> FindByEmailAsync(string email);
        Task<string> GetUserIdAsync(CustomUser user);
        public IdentityOptions Options{get;set;}
        Task<CustomUser> FindByNameAsync(string userName);
        Task<CustomUser> FindByIdAsync(string userId);
        Task<IdentityResult> AddToRoleAsync(CustomUser user, string role);
    }
}