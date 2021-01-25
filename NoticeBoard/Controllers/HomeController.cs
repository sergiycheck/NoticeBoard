using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoticeBoard.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message = null)
        {
            var errorViewModel = new ErrorViewModel();
            errorViewModel.Message = message ?? "";
            errorViewModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            return View(errorViewModel);
        }
    }
}
