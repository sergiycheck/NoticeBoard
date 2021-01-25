using Microsoft.AspNetCore.Identity;
using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Helpers.CustomUserValidator
{
    public interface IUserValidator<TUser> where TUser : class
    {
        Task<IdentityResult> ValidateAsync(ICustomUserManager manager, TUser user);
    }
    public class CustomUserValidator : IUserValidator<CustomUser>
    {
        public Task<IdentityResult> ValidateAsync(ICustomUserManager userManager, CustomUser user)
        {
            var errors = new List<IdentityError>();
            if (user.Email.ToLower().EndsWith("@spam.com")) 
            {
                errors.Add(new IdentityError() { Description = $"this domain {user.Email} is banned" });
            }
            if (user.UserName.ToLower().Contains("admin")) 
            {
                errors.Add(new IdentityError() { Description = $"user name  {user.UserName} can not contain phrase like \'admin\' " });
            }
            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
