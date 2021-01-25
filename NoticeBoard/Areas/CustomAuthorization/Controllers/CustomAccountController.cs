using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NoticeBoard.Models.ViewModels;
using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using NoticeBoard.Helpers.CustomMapper;

namespace NoticeBoard.Controllers
{
    [Area("CustomAuthorization")]
    public class CustomAccountController:Controller
    {
        private readonly ICustomSignInManager _signInManager;
        private readonly ICustomUserManager _userManager;
        private readonly ILogger<CustomAccountController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ICustomMapper _mapper;
        private readonly NoticeBoard.Helpers.CustomUserValidator.IUserValidator<CustomUser> _userValidator;


        //register properties
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public CustomAccountController(
            ICustomUserManager userManager,
            ICustomSignInManager signInManager,
            ILogger<CustomAccountController> logger,
            IEmailSender emailSender,
            ICustomMapper mapper,
            NoticeBoard.Helpers.CustomUserValidator.IUserValidator<CustomUser> userValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _mapper = mapper;
            _userValidator = userValidator;
        }
        [HttpGet]
        public async Task<IActionResult> Index() 
        {
            _logger.LogInformation("method: Get. CustomAccount controller index");
            if (User != null)
            {
                var user = await _userManager.GetUserAsync(User);
                return user == null ? NotFound() : View(_mapper.Map_EditUserViewModel(user));

            }
            return Challenge();
        }


        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(new RegisterInputModel(){ReturnUrl= returnUrl ?? "/" });
        }
       
        [AllowAnonymous]
        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterConfirmed
                    (/*[Bind("Email,Password")]*/RegisterInputModel Input)
        {
            var returnUrl = Input.ReturnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new CustomUser 
                { 
                    UserName = Input.UserName,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    PhoneNumber = Input.PhoneNumber,
                    Country = Input.Country,
                    City = Input.City,
                    Address = Input.Address
                    };
                var validationResult = await _userValidator.ValidateAsync(_userManager, user);
                if (validationResult.Succeeded) 
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var callbackUrl = await GenerateConfirmationLink(user, returnUrl);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // this property is set in startup (services.AddDefaultIdentity ...)
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToAction("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    AddErrors(result);
                }
                AddErrors(validationResult);

            }

            return View(Input);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToAction(nameof(Register));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            //set to true if you want to display page for registerconfirmation
            var registerConfirmationViewModel = new RegisterConfirmationViewModel()
            {
                Email = email,
                DisplayConfirmAccountLink = false
            };            
            //TODO: add email sender and confirmation https://app.sendgrid.com/

            if (registerConfirmationViewModel.DisplayConfirmAccountLink)
            {
                registerConfirmationViewModel.EmailConfirmationUrl = await GenerateConfirmationLink(user, returnUrl);      
            }

            return View(registerConfirmationViewModel);
        }

        private async Task<string> GenerateConfirmationLink(CustomUser user, string returnUrl)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //var userRegistrationToken = new UserRegistrationToken { UserId = userId, Token = code };
            //await _userManager.StoreUserTokenForConfirmation(userRegistrationToken);

            //Here we have asp net core identity implementation
            var emailConfirmationUrl = Url.Action(
                "ConfirmUser",
                "CustomAccount",
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            //when we click on generaged link that is in registerConfirmationViewModel.EmailConfirmationUrl
            // AspNetUser user EmailConfirmed property becomes true

            return emailConfirmationUrl;
        }
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmUser(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, true);
                return RedirectToRoute("default", new { controller = "Notification", action = "Index" });
            }
            else
            {
                AddErrors(result);
                return View();
            }
        }


        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var loginViewModel = new LoginViewModel()
            {
                ReturnUrl = returnUrl
            };
            return View(loginViewModel);
            
        }

        [AllowAnonymous]
        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginConfirmed([Bind("Email,Password,RememberMe,ReturnUrl")] LoginViewModel Input)
        {
            var returnUrl = Input.ReturnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //can passwordSignInAsync(TUser user ...) first have to check whene user exists and return it
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                     ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                     return View(new LoginViewModel(){ReturnUrl="/"});
                } 

                //setting up cookie authentication
                var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(Input);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(Input);
        }        

        [AllowAnonymous]
        public IActionResult Logout()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost, ActionName("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutConfirmed(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return RedirectToRoute("default", new { controller = "Notification", action = "Index"});
            }
            else
            {
                return RedirectToAction();
            }
        }


        [HttpPost, ActionName("EditUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel editUserView) 
        {
            if (ModelState.IsValid) 
            {
                var customUser = _mapper.Map_EditUserViewModel(editUserView);
                var user = await _userManager.FindByIdAsync(customUser.Id);
                if (await _userManager.UserExists(customUser.Id)) 
                {
                    user.UserName = customUser.UserName;
                    user.FirstName = customUser.FirstName;
                    user.LastName = customUser.LastName;
                    user.Country = customUser.Country;
                    user.City = customUser.City;
                    user.Address = customUser.Address;
                    user.PhoneNumber = customUser.PhoneNumber;
                    user.Email = customUser.Email;
                    
                    IdentityResult result  = await _userManager.UpdateAsync(user);

                    if (result.Succeeded) 
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        return RedirectToAction("Index", "CustomAccount");
                    }
                    else 
                    {
                        AddErrors(result);

                    }
                }
            }
            return View(editUserView);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string Id) 
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToRoute("default", new { controller = "Notification", action = "Index" });
                }
                else
                {
                    AddErrors(result);
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with Name '{User.Identity.Name}'.");
            }
            return View(new ChangePasswordViewModel() { });
        }
        [HttpPost, ActionName("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordConfirm(ChangePasswordViewModel changePasswordViewModel) 
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            changePasswordViewModel.StatusMessage = "Your password has been changed.";

            return View(changePasswordViewModel);
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }
        


    }
}