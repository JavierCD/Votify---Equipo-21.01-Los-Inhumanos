using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Votify.Core.Models;

namespace Votify.Web.Services
{
    public class SessionClaimsPrincipalFactory : IUserClaimsPrincipalFactory<Miembro>
    {
        public async Task<ClaimsPrincipal> CreateAsync(Miembro user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.GetType().Name)
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            return new ClaimsPrincipal(identity);
        }
    }
}
