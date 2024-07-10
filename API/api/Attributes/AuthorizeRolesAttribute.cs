using api.Models;
using Microsoft.AspNetCore.Authorization;

namespace api.Attributes;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params Roles[] allowedRoles)
    {
        var allowedRolesAsStrings = allowedRoles.Select(x => Enum.GetName(typeof(Roles), x));
        Roles = string.Join(",", allowedRolesAsStrings);
    }
}