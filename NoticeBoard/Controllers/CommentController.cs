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

namespace NoticeBoard.Controllers
{
    public class CommentController : DI_BaseController
    {

        public CommentController(NoticeBoardDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager):base(context,authorizationService,userManager)
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
            if(!isAuthorized)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificationId,Description")] Comment comment)//
        {
            if (ModelState.IsValid)
            {
                if(!await CheckIfUserAuthorizedForNotification(comment,NotificatinOperations.Create))
                {
                    return Forbid();
                }
                comment.OwnerID = _userManager.GetUserId(User);
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction($"Details/{comment.NotificationId}","Notification");
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
            ViewData["NotificationId"] = new SelectList(_context.Notifications, "NotificationId", "NotificationId", comment.NotificationId);
            return View(comment);
        }

        // POST: Comment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,NotificationId,OwnerID,Description")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["NotificationId"] = new SelectList(_context.Notifications, "NotificationId", "NotificationId", comment.NotificationId);
            return View(comment);
        }

        // GET: Comment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
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

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
