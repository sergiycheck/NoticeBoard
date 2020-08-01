using NoticeBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace NoticeBoard.Authorization
{
    public class NoticetIsOwnerAuthorizationHandler:AuthorizationHandler<OperationAuthorizationRequirement,Notification>
    {
        UserManager<IdentityUser> _userManager;

        public NoticetIsOwnerAuthorizationHandler(UserManager<IdentityUser> 
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Notification resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != NotificationConstants.CreateOperationName &&
                requirement.Name != NotificationConstants.ReadOperationName   &&
                requirement.Name != NotificationConstants.UpdateOperationName &&
                requirement.Name != NotificationConstants.DeleteOperationName )
            {
                return Task.CompletedTask;
            }

            if (resource.OwnerID == _userManager.GetUserId(context.User))//if user is owner of selected 
            //contact OperationAuthorizationRequirement is Succed
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;// Task result affects on future event handlers
        }
    }
}