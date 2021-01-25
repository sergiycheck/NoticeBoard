using NoticeBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using NoticeBoard.AuthorizationsManagers;
using NoticeBoard.Interfaces;

namespace NoticeBoard.Authorization
{
    public class NoticetIsOwnerAuthorizationHandler:AuthorizationHandler<OperationAuthorizationRequirement,BaseModel>
    {
        ICustomUserManager _userManager;

        public NoticetIsOwnerAuthorizationHandler(ICustomUserManager
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
                                   AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   BaseModel resource)
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

            if (resource.OwnerID == _userManager.GetUserId(context.User))//if user is owner of selected item
            {
                context.Succeed(requirement);//contact OperationAuthorizationRequirement is Succed
            }

            return Task.CompletedTask;// Task result affects on future event handlers
        }
    }
}