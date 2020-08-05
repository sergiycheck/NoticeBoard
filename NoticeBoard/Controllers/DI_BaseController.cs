using NoticeBoard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace NoticeBoard.Controllers
{
    public class DI_BaseController : Controller
    {
        protected readonly IAuthorizationService _authorizationService;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly ILogger<DI_BaseController> _logger;

        public DI_BaseController(
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            ILogger<DI_BaseController> logger) : base()
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _logger = logger;
        } 
    }
}