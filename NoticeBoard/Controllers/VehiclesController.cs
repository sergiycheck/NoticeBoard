using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Data;
using NoticeBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NoticeBoard.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using NoticeBoard.Interfaces;
using NoticeBoard.Helpers;
using NoticeBoard.Models.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoticeBoard.Controllers
{
    public class VehiclesController : DI_BaseController
    {


        public VehiclesController(
            ICustomAuthorizationService authorizationService,
            ICustomUserManager userManager,
            ILogger<DI_BaseController> logger):base(authorizationService,userManager,logger)
        {
 
        }
                // GET: Notification
        [AllowAnonymous]
        public IActionResult Index()
        {
            _logger.LogInformation("method: Get. Vehicles controller index");
            return View();
        }
    }
}