using System.Threading.Tasks;
using NoticeBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace NoticeBoard.Authorization
{
    public class NoticeAdministratorAuthorizationHandler:
    AuthorizationHandler<OperationAuthorizationRequirement,BaseModel>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            BaseModel resource)
        {
            if(context.User==null) 
                return Task.CompletedTask;

            if(context.User.IsInRole(NotificationConstants.ContactAdministratorsRole))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
