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


namespace NoticeBoard.Controllers
{
    public class NotificationController : DI_BaseController
    {

        private INotificationsRepository _repository;
        public NotificationController(
            ICustomAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            ILogger<DI_BaseController> logger,
            INotificationsRepository repository):base(authorizationService,userManager,logger)
        {
            _repository = repository;    
        }

        // GET: Notification
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("method: Get. Notification controller index");
            return View(await _repository.GetAll().ToListAsync());
        }

        // GET: Notification/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _repository.NotificationIncludeComments(id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }
        //TODO:add method or controller to post comments

        // GET: Notification/Create
        public IActionResult Create()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return Challenge();
            }
            ViewBag.UserId = _userManager.GetUserId(User);

            return View();
        }

        // POST: Notification/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                notification.OwnerID = _userManager.GetUserId(User);
                if(!await CheckIfUserAuthorizedForNotification(notification,NotificatinOperations.Create))
                {
                    return Forbid();
                }

                await _repository.Create(notification);
                await _repository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }
        private async Task<bool> CheckIfUserAuthorizedForNotification(
                                    Notification notification,
                                    OperationAuthorizationRequirement requirement)
        {
            var isAuthorized = await _authorizationService
                                        .AuthorizeAsync(User,notification,requirement);
            if(!isAuthorized.Succeeded)
            {
                return false;
            }
            return true;

        }

        // GET: Notification/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var notification = await _repository.NotificationIncludeComments(id);
            if (notification == null)
            {
                return NotFound();
            }
            if(!await CheckIfUserAuthorizedForNotification(notification,NotificatinOperations.Update))
            {
                return Forbid();
            }

            return View(notification);
        }

        // POST: Notification/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Notification notification)
        {
            if (id != notification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var notificationDb = await _repository.GetById(id);
                    if(notificationDb==null)
                    {
                        return NotFound();
                    }
                    notification.OwnerID=notificationDb.OwnerID;

                    if(!await CheckIfUserAuthorizedForNotification(notification,NotificatinOperations.Update))
                    {
                        return Forbid();
                    }
                    _repository.Update(notification);
                    await _repository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        //TODO:Continue here
        // GET: Notification/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _repository.GetById(id);

            if (notification == null)
            {
                return NotFound();
            }
            if(!await CheckIfUserAuthorizedForNotification(notification,NotificatinOperations.Delete))
            {
                return Forbid();
            }


            return View(notification);
        }

        // POST: Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _repository.GetById(id);
            if(notification==null)
            {
                return NotFound();
            }

            if(!await CheckIfUserAuthorizedForNotification(notification,NotificatinOperations.Delete))
            {
                return Forbid();
            }

            await _repository.Delete(id);
            await _repository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _repository.EntityExists(id);
        }
    }
}
