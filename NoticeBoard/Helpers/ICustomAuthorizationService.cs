using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoticeBoard.Helpers
{
    public interface ICustomAuthorizationService {
     Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resourse,IAuthorizationRequirement requirement);
}
}