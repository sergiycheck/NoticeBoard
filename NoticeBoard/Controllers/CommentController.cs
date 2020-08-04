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

namespace NoticeBoard.Controllers
{
    public class CommentController : DI_BaseController
    {

        public CommentController(NoticeBoardDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,ILogger<DI_BaseController> logger):base(context,authorizationService,userManager,logger)
        {    
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
            var noticeBoardDbContext = _context.Comments.AsNoTracking().Include(c => c.Notification);
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

            var comment = await _context.Comments
                .Include(c => c.Notification)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comment/Create
        public IActionResult Create()
        {
            ViewData["NotificationId"] = new SelectList(_context.Notifications, "NotificationId", "NotificationId");
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
                _context.Add(comment);
                await _context.SaveChangesAsync();
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

            var comment = await _context.Comments.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,NotificationId,Description")] Comment comment)
        {
            if (id != comment.CommentId)
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
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
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

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.CommentId == id);
            
            if (comment == null)
            {
                return NotFound();
            }
            comment.OwnerID = _userManager.GetUserId(User);
            if(!await CheckIfUserAuthorizedForNotification(comment,NotificatinOperations.Delete))
            {
                return Forbid();
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Redirect($"/Notification/Details/{comment.NotificationId}");

        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
