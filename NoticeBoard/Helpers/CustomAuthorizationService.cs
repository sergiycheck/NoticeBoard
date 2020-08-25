using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoticeBoard.Helpers
{
    public class CustomAuthorizationService: ICustomAuthorizationService {
    private readonly IAuthorizationService service;

    public CustomAuthorizationService(IAuthorizationService service) {
        this.service = service;
    }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resourse, IAuthorizationRequirement requirement) {
        var res = service.AuthorizeAsync(user, resourse, requirement);
        return res;
    }
}
}