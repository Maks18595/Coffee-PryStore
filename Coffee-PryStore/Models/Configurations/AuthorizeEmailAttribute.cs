
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;


namespace Coffee_PryStore.Models.Configurations
{
    public class AuthorizeEmailAttribute : AuthorizeAttribute, IAuthorizationRequirement
    {
        public string AllowedEmail { get; }

        public AuthorizeEmailAttribute(string allowedEmail)
        {
            AllowedEmail = allowedEmail;
        }

        public bool IsEmailAuthorized(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated &&
                   user.FindFirst(ClaimTypes.Email)?.Value.Equals(AllowedEmail, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
