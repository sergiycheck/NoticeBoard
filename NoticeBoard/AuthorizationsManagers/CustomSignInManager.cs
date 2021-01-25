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
using System.Security.Claims;
using NoticeBoard.Models;

namespace NoticeBoard.AuthorizationsManagers
{
    public class CustomSignInManager : SignInManager<CustomUser>, ICustomSignInManager
    {
        public CustomSignInManager(UserManager<CustomUser> userManager, 
                                    IHttpContextAccessor contextAccessor, 
                                    IUserClaimsPrincipalFactory<CustomUser> claimsFactory, 
                                    IOptions<IdentityOptions> optionsAccessor, 
                                    ILogger<SignInManager<CustomUser>> logger, 
                                    IAuthenticationSchemeProvider schemes, 
                                    IUserConfirmation<CustomUser> confirmation) : base(userManager,
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
        public override Task<SignInResult> PasswordSignInAsync(CustomUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public override Task SignInAsync(CustomUser user, bool isPersistent, string authenticationMethod = null)
        {
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public override Task SignOutAsync()
        {
            return base.SignOutAsync();
        }
        public override bool IsSignedIn(ClaimsPrincipal principal)
        {
            return base.IsSignedIn(principal);
        }
        public override Task RefreshSignInAsync(CustomUser user)
        {
            return base.RefreshSignInAsync(user);
        }
    }
}
