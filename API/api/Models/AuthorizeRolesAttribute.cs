using Microsoft.AspNetCore.Authorization;

namespace api.Models;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params Roles[] allowedRoles)
    {
        var allowedRolesAsStrings = allowedRoles.Select(x => Enum.GetName(typeof(Roles), x));
        Roles = string.Join(",", allowedRolesAsStrings);
    }
}