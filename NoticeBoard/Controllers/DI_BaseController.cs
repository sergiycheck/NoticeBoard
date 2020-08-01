using NoticeBoard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace NoticeBoard.Controllers
{
    public class DI_BaseController : Controller
    {
        protected readonly NoticeBoardDbContext _context;
        protected readonly IAuthorizationService _authorizationService;
        protected readonly UserManager<IdentityUser> _userManager;

        public DI_BaseController(
            NoticeBoardDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base()
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        } 
    }
}