using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoticeBoard.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.AuthorizationsManagers
{
    public class CustomSignInManager : SignInManager<IdentityUser>, ICustomSignInManager
    {
        public CustomSignInManager(UserManager<IdentityUser> userManager, 
                                    IHttpContextAccessor contextAccessor, 
                                    IUserClaimsPrincipalFactory<IdentityUser> claimsFactory, 
                                    IOptions<IdentityOptions> optionsAccessor, 
                                    ILogger<SignInManager<IdentityUser>> logger, 
                                    IAuthenticationSchemeProvider schemes, 
                                    IUserConfirmation<IdentityUser> confirmation) : base(userManager,
                                                                                         contextAccessor, 
                                                                                         claimsFactory, 
                                                                                         optionsAccessor, 
                                                                                         logger, 
                                                                                         schemes, 
                                                                                         confirmation)
        {
        }
        public override Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return base.GetExternalAuthenticationSchemesAsync();
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public override Task SignInAsync(IdentityUser user, bool isPersistent, string authenticationMethod = null)
        {
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public override Task SignOutAsync()
        {
            return base.SignOutAsync();
        }
    }
}
