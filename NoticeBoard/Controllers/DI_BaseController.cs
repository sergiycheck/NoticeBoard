using NoticeBoard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoticeBoard.Helpers;


namespace NoticeBoard.Controllers
{
    public class DI_BaseController : Controller
    {
        protected readonly ICustomAuthorizationService _authorizationService;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly ILogger<DI_BaseController> _logger;

        public DI_BaseController(
            ICustomAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            ILogger<DI_BaseController> logger) : base()
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _logger = logger;
        } 
    }
}