using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoticeBoard.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoticeBoard.AuthorizationsManagers
{
    public class CustomUserManager : UserManager<IdentityUser>, ICustomUserManager
    {
        public CustomUserManager(IUserStore<IdentityUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<IdentityUser> passwordHasher, IEnumerable<IUserValidator<IdentityUser>> userValidators,
            IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<IdentityUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
        public override Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        {
            return base.CreateAsync(user, password);
        }

        public override Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            return base.CreateAsync(user);
        }

        public override Task<IdentityUser> FindByEmailAsync(string email)
        {
            return base.FindByNameAsync(email);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
        {
            return base.GetAuthenticatorKeyAsync(user);
        }

        public override string GetUserId(ClaimsPrincipal principal)
        {
            return base.GetUserId(principal);
        }

        public override Task<string> GetUserIdAsync(IdentityUser user)
        {
            return base.GetUserIdAsync(user);
        }
    }
}
