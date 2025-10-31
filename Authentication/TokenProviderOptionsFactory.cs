using ASI.Basecode.Resources.Constants;
using ASI.Basecode.WebApp.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ASI.Basecode.Services.Interfaces;


namespace ASI.Basecode.WebApp.Authentication
{
    public static class ServiceProviderAccessor
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }

    /// <summary>
    /// Token provider factory
    /// </summary>
    public class TokenProviderOptionsFactory
    {
        /// <summary>
        /// Creates the token
        /// </summary>
        /// <param name="token">Token authentication</param>
        /// <param name="signingKey">Signing key</param>
        /// <returns>Token Provider Options</returns>
        public static TokenProviderOptions Create(TokenAuthentication token, SymmetricSecurityKey signingKey)
        {
            var options = new TokenProviderOptions
            {
                Path = token.TokenPath,
                Audience = token.Audience,
                Issuer = Const.Issuer,
                Expiration = TimeSpan.FromMinutes(token.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = async (username, password) =>
                {
                    // Resolve services from DI
                    var userService = ServiceProviderAccessor.ServiceProvider.GetService<IUserService>();
                    var httpContextAccessor = ServiceProviderAccessor.ServiceProvider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();

                    if (userService == null || httpContextAccessor == null)
                        throw new Exception("Services not available for IdentityResolver.");

                    // Authenticate user
                    var user = await userService.AuthenticateUserAsync(username, password);
                    if (user == null)
                        return null;

                    // Create claims identity
                    var signInManager = new SignInManager(httpContextAccessor);
                    return signInManager.CreateClaimsIdentity(user);
                }
            };

            return options;
        }
    }
}
