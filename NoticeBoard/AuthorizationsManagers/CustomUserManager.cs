using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using NoticeBoard.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoticeBoard.AuthorizationsManagers
{
    public class CustomUserManager : UserManager<CustomUser>, ICustomUserManager
    {
        private ICustomUserRepository _customUserRepository;
        public CustomUserManager(IUserStore<CustomUser> store, 
                                IOptions<IdentityOptions> optionsAccessor,
                                IPasswordHasher<CustomUser> passwordHasher, 
                                IEnumerable<IUserValidator<CustomUser>> userValidators,
                                IEnumerable<IPasswordValidator<CustomUser>> passwordValidators, 
                                ILookupNormalizer keyNormalizer,
                                IdentityErrorDescriber errors, 
                                IServiceProvider services, 
                                ILogger<UserManager<CustomUser>> logger,
                                ICustomUserRepository customUserRepository
                                )
                                : base(store, 
                                      optionsAccessor, 
                                      passwordHasher, 
                                      userValidators, 
                                      passwordValidators, 
                                      keyNormalizer, 
                                      errors, 
                                      services, 
                                      logger)
        {
            _customUserRepository = customUserRepository;
        }
        public new IdentityOptions Options{get=> base.Options;set=>base.Options=value;}//not sure here
        
        public override Task<IdentityResult> CreateAsync(CustomUser user, string password)
        {
            return base.CreateAsync(user, password);
        }
        public async Task<bool> UserExists(string id) 
        {
            return await _customUserRepository.EntityExists(id);
        }

        public override Task<IdentityResult> CreateAsync(CustomUser user)
        {
            return base.CreateAsync(user);
        }

        public override Task<CustomUser> FindByEmailAsync(string email)
        {
            return base.FindByEmailAsync(email);
        }
        public override Task<IdentityResult> ConfirmEmailAsync(CustomUser user, string code)
        {
            return base.ConfirmEmailAsync(user, code);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(CustomUser user)
        {
            return base.GenerateEmailConfirmationTokenAsync(user);
        }

        public override string GetUserId(ClaimsPrincipal principal)
        {
            return base.GetUserId(principal);
        }

        public override Task<string> GetUserIdAsync(CustomUser user)
        {
            
            return base.GetUserIdAsync(user);
        }
        public override Task<CustomUser> FindByNameAsync(string userName)
        {
            return base.FindByNameAsync(userName);
        }
        public override Task<CustomUser> FindByIdAsync(string userId)
        {
            //return _customUserRepository.GetUserByIdAsNoTracking(userId);

            return base.FindByIdAsync(userId);
        }
        public override Task<bool> IsInRoleAsync(CustomUser user,string role)
        {
            return base.IsInRoleAsync(user,role);
        }
        public override Task<IdentityResult> AddToRoleAsync(CustomUser user, string role)
        {
            return base.AddToRoleAsync(user,role);
        }
        public override Task<CustomUser> GetUserAsync(ClaimsPrincipal user)
        {
            return base.GetUserAsync(user);
        }

        public override Task<IdentityResult> UpdateAsync(CustomUser user)
        {
            return base.UpdateAsync(user);
        }
        public override Task<IdentityResult> DeleteAsync(CustomUser user) 
        {
            return base.DeleteAsync(user);
        }
        public override  Task<IdentityResult> ChangePasswordAsync(CustomUser user, string oldPassword, string newPassword) 
        {
            return base.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<ICollection<CustomUser>> ListUsersAsync()
        {
            return await base.Users.AsNoTracking().ToListAsync();
        }


        public override  Task<IdentityResult> AddToRolesAsync(CustomUser user, IEnumerable<string> addedRoles) 
        {
            return base.AddToRolesAsync(user, addedRoles);
        }
        public override Task<IdentityResult> RemoveFromRolesAsync(CustomUser user, IEnumerable<string> removedRoles) 
        {
            return base.RemoveFromRolesAsync(user, removedRoles);
        }
    }
}
