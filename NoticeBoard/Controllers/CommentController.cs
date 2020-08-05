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

namespace NoticeBoard.Controllers
{
    public class CommentController : DI_BaseController
    {
        private ICommentsRepository _repository;
        public CommentController(
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            ILogger<DI_BaseController> logger,
            ICommentsRepository repository):base(authorizationService,userManager,logger)
        {
            _repository = repository;    
        }

        private async Task<bool> CheckIfUserAuthorizedForNotification(
                                    Comment comment,
                                    OperationAuthorizationRequirement requirement)
        {
            var isAuthorized = await _authorizationService
                                        .AuthorizeAsync(User,comment,requirement);
            if(!isAuthorized.Succeeded)
            {
                return false;
            }
            return true;

        }

        // GET: Comment
        public async Task<IActionResult> Index()
        {
            var isAuthorized = User.IsInRole(NotificationConstants.ContactAdministratorsRole);
            if(!isAuthorized)//if isAuthorized false !isAuthorized true
            {
                return Forbid();
            }
            var noticeBoardDbContext = _repository.CommentsIncludeNotification();
            return View(await noticeBoardDbContext.ToListAsync());
        }

        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = User.IsInRole(NotificationConstants.ContactAdministratorsRole);
            if(!isAuthorized)
            {
                return Forbid();
            }

            var comment = await _repository.CommentIncludeNotification(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comment/Create
        public IActionResult Create()
        {
            ViewData["NotificationId"] = new SelectList(_repository.GetAll(), "NotificationId", "NotificationId");
            return View();
        }

        // POST: Comment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificationId,Description")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.OwnerID = _userManager.GetUserId(User);
                if(!await CheckIfUserAuthorizedForNotification(comment,NotificatinOperations.Create))
                {
                    return Forbid();
                }
                await _repository.Create(comment);
                await _repository.SaveChangesAsync();
                return Redirect($"/Notification/Details/{comment.NotificationId}");
            }
            return BadRequest();
        }

        // GET: Comment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _repository.GetById(id);
            if (comment == null)
            {
                return NotFound();
            }
            if(!await CheckIfUserAuthorizedForNotification(comment,NotificatinOperations.Update))
            {
                return Forbid();
            }

            return View(comment);
        }

        // POST: Comment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NotificationId,Description")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comment.OwnerID = _userManager.GetUserId(User);
                    if(!await CheckIfUserAuthorizedForNotification(comment,NotificatinOperations.Update))
                    {
                        return Forbid();
                    }
                    _repository.Update(comment);
                    await _repository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect($"/Notification/Details/{comment.NotificationId}");
            }
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _repository.GetById(id);
            
            if (comment == null)
            {
                return NotFound();
            }
            comment.OwnerID = _userManager.GetUserId(User);
            if(!await CheckIfUserAuthorizedForNotification(comment,NotificatinOperations.Delete))
            {
                return Forbid();
            }
            await _repository.Delete(id);
            await _repository.SaveChangesAsync();
            return Redirect($"/Notification/Details/{comment.NotificationId}");

        }

        private bool CommentExists(int id)
        {
            return _repository.EntityExists(id);
        }
    }
}
