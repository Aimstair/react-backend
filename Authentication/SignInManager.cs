using ASI.Basecode.Data.Data.Entities;
using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ASI.Basecode.Resources.Constants;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Authentication
{
    public class SignInManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SignInManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsIdentity CreateClaimsIdentity(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            return new ClaimsIdentity(claims, Const.AuthenticationScheme);
        }

        public IPrincipal CreateClaimsPrincipal(ClaimsIdentity identity)
        {
            return new ClaimsPrincipal(identity);
        }

        public async Task SignInAsync(User user, bool isPersistent = false)
        {
            var identity = CreateClaimsIdentity(user);
            var principal = CreateClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(
                Const.AuthenticationScheme, // Must match AddCookie() scheme
                (ClaimsPrincipal)principal,
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(Const.AuthenticationScheme);
        }
    }
}
